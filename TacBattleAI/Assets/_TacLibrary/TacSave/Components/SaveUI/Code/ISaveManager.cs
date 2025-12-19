using Tac;

public interface ISaveManager
{
	public string Version { get; set; }
	public event Change LoadError;

	public void Save(string argDirName, string argFileName);
	public void Load(string argDirName, string argFileName);
}
