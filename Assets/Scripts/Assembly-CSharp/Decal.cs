using UnityEngine;

public class Decal : MonoBehaviour
{
	public DecalManager.DecalType mDecalType;

	public float mLifeTime;

	private float mFadeTime;

	private Renderer mRendRef;

	private MeshFilter mMeshFilterRef;

	private Color mColour = Color.white;

	private float mOneOverFadeTimeOrig;

	public bool IsActive { get; set; }

	public bool IsFadingOut
	{
		get
		{
			return IsActive && mLifeTime <= 0f && mFadeTime > 0f;
		}
	}

	public void SetMesh(Mesh mesh)
	{
		if (mMeshFilterRef != null)
		{
			mMeshFilterRef.sharedMesh = mesh;
		}
	}

	private void Awake()
	{
		Renderer componentInChildren = GetComponentInChildren<Renderer>();
		if (componentInChildren != null && componentInChildren.material != null)
		{
			mRendRef = componentInChildren;
			mRendRef.enabled = false;
			mColour = mRendRef.material.color;
		}
		mMeshFilterRef = GetComponent<MeshFilter>();
	}

	public void Setup(DecalManager.DecalType type, float lifeTime, float fadeTime)
	{
		mDecalType = type;
		mLifeTime = lifeTime;
		mFadeTime = fadeTime;
		if (fadeTime > 0f)
		{
			mOneOverFadeTimeOrig = 1f / fadeTime;
		}
		else
		{
			mOneOverFadeTimeOrig = 0f;
		}
		IsActive = true;
		if ((bool)mRendRef)
		{
			mColour.a = 1f;
			mRendRef.material.color = mColour;
			mRendRef.enabled = true;
		}
	}

	public void DeactivateNow()
	{
		IsActive = false;
		if (mLifeTime > 0f && (bool)mRendRef)
		{
			mColour.a = 0f;
			mRendRef.material.color = mColour;
			mRendRef.enabled = false;
		}
	}

	public void ManagerUpdate()
	{
		if (mLifeTime > 0f)
		{
			mLifeTime -= TimeManager.DeltaTime;
		}
		else if (mFadeTime > 0f)
		{
			mFadeTime -= TimeManager.DeltaTime;
			if ((bool)mRendRef)
			{
				mColour.a = mFadeTime * mOneOverFadeTimeOrig;
				mRendRef.material.color = mColour;
			}
		}
		else
		{
			IsActive = false;
		}
	}
}
