using UnityEngine;

public class ViewModelTest : MonoBehaviour
{
	public bool Active;

	public bool DontClearDepth;

	public string Id;

	public AnimationClip Clip;

	public float Time;

	public Vector3 LocatorPosition;

	public Quaternion LocatorRotation;

	private void Update()
	{
		if (!(ViewModelRig.Instance() == null))
		{
			if (Active)
			{
				ViewModelRig.Instance().SetOverride((Id != null && Id.Length != 0) ? Id : "Empty", Clip, Time, LocatorPosition, LocatorRotation, DontClearDepth);
			}
			else
			{
				ViewModelRig.Instance().ClearOverride();
			}
		}
	}
}
