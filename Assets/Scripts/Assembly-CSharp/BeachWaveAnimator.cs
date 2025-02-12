using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BeachWaveAnimator : MonoBehaviour
{
	private Material m_material;

	private Color m_initialTint;

	private float m_waveTimer;

	[SerializeField]
	private float m_waveSpeed = 0.75f;

	[SerializeField]
	private float m_minSpread = 0.4f;

	[SerializeField]
	private float m_maxSpread = 1f;

	[SerializeField]
	private float m_minOffset = -0.2f;

	[SerializeField]
	private float m_maxOffset = 0.4f;

	[SerializeField]
	private float m_minAlpha;

	[SerializeField]
	private float m_maxAlpha = 1f;

	[SerializeField]
	private float m_minColourScrollSpeed = 0.03f;

	[SerializeField]
	private float m_maxColourScrollSpeed = 0.005f;

	[SerializeField]
	private float m_minWavesScrollSpeed = -0.03f;

	[SerializeField]
	private float m_maxWavesScrollSpeed = -0.005f;

	[SerializeField]
	private float m_minTextureScrollSpeed = -0.02f;

	[SerializeField]
	private float m_maxTextureScrollSpeed = -0.05f;

	private void Awake()
	{
		m_material = base.GetComponent<Renderer>().material;
		m_initialTint = m_material.GetColor("_TintColor");
		base.enabled = false;
	}

	private void OnDestroy()
	{
		if (m_material != null)
		{
			UnityEngine.Object.Destroy(m_material);
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
		m_waveTimer = Mathf.Repeat(m_waveTimer + deltaTime * m_waveSpeed, (float)Math.PI * 2f);
		float t = Mathf.Sin(m_waveTimer) * 0.5f + 0.5f;
		Vector2 mainTextureScale = m_material.mainTextureScale;
		mainTextureScale.y = Mathf.Lerp(m_minSpread, m_maxSpread, t);
		m_material.mainTextureScale = mainTextureScale;
		Color initialTint = m_initialTint;
		float num = Mathf.Lerp(m_minAlpha, m_maxAlpha, t);
		initialTint.a = Mathf.Clamp01(initialTint.a - (1f - num));
		m_material.SetColor("_TintColor", initialTint);
		float num2 = Mathf.Lerp(m_minColourScrollSpeed, m_maxColourScrollSpeed, t);
		Vector2 textureOffset = m_material.GetTextureOffset("_MainTex");
		textureOffset.x = Mathf.Repeat(textureOffset.x + deltaTime * num2, 1f);
		textureOffset.y = Mathf.Lerp(m_minOffset, m_maxOffset, t);
		m_material.SetTextureOffset("_MainTex", textureOffset);
		float num3 = Mathf.Lerp(m_minWavesScrollSpeed, m_maxWavesScrollSpeed, t);
		textureOffset = m_material.GetTextureOffset("_Waves");
		textureOffset.x = Mathf.Repeat(textureOffset.x + deltaTime * num3, 1f);
		m_material.SetTextureOffset("_Waves", textureOffset);
		float num4 = Mathf.Lerp(m_minTextureScrollSpeed, m_maxTextureScrollSpeed, t);
		textureOffset = m_material.GetTextureOffset("_Texture");
		textureOffset.y = Mathf.Repeat(textureOffset.y + deltaTime * num4, 1f);
		m_material.SetTextureOffset("_Texture", textureOffset);
	}
}
