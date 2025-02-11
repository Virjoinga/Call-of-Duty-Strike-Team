using UnityEngine;

public class LoadScene : MonoBehaviour
{
	public string SceneToLoad;

	public bool AsyncLoad;

	private void Start()
	{
		if (AsyncLoad)
		{
			Application.LoadLevelAsync(SceneToLoad);
		}
		else
		{
			Application.LoadLevel(SceneToLoad);
		}
	}

	private void Update()
	{
	}
}
