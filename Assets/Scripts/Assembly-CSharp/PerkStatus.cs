public class PerkStatus : ISaveLoadNamed
{
	private const string PerkStatusProXP = "ProXP";

	public int ProXP { get; set; }

	public bool ReachedProThisMission { get; set; }

	public PerkStatus()
	{
		Reset();
	}

	public void Reset()
	{
		ProXP = 0;
		ReachedProThisMission = false;
	}

	public void Save(string objectName)
	{
		SecureStorage.Instance.SetInt(objectName + "ProXP", ProXP);
	}

	public void Load(string objectName)
	{
		ProXP = SecureStorage.Instance.GetInt(objectName + "ProXP", 0);
	}
}
