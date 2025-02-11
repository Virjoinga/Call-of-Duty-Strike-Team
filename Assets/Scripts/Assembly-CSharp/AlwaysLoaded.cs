using UnityEngine;

public class AlwaysLoaded : MonoBehaviour
{
	public Texture[] Texures;

	private static AlwaysLoaded smInstance;

	public static AlwaysLoaded Instance
	{
		get
		{
			return smInstance;
		}
	}

	private void Awake()
	{
		if (smInstance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		smInstance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
