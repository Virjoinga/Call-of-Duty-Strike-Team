using UnityEngine;

public class DebugBlip : MonoBehaviour
{
	private Camera mGuiCamRef;

	private PackedSprite sprite;

	public Vector2 ScreenPos = Vector2.zero;

	public float decayTimer;

	private void Start()
	{
		mGuiCamRef = GUISystem.Instance.m_guiCamera;
		sprite = GetComponent<PackedSprite>();
		sprite.SetColor(Color.red);
	}

	public void Decay(float time)
	{
		decayTimer = time;
	}

	private void Update()
	{
		Vector3 vector = mGuiCamRef.ScreenToWorldPoint(ScreenPos);
		base.transform.position = new Vector3(vector.x, vector.y, -10f);
		float num = Mathf.Clamp01(decayTimer * 2f);
		sprite.SetSize(num, num);
		decayTimer -= Time.deltaTime;
		if (decayTimer <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
