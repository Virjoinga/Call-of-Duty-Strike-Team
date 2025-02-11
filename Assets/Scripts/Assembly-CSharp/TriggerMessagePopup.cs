using System.Collections;
using UnityEngine;

public class TriggerMessagePopup : MonoBehaviour
{
	public string Title;

	public string Message;

	public MessageBox MessageBoxPrefab;

	public bool TriggerOnlyOnce = true;

	public Vector2 MinSize = new Vector2(5f, 5f);

	public bool AutoSize = true;

	private bool mHasTriggered;

	public bool CanDisplay
	{
		get
		{
			if (mHasTriggered && TriggerOnlyOnce)
			{
				return false;
			}
			return true;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		BehaviourController component = other.gameObject.GetComponent<BehaviourController>();
		if (component != null && component.PlayerControlled && (!TriggerOnlyOnce || !mHasTriggered))
		{
			mHasTriggered = true;
			bool flag = false;
			if ((bool)GameController.Instance && GameController.Instance.mFirstPersonActor != null)
			{
				flag = true;
			}
			if (!flag)
			{
				StartCoroutine(PanAndShowMsgBox(other.gameObject));
			}
		}
	}

	private IEnumerator PanAndShowMsgBox(GameObject go)
	{
		InputManager.Instance.SetForMessageBox();
		TimeManager.instance.StopTime();
		CameraController cc = CameraManager.Instance.PlayCameraController;
		PlayCameraInterface cfd = cc.CurrentCameraBase as PlayCameraInterface;
		if (cfd != null)
		{
			cfd.FocusOnTarget(go.transform, true);
			while (cfd.PanOffset.sqrMagnitude > 1.3f)
			{
				yield return new WaitForEndOfFrame();
			}
			yield return StartCoroutine(GameController.Instance.DisplayInGameMessageBoxHelper(MessageBoxPrefab, Title, Message, MinSize, AutoSize));
		}
		else
		{
			yield return StartCoroutine(GameController.Instance.DisplayInGameMessageBoxHelper(MessageBoxPrefab, Title, Message, MinSize, AutoSize));
		}
	}
}
