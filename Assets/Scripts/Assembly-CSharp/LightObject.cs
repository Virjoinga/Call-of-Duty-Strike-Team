using UnityEngine;

public class LightObject : MonoBehaviour
{
	public enum LightCategory
	{
		Default = 0,
		Ambient = 1,
		Electric = 2
	}

	public bool m_OnDuringDay = true;

	public bool m_OnDuringNight = true;

	public LightCategory m_Category;
}
