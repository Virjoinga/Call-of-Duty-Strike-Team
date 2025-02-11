using System;
using UnityEngine;

public class BlobShadow : MonoBehaviour
{
	private enum State
	{
		Normal = 0,
		FadeOutAndDestroy = 1,
		FadeOut = 2,
		Hidden = 3,
		FadeIn = 4
	}

	public Transform ShadowCasterRoot;

	public Transform ShadowCaster;

	public GameObject ActorBase;

	public bool SnapToGround;

	public float GroundOffset = 0.01f;

	public Color ShadowColour = Color.black;

	private float mFadeAlpha = 1f;

	private State mShadowState;

	private void Awake()
	{
		if (OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.DetailedShadows))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (!(base.renderer == null))
		{
			LODGroup lODGroup = base.gameObject.AddComponent<LODGroup>();
			LOD[] array = new LOD[1];
			Renderer[] renderers = new Renderer[1] { base.renderer };
			array[0] = new LOD(0.01f, renderers);
			lODGroup.SetLODS(array);
			lODGroup.RecalculateBounds();
			Animation component = base.gameObject.GetComponent<Animation>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
		}
	}

	private void Start()
	{
		mShadowState = State.Normal;
		if (ActorBase != null)
		{
			Actor component = ActorBase.GetComponent<Actor>();
			HealthComponent healthComponent = ((!(component != null)) ? null : component.health);
			if (healthComponent != null)
			{
				healthComponent.OnHealthEmpty += OnHealthEmpty;
			}
		}
	}

	private void OnHealthEmpty(object sender, EventArgs args)
	{
		if (mShadowState != State.FadeOutAndDestroy)
		{
			if (mShadowState == State.Hidden)
			{
				DestroyMe();
			}
			else
			{
				mShadowState = State.FadeOutAndDestroy;
			}
		}
	}

	public void FadeOutAndHide()
	{
		if (mShadowState != State.FadeOut && (mShadowState == State.Normal || mShadowState == State.FadeIn))
		{
			mShadowState = State.FadeOut;
		}
	}

	public void UnHideAndFadeIn()
	{
		if (mShadowState != State.FadeIn && (mShadowState == State.Hidden || mShadowState == State.FadeOut))
		{
			if (mShadowState == State.Hidden)
			{
				base.renderer.enabled = true;
				mFadeAlpha = 0f;
			}
			mShadowState = State.FadeIn;
		}
	}

	private void DestroyMe()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void PositionShadow()
	{
		Vector3 position = ShadowCaster.position;
		position.y = ShadowCasterRoot.position.y + GroundOffset;
		base.gameObject.transform.position = position;
		base.gameObject.renderer.material.color = ShadowColour;
		RaycastHit hitInfo;
		if (SnapToGround && Physics.Raycast(base.gameObject.transform.position, -Vector3.up, out hitInfo, 3f, 1))
		{
			base.gameObject.transform.position = hitInfo.point + Vector3.up * GroundOffset;
		}
	}

	private float UpdateFadeout()
	{
		mFadeAlpha -= TimeManager.DeltaTime;
		if (mFadeAlpha < 0f)
		{
			mFadeAlpha = 0f;
		}
		Color color = base.gameObject.renderer.material.color;
		color.a = mFadeAlpha;
		base.gameObject.renderer.material.color = color;
		return mFadeAlpha;
	}

	private float UpdateFadeIn()
	{
		mFadeAlpha += TimeManager.DeltaTime;
		if (mFadeAlpha > 1f)
		{
			mFadeAlpha = 1f;
		}
		Color color = base.gameObject.renderer.material.color;
		color.a = mFadeAlpha;
		base.gameObject.renderer.material.color = color;
		return mFadeAlpha;
	}

	private void LateUpdate()
	{
		if (ShadowCaster == null)
		{
			DestroyMe();
			return;
		}
		switch (mShadowState)
		{
		case State.Normal:
			PositionShadow();
			break;
		case State.FadeOut:
			PositionShadow();
			if (UpdateFadeout() <= 0f)
			{
				base.renderer.enabled = false;
				mShadowState = State.Hidden;
			}
			break;
		case State.FadeIn:
			PositionShadow();
			if (UpdateFadeIn() >= 1f)
			{
				mShadowState = State.Normal;
			}
			break;
		case State.Hidden:
			break;
		case State.FadeOutAndDestroy:
			PositionShadow();
			if (UpdateFadeout() <= 0f)
			{
				DestroyMe();
			}
			break;
		}
	}
}
