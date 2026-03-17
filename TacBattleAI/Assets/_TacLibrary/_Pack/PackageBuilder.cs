using System.IO;
using System.Collections.Generic;
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
		DeleteMeta("com.tac.tacstandart", "TacStandart");
	}

	[MenuItem("Tools/Build TacLibrary/TacSave")]
	public static void BuildTacSave()
	{
		BuildPackage("com.tac.tacsave", "TacSave");
		DeleteMeta("com.tac.tacsave", "TacSave", new List<string> { "SaveUI" });
	}

	[MenuItem("Tools/Build TacLibrary/TacWireframe")]
	public static void BuildTacWireframe()
	{
		BuildPackage("com.tac.tacwireframe", "TacWireframe");
		DeleteMeta("com.tac.tacwireframe", "TacWireframe", new List<string> { "Material", "Shader" });
	}

	[MenuItem("Tools/Build TacLibrary/TacCamera")]
	public static void BuildTacCamera()
	{
		BuildPackage("com.tac.taccamera", "TacCamera");
		DeleteMeta("com.tac.taccamera", "TacCamera", new List<string> { "Prefab", "Code" });
	}

	[MenuItem("Tools/Build TacLibrary/TacItemCreate")]
	public static void BuildTacItemCreate()
	{
		BuildPackage("com.tac.tacitemcreate", "TacItemCreate");
		DeleteMeta("com.tac.tacitemcreate", "TacItemCreate");
	}

	[MenuItem("Tools/Build TacLibrary/TacGameLogic")]
	public static void BuildTacGameLogic()
	{
		BuildPackage("com.tac.tacgamelogic", "_TacGameLogic");
		DeleteMeta("com.tac.tacgamelogic", "_TacGameLogic");
	}

	[MenuItem("Tools/Build TacLibrary/TacUI")]
	public static void BuildTacUI()
	{
		BuildPackage("com.tac.tacui", "TacUI");
	}


	[MenuItem("Tools/Build TacLibrary/TacAgent")]
	public static void BuildTacAgent()
	{
		BuildPackage("com.tac.tacagent", "TacAgent");
		DeleteMeta("com.tac.tacagent", "TacAgent", new List<string> { "UI" });
	}

	[MenuItem("Tools/Build TacLibrary/TacPerson")]
	public static void BuildTacPerson()
	{
		BuildPackage("com.tac.tacperson", "TacPerson");
		DeleteMeta("com.tac.tacperson", "TacPerson", new List<string> { "Resources" });
	}

	[MenuItem("Tools/Build TacLibrary/TacItemMove")]
	public static void BuildTacItemMove()
	{
		BuildPackage("com.tac.tacitemmove", "TacItemMove");
		DeleteMeta("com.tac.tacitemmove", "TacItemMove", new List<string> { "Grid" });
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

		// Можно автоматически увеличить версию в package.json (опционально)
		// UpdateVersion(targetPath);

		Debug.Log($"Package built successfully at: {targetPath}");
		EditorUtility.RevealInFinder(targetPath); // открываем папку в проводнике
	}

	private static void DeleteMeta(string argPackageName, string argDirName, List<string> exceptDir = null)
	{
		string locTargetFolder = TargetRoot + argDirName;
		// Определяем целевую папку для сборки
		string projectPath = Path.GetDirectoryName(Application.dataPath); // корень проекта
		string targetPath = Path.Combine(projectPath, locTargetFolder, argPackageName);

		DeleteMeta(targetPath, exceptDir);
	}

	private static void DeleteMeta(string targetPath, List<string> exceptDir = null)
	{
		DirectoryInfo target = new DirectoryInfo(targetPath);

		// Удаляем все .meta файлы в целевой папке (они не нужны в пакете)
		foreach (string metaFile in Directory.GetFiles(targetPath, "*.meta", SearchOption.TopDirectoryOnly))
		{
			File.Delete(metaFile);
		}

		// Копируем подпапки рекурсивно
		foreach (DirectoryInfo subdir in target.GetDirectories())
		{
			if (exceptDir == null || exceptDir.Contains(subdir.Name) == false)
			{
				// Игнорируем папки, которые не нужны (например, если есть папка .git или Temp)
				if (subdir.Name.StartsWith(".")) continue;

				DeleteMeta(subdir.FullName, exceptDir);
			}
		}
	}

	private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
	{
		// Копируем файлы
		foreach (FileInfo file in source.GetFiles())
		{
			file.CopyTo(Path.Combine(target.FullName, file.Name));
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
