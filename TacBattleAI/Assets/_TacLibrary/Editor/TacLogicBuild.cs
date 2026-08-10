using System.IO;
using System.Text;
using System.Collections.Generic;
using TacCompiler;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

[InitializeOnLoad]
public static class TacLogicBuild
{
	[MenuItem("TacLogic/Compile")]
	public static void Compile()
	{
		List<string> source = new List<string>();
		List<string> backup = new List<string>();

		source.Add("Assets/_RotarkCode");
		backup.Add("TacBackup/_RotarkCode");

		source.Add("Assets/_TacLibrary");
		backup.Add("TacBackup/_TacLibrary");

		for (int i = 0; i < source.Count; i++)
		{
			BackupDirectory(source[i], backup[i]);

			Compiler compiler = new Compiler();

			string[] files = Directory.GetFiles(source[i], "*.cs", SearchOption.AllDirectories);
			foreach (string file in files)
			{
				string sourceCode = File.ReadAllText(file, Encoding.GetEncoding("windows-1251"));

				string newSource = compiler.PreProcessing(sourceCode);

				if (newSource != sourceCode)
				{
					File.WriteAllText(file, newSource, Encoding.UTF8);
				}
			}
		}

		AssetDatabase.Refresh();
		CompilationPipeline.RequestScriptCompilation();
	}

	public static void BackupDirectory(string source, string backup)
	{
		if (Directory.Exists(backup))
			Directory.Delete(backup, true);
		CopyDirectory(source, backup);
	}


	public static void RestoreDirectory(string backup, string destination)
	{
		CopyDirectory(backup, destination);
	}

	private static void CopyDirectory(string source, string destination)
	{
		Directory.CreateDirectory(destination);

		foreach (string file in Directory.GetFiles(source))
		{
			string target = Path.Combine(destination, Path.GetFileName(file));
			File.Copy(file, target, true);
		}

		foreach (string dir in Directory.GetDirectories(source))
		{
			string target = Path.Combine(destination, Path.GetFileName(dir));
			CopyDirectory(dir, target);
		}
	}

}