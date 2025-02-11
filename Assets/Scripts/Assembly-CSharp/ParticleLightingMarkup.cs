using UnityEngine;

public class ParticleLightingMarkup : MonoBehaviour
{
	[SerializeField]
	private bool m_applyLighting;

	public bool ApplyLighting
	{
		get
		{
			return m_applyLighting;
		}
	}
}
