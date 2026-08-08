// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.ServiceProcess;
using System.Linq;


namespace Tac.Sql
{

	public static class ServiceManager
	{
		private static readonly object _lock = new object();
		private static bool _initialized = false;

		public static void EnsureServiceInstalledAndRunning()
		{
			lock (_lock)
			{
				if (_initialized) return;
				_initialized = true;

				try
				{
					// 1. Проверяем, существует ли сервис
					ServiceController sc = ServiceController.GetServices()
						.FirstOrDefault(s => s.ServiceName == PipeConstants.ServiceName);

					if (sc == null)
					{
						// Сервис не установлен – устанавливаем
						InstallService();
						sc = new ServiceController(PipeConstants.ServiceName);
					}

					// 2. Запускаем, если остановлен
					if (sc.Status == ServiceControllerStatus.Stopped)
					{
						sc.Start();
						sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
					}
					else if (sc.Status == ServiceControllerStatus.Running)
					{
						// Уже работает – хорошо
					}
					else
					{
						// Другие состояния (Paused и т.д.) – пробуем запустить
						if (sc.Status != ServiceControllerStatus.Running)
						{
							sc.Start();
							sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
						}
					}
				}
				catch (Exception ex)
				{
					// Не удалось установить/запустить – будем использовать fallback
					// Можно залогировать в EventLog или файл
					Trace.WriteLine($"Не удалось настроить сервис логов: {ex.Message}");
					// Запоминаем, что сервис недоступен
					ServiceAvailabilityCache.SetUnavailable();
				}
			}
		}

		private static void InstallService()
		{
			string serviceExePath = Path.Combine(PipeConstants.ServicePathInstall, "TacSqlService.exe");
			if (!File.Exists(serviceExePath))
				throw new FileNotFoundException($"Не найден {serviceExePath}");

			// Используем sc.exe для создания сервиса
			string args = $"create \"{PipeConstants.ServiceName}\" binPath= \"{serviceExePath}\" start= auto";
			ProcessStartInfo psi = new ProcessStartInfo("sc", args)
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};
			using var process = Process.Start(psi);
			process.WaitForExit();
			if (process.ExitCode != 0)
			{
				string error = process.StandardError.ReadToEnd();
				throw new Exception($"Ошибка установки сервиса: {error}");
			}

			// Добавляем описание
			psi = new ProcessStartInfo("sc", $"description \"{PipeConstants.ServiceName}")
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true
			};
			using var procDesc = Process.Start(psi);
			procDesc.WaitForExit();
		}
	}

	// Кэш доступности для клиента
	public static class ServiceAvailabilityCache
	{
		private static bool _available = false;
		private static DateTime _lastCheck = DateTime.MinValue;
		private static readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(10);

		public static bool IsAvailable()
		{
			if (DateTime.UtcNow - _lastCheck < _cacheDuration)
				return _available;

			// Проверяем через попытку подключения к каналу с таймаутом
			try
			{
				using var client = new NamedPipeClientStream(".", PipeConstants.PipeName, PipeDirection.InOut);
				client.Connect(200);
				_available = true;
			}
			catch
			{
				_available = false;
			}
			_lastCheck = DateTime.UtcNow;
			return _available;
		}

		public static void SetUnavailable()
		{
			_available = false;
			_lastCheck = DateTime.UtcNow;
		}
	}
}
