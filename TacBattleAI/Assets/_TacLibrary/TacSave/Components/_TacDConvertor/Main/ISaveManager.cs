// Author: Sergej Jakovlev <tac1402@gmail.com>
// Copyright (C) 2026 Sergej Jakovlev

namespace Tac.DConvert
{
	public interface ISaveManager
	{
		public string Version { get; set; }
		public event Change LoadError;

		public void Save(string argDirName, string argFileName);
		public void Load(string argDirName, string argFileName);
	}
}
