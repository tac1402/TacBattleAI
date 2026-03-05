using System.IO;
using UnityEditor;
using UnityEngine;

public class PackageBuilder
{

	// Настройки
	private static string SourceFolder = "Assets/_TacLibrary/"; // папка с исходниками пакета
	private static string TargetRoot = "../Install/TacLibrary/"; // папка для готовых пакетов (относительно корня проекта)

	[MenuItem("Tools/Build TacLibrary/TacStandart")]
	public static void BuildTacStandart()
	{
		BuildPackage("com.tac.tacstandart", "TacStandart");	
	}

	[MenuItem("Tools/Build TacLibrary/TacSave")]
	public static void BuildTacSave()
	{
		BuildPackage("com.tac.tacsave", "TacSave");
	}

	public static void BuildPackage(string argPackageName, string argDirName)
	{
		string locSourceFolder = SourceFolder + argDirName;
		string locTargetFolder = TargetRoot + argDirName;

		// Определяем путь к исходной папке (полный)
		string sourcePath = Path.Combine(Application.dataPath, locSourceFolder.Replace("Assets/", ""));
		if (!Directory.Exists(sourcePath))
		{
			Debug.LogError($"Source folder not found: {sourcePath}");
			return;
		}

		// Определяем целевую папку для сборки
		string projectPath = Path.GetDirectoryName(Application.dataPath); // корень проекта
		string targetPath = Path.Combine(projectPath, locTargetFolder, argPackageName);

		// Создаём целевую папку (если есть, очищаем)
		if (Directory.Exists(targetPath))
		{
			Directory.Delete(targetPath, true);
		}
		Directory.CreateDirectory(targetPath);

		// Копируем все файлы из исходной папки
		CopyAll(new DirectoryInfo(sourcePath), new DirectoryInfo(targetPath));

		// Удаляем все .meta файлы в целевой папке (они не нужны в пакете)
		foreach (string metaFile in Directory.GetFiles(targetPath, "*.meta", SearchOption.AllDirectories))
		{
			File.Delete(metaFile);
		}

		// Можно автоматически увеличить версию в package.json (опционально)
		// UpdateVersion(targetPath);

		Debug.Log($"Package built successfully at: {targetPath}");
		EditorUtility.RevealInFinder(targetPath); // открываем папку в проводнике
	}

	private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
	{
		// Копируем файлы
		foreach (FileInfo file in source.GetFiles())
		{
			// Игнорируем .meta файлы сразу (можно не копировать)
			if (file.Extension != ".meta")
			{
				file.CopyTo(Path.Combine(target.FullName, file.Name));
			}
		}

		// Копируем подпапки рекурсивно
		foreach (DirectoryInfo subdir in source.GetDirectories())
		{
			// Игнорируем папки, которые не нужны (например, если есть папка .git или Temp)
			if (subdir.Name.StartsWith(".")) continue;

			DirectoryInfo nextTarget = target.CreateSubdirectory(subdir.Name);
			CopyAll(subdir, nextTarget);
		}
	}

	// Опционально: обновить версию в package.json
	private void UpdateVersion(string packagePath)
	{
		string jsonPath = Path.Combine(packagePath, "package.json");
		if (!File.Exists(jsonPath)) return;

		string json = File.ReadAllText(jsonPath);
		// Здесь можно парсить JSON и увеличивать patch-версию
		// Простейший вариант: заменить "version": "x.y.z" на новую
		// Но лучше использовать сериализацию
		// Для простоты не реализую
	}

}
