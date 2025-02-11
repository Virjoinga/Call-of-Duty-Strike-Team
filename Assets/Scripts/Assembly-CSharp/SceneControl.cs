using UnityEngine;

public class SceneControl : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			Time.timeScale = ((Time.timeScale != 0f) ? 0f : 1f);
		}
	}
}
