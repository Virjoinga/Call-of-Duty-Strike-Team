using UnityEngine;

public class LoadNextSection : MonoBehaviour
{
	public string SectionName;

	private void OnTriggerEnter(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (component != null && component.behaviour.PlayerControlled)
		{
			base.enabled = false;
			Object.DontDestroyOnLoad(CameraManager.Instance.gameObject);
			Object.DontDestroyOnLoad(GameController.Instance.gameObject);
			Object.DontDestroyOnLoad(GUISystem.Instance.gameObject);
			Object.DontDestroyOnLoad(SceneNanny.smInstance.gameObject);
			Object.DontDestroyOnLoad(TargetWrapperManager.Instance().gameObject);
			Application.LoadLevelAsync(SectionName);
		}
	}
}
