using Microsoft.VisualBasic.FileIO;

namespace Central_Server;

public class FileAccess
{
	private readonly string _dataDir;

	private readonly string? _evuName;
	
	public FileAccess()
	{
#if RELEASE
		_dataDir = "/Data";
#else
		_dataDir = Path.Combine(SpecialDirectories.MyDocuments, "AutoTf/CentralServer");
		Directory.CreateDirectory(_dataDir);
#endif
		_evuName = Environment.GetEnvironmentVariable("evuName");
	}

	public string GetEvuName()
	{
		return _evuName;
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

	public void AppendAllLines(string fileName, string[] content)
	{
		string path = Path.Combine(_dataDir, fileName);
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		File.AppendAllLines(path, content);
	}
}