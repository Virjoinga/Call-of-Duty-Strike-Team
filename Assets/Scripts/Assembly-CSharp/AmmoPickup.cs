using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
	public bool AmmoActive = true;

	public void Init(Vector3 pos)
	{
		base.gameObject.SetActive(true);
		base.transform.position = pos;
		base.gameObject.MoveBy(new Vector3(0f, 0.3f, 0f), 1f, 0f, EaseType.easeInOutSine, LoopType.pingPong);
		AmmoActive = true;
	}

	public void Reset()
	{
		iTween.Stop(base.gameObject);
		base.gameObject.SetActive(false);
		AmmoActive = false;
	}

	private void Update()
	{
		if (CameraManager.Instance != null && CameraManager.Instance.CurrentCamera != null)
		{
			Camera currentCamera = CameraManager.Instance.CurrentCamera;
			base.transform.forward = currentCamera.transform.forward * -1f;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (component != null && component.behaviour.PlayerControlled)
		{
			AmmoDropManager.Instance.PickUpAmmo(component, this);
		}
	}
}
