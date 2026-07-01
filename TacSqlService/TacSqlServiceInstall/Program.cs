// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2025-26 Sergej Jakovlev

using Tac.Sql;
using System.Data;


class Program
{
	static void Main(string[] args)
	{
		ServiceManager.EnsureServiceInstalledAndRunning();

		bool exists = Directory.GetFiles(@"\\.\pipe\").Any(p => p.EndsWith(PipeConstants.PipeName, StringComparison.OrdinalIgnoreCase));

		Console.WriteLine(exists.ToString());
		Console.ReadLine();
	}



}

