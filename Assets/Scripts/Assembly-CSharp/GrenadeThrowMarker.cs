using UnityEngine;

public class GrenadeThrowMarker : MonoBehaviour
{
	public enum IconState
	{
		Normal = 0,
		CancelThrow = 1,
		PlaceClaymore = 2,
		CancelClaymore = 3,
		DropBody = 4,
		CancelBody = 5
	}

	private const float kGrenadeRingElevation = 0.15f;

	public PackedSprite Icon;

	public PackedSprite[] Arrows;

	private float mArrowAnimSpeed = 2f;

	private float mArrowTimer;

	private float mTimeBetweenArrows = 0.3f;

	private float mArrowAlphaMuliplier = 0.5f;

	private Vector3 mLastKnownTarget;

	private GameObject mObjPlacementObj;

	private IconState mCurrentState;

	public bool positionMoved;

	private void Awake()
	{
		mObjPlacementObj = (GameObject)Object.Instantiate(Resources.Load("ObjectPlacement"));
		if (mObjPlacementObj != null)
		{
			mObjPlacementObj.transform.parent = base.transform;
			mObjPlacementObj.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0f);
		}
		mCurrentState = IconState.Normal;
	}

	private void Start()
	{
		mArrowTimer = 0f;
		SetIconState(mCurrentState);
		for (int i = 0; i < Arrows.Length; i++)
		{
			Arrows[i].Color = new Color(1f, 1f, 1f, 0f);
		}
	}

	private void LateUpdate()
	{
		mArrowTimer += TimeManager.DeltaTime * mArrowAnimSpeed;
		float num = mTimeBetweenArrows * (float)(Arrows.Length + 1);
		if (mArrowTimer >= num)
		{
			mArrowTimer -= num;
		}
		Color color = ((mCurrentState != IconState.CancelThrow && mCurrentState != IconState.CancelClaymore && mCurrentState != IconState.CancelBody) ? ColourChart.GrenadeThrow : ColourChart.GrenadeCancel);
		if (mObjPlacementObj != null)
		{
			mObjPlacementObj.GetComponent<Renderer>().material.color = color;
		}
		float num2 = 1f;
		for (int i = 0; i < Arrows.Length; i++)
		{
			num2 = Mathf.Clamp01(1f + (float)i * mTimeBetweenArrows - mArrowTimer);
			if (num2 >= 1f)
			{
				num2 = 0f;
			}
			Arrows[i].Color = color.Alpha(num2 * mArrowAlphaMuliplier);
		}
		UpdatePosition(mLastKnownTarget);
	}

	public void UpdatePosition(Vector3 target)
	{
		mLastKnownTarget = target;
		Camera currentCamera = CameraManager.Instance.CurrentCamera;
		Vector3 position = currentCamera.WorldToScreenPoint(target);
		if (position.z < 0f)
		{
			position.y = 0f - position.y;
			position.x = 0f - position.x;
			position.z = 0f - position.z;
		}
		position = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(position);
		position.z = 0f;
		base.transform.position = position;
		if (mObjPlacementObj != null)
		{
			target.y += 0.15f;
			mObjPlacementObj.transform.position = target;
		}
	}

	public void SetIconState(IconState state)
	{
		Icon.SetFrame(0, (int)state);
		mCurrentState = state;
	}
}
