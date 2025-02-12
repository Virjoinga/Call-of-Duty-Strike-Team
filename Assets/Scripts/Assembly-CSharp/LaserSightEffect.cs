using System.Collections;
using UnityEngine;

public class LaserSightEffect : MonoBehaviour
{
	public float MaximumLength = 10f;

	public float scrollSpeed = 0.3f;

	public float pulseSpeed = 1f;

	public float noiseSize = 0.5f;

	public float maxWidth = 0.3f;

	public float minWidth = 0.66f;

	private LineRenderer m_LineRenderer;

	private float aniDir = 1f;

	private int mLayerMask;

	private void Start()
	{
		m_LineRenderer = GetComponent<LineRenderer>();
		mLayerMask = 1 << LayerMask.NameToLayer("Default");
		StartCoroutine(ChoseNewAnimationTargetCoroutine());
	}

	private IEnumerator ChoseNewAnimationTargetCoroutine()
	{
		while (true)
		{
			aniDir = aniDir * 0.9f + Random.Range(0.5f, 1.5f) * 0.1f;
			yield return null;
			minWidth = minWidth * 0.8f + Random.Range(0.1f, 1f) * 0.2f;
			yield return new WaitForSeconds(1f + Random.value * 2f - 1f);
		}
	}

	private void Update()
	{
		RaycastHit hitInfo;
		float z = ((!Physics.Raycast(base.transform.position, base.transform.forward, out hitInfo, MaximumLength, mLayerMask)) ? MaximumLength : hitInfo.distance);
		m_LineRenderer.SetPosition(1, new Vector3(0f, 0f, z));
		base.GetComponent<Renderer>().material.mainTextureOffset += new Vector2(Time.deltaTime * aniDir * scrollSpeed, 0f);
		base.GetComponent<Renderer>().material.SetTextureOffset("_NoiseTex", new Vector2((0f - Time.time) * aniDir * scrollSpeed, 0f));
		float b = Mathf.PingPong(Time.time * pulseSpeed, 1f);
		b = Mathf.Max(minWidth, b) * maxWidth;
		m_LineRenderer.SetWidth(b, b);
	}
}
