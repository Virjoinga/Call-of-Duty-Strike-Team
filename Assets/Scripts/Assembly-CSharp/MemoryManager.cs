using UnityEngine;

public class MemoryManager : SingletonMonoBehaviour
{
	protected override void AwakeOnce()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void HandleMemoryWarning()
	{
		Resources.UnloadUnusedAssets();
	}
}
