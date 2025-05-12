namespace Central_Server.Data;

public class FileAccess
{
	private readonly string _dataDir;

	private readonly string _evuName;
	private readonly string _containerName;
	
	public FileAccess()
	{
#if RELEASE
		_dataDir = "/Data";
#else
		_dataDir = Path.Combine(SpecialDirectories.MyDocuments, "AutoTf/CentralServer");
#endif
		// UnknownEVU should pretty much just never happen, unless someone fucked up and misconfigured the container.
		_evuName = Environment.GetEnvironmentVariable("evuName") ?? "UnknownEVU";
		_containerName = Environment.GetEnvironmentVariable("containerName") ?? "UnknownEVU";

		if (!ReadFile("allowedTrainsCount", out _))
		{
			if (int.TryParse(Environment.GetEnvironmentVariable("allowedTrainsCount"), out int allowedTrainsInt))
			{
				WriteAllText("allowedTrainsCount", allowedTrainsInt.ToString());
			}
		}
		
		Directory.CreateDirectory(_dataDir);
	}

	public string GetEvuName()
	{
		return _evuName;
	}

	public int GetAllowedTrainsCount()
	{
		return Convert.ToInt32(ReadFile("allowedTrainsCount"));
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
	
	public string[] GetFiles(string directory)
	{
		string path = Path.Combine(_dataDir, directory);
		return Directory.GetFiles(path);
	}
	
	public string[] GetDirectories(string directory)
	{
		string path = Path.Combine(_dataDir, directory);
		return Directory.GetDirectories(path);
	}

	public bool DirectoryExists(string directory)
	{
		string path = Path.Combine(_dataDir, directory);
		return Directory.Exists(path);
	}
	
	public string ReadFile(string fileName)
	{
		string path = Path.Combine(_dataDir, fileName);
		return File.ReadAllText(path);
	}
	
	public string[] ReadAllLines(string fileName)
	{
		string path = Path.Combine(_dataDir, fileName);
		return File.ReadAllLines(path);
	}
	
	public byte[] ReadAllBytes(string fileName)
	{
		string path = Path.Combine(_dataDir, fileName);
		return File.ReadAllBytes(path);
	}

	public bool FileExists(string fileName)
	{
		string path = Path.Combine(_dataDir, fileName);
		return File.Exists(path);
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

	public void SaveVideo(string fileName, IFormFile file)
	{
		string path = Path.Combine(_dataDir, fileName);
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);

		using FileStream stream = new FileStream(path, FileMode.Create);
		
		file.CopyTo(stream);
	}
}