using Microsoft.VisualBasic.FileIO;

namespace Central_Server;

public class FileAccess
{
	private readonly string _dataDir;
	public FileAccess()
	{
#if RELEASE
		_dataDir = "/Data";
#else
		_dataDir = Path.Combine(SpecialDirectories.MyDocuments, "AutoTf/CentralServer");
		Directory.CreateDirectory(_dataDir);
#endif
	}

	public bool ReadFile(string fileName, out string content)
	{
		content = "";
		string path = Path.Combine(_dataDir, fileName);
		if (!File.Exists(path))
			return false;

		content = File.ReadAllText(path);
		return true;
	}

	public void WriteAllText(string fileName, string content)
	{
		string path = Path.Combine(_dataDir, fileName);
		File.WriteAllText(path, content);
	}
}