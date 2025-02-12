using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class WaveAnimator : MonoBehaviour
{
	private Material m_material;

	[SerializeField]
	private float m_colour1ScrollSpeed = 0.025f;

	[SerializeField]
	private float m_colour2ScrollSpeed = 0.025f;

	[SerializeField]
	private float m_waves1ScrollSpeed = -0.15f;

	[SerializeField]
	private float m_waves2ScrollSpeed = 0.15f;

	private void Awake()
	{
		m_material = base.GetComponent<Renderer>().material;
		base.enabled = false;
	}

	private void OnDestroy()
	{
		if (m_material != null)
		{
			Object.Destroy(m_material);
			m_material = null;
		}
	}

	private void OnBecameVisible()
	{
		base.enabled = true;
	}

	private void OnBecameInvisible()
	{
		base.enabled = false;
	}

	private void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		Vector2 textureOffset = m_material.GetTextureOffset("_MainTex1");
		textureOffset.x = Mathf.Repeat(textureOffset.x + deltaTime * m_colour1ScrollSpeed, 1f);
		m_material.SetTextureOffset("_MainTex1", textureOffset);
		textureOffset = m_material.GetTextureOffset("_MainTex2");
		textureOffset.x = Mathf.Repeat(textureOffset.x + deltaTime * m_colour2ScrollSpeed, 1f);
		m_material.SetTextureOffset("_MainTex2", textureOffset);
		textureOffset = m_material.GetTextureOffset("_Waves1");
		textureOffset.x = Mathf.Repeat(textureOffset.x + deltaTime * m_waves1ScrollSpeed, 1f);
		m_material.SetTextureOffset("_Waves1", textureOffset);
		textureOffset = m_material.GetTextureOffset("_Waves2");
		textureOffset.x = Mathf.Repeat(textureOffset.x + deltaTime * m_waves2ScrollSpeed, 1f);
		m_material.SetTextureOffset("_Waves2", textureOffset);
	}
}
