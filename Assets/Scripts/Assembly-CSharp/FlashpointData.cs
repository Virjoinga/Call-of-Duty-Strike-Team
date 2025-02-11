public class FlashpointData : ISaveLoadNamed
{
	private const string ActiveKey = ".Active";

	private const string LastActiveKey = ".LastActive";

	private const string TimeToStayActiveKey = ".TimeToStayActive";

	private const string SelectedSectionKey = ".Section";

	public bool isActive;

	public int timeLastMadeActive;

	public int timeToStayActive;

	public int selectedSection = -1;

	public MissionData Mission;

	public void Save(string objectName)
	{
		SecureStorage.Instance.SetBool(objectName + ".Active", isActive);
		SecureStorage.Instance.SetInt(objectName + ".LastActive", timeLastMadeActive);
		SecureStorage.Instance.SetInt(objectName + ".TimeToStayActive", timeToStayActive);
		SecureStorage.Instance.SetInt(objectName + ".Section", timeToStayActive);
	}

	public void Load(string objectName)
	{
		isActive = SecureStorage.Instance.GetBool(objectName + ".Active");
		timeLastMadeActive = SecureStorage.Instance.GetInt(objectName + ".LastActive");
		timeToStayActive = SecureStorage.Instance.GetInt(objectName + ".TimeToStayActive");
		selectedSection = SecureStorage.Instance.GetInt(objectName + ".Section");
	}

	public void Reset()
	{
		isActive = false;
		timeLastMadeActive = 0;
		timeToStayActive = 0;
		selectedSection = -1;
	}
}
