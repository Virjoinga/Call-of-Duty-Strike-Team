using UnityEngine;

public class MouseLookAlt : MonoBehaviour
{
	[SerializeField]
	private VirtualDPad m_dpad;

	private void Update()
	{
		base.transform.Rotate(Vector3.up, m_dpad.XAxis);
		base.transform.Rotate(base.transform.right, 0f - m_dpad.YAxis);
	}
}
