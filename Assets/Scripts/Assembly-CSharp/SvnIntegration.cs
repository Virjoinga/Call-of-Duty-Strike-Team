public class SvnIntegration
{
	public static bool m_DontUseSVN = true;

	private static SvnIntegration m_instance;

	public static SvnIntegration Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = new SvnIntegration();
				m_instance.Initialise();
			}
			return m_instance;
		}
	}

	public bool IsFileLockedByMe(string Filename)
	{
		return true;
	}

	public bool AddFile(string Filename)
	{
		return true;
	}

	public bool AddDirectory(string Pathname)
	{
		return true;
	}

	public bool CleanUp()
	{
		return true;
	}

	public bool RemoveFile(string Filename)
	{
		return true;
	}

	public bool RevertFile(string Filename, bool Unlock)
	{
		return true;
	}

	public bool SyncFile(string Filename, bool Lock)
	{
		return true;
	}

	public bool CommitFile(string Filename, string Message, bool unlock)
	{
		return true;
	}

	public int DoProcess(string args, string fileName, string globalParams)
	{
		return 0;
	}

	public void SetUserAndPass(string userName, string password)
	{
	}

	public bool GetUserAndPass()
	{
		return true;
	}

	private void Initialise()
	{
	}
}
