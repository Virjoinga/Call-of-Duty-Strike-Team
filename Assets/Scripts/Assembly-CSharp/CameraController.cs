using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
	public CameraBase StartCamera;

	private static CameraBase mCurrentCamera;

	private Queue<CameraTransitionData> mTransitionData = new Queue<CameraTransitionData>();

	public TweenFunctions.TweenType DefaultTransitionType = TweenFunctions.TweenType.easeInOutCubic;

	public float DefaultTransitionTime = 0.5f;

	public CameraBase CurrentCameraBase
	{
		get
		{
			if (mCurrentCamera != null)
			{
				return mCurrentCamera;
			}
			return StartCamera;
		}
		set
		{
			mCurrentCamera = value;
		}
	}

	private void Awake()
	{
		mCurrentCamera = StartCamera;
		base.GetComponent<Camera>().rect = new Rect(0f, 0f, 1f, 1f);
	}

	public void UpdateFromSingleSource(CameraBase cam)
	{
		if ((bool)cam)
		{
			base.GetComponent<Camera>().transform.position = cam.Position;
			base.GetComponent<Camera>().transform.rotation = cam.Rotation;
			base.GetComponent<Camera>().fieldOfView = cam.Fov;
			base.GetComponent<Camera>().nearClipPlane = cam.NearClip;
		}
	}

	public bool IsTransitioning()
	{
		return mTransitionData.Count > 0;
	}

	public bool IsTransitioningTo(CameraBase cam)
	{
		return mTransitionData.Count > 0 && mTransitionData.Peek().CameraTo == cam;
	}

	private void EndTransition(CameraTransitionData trans)
	{
		mCurrentCamera = trans.CameraTo;
		trans.Reset();
		CameraTransitionData cameraTransitionData = mTransitionData.Dequeue();
		TBFAssert.DoAssert(trans == cameraTransitionData, "cam trans missmatch?");
	}

	private void LateUpdate()
	{
		if (mTransitionData.Count > 0)
		{
			CameraTransitionData cameraTransitionData = mTransitionData.Peek();
			if (mCurrentCamera == null || cameraTransitionData.Duration <= 0f)
			{
				EndTransition(cameraTransitionData);
				UpdateFromSingleSource(mCurrentCamera);
				return;
			}
			cameraTransitionData.UpdateProgress();
			if (cameraTransitionData.CameraTo != null)
			{
				base.GetComponent<Camera>().transform.position = Vector3.Slerp(mCurrentCamera.Position, cameraTransitionData.CameraTo.Position, cameraTransitionData.Progress);
				base.GetComponent<Camera>().transform.rotation = Quaternion.Slerp(mCurrentCamera.Rotation, cameraTransitionData.CameraTo.Rotation, cameraTransitionData.Progress);
				base.GetComponent<Camera>().fieldOfView = Mathf.Lerp(mCurrentCamera.Fov, cameraTransitionData.CameraTo.Fov, cameraTransitionData.Progress);
				base.GetComponent<Camera>().nearClipPlane = Mathf.Lerp(mCurrentCamera.NearClip, cameraTransitionData.CameraTo.NearClip, cameraTransitionData.Progress);
			}
			if (cameraTransitionData.Progress >= 1f)
			{
				EndTransition(cameraTransitionData);
			}
		}
		else
		{
			UpdateFromSingleSource(mCurrentCamera);
		}
	}

	public void ForcedBlendTo(CameraBase cam, float duration)
	{
		mTransitionData.Clear();
		BlendTo(new CameraTransitionData(cam, TweenFunctions.TweenType.easeInOutSine, duration));
	}

	public void ForcedCutTo(CameraBase cam)
	{
		mTransitionData.Clear();
		CutTo(cam);
	}

	public void CutTo(CameraBase cam)
	{
		mTransitionData.Enqueue(new CameraTransitionData(cam));
	}

	public void BlendTo(CameraTransitionData ctd)
	{
		mTransitionData.Enqueue(ctd);
	}

	public void RestoreCameraToDefault()
	{
		RestoreCameraToDefault(0.3f);
	}

	public void RestoreCameraToDefault(float time)
	{
		InterfaceSFX.Instance.ViewChangeStatic.Play2D();
		BlendTo(new CameraTransitionData(StartCamera, TweenFunctions.TweenType.easeInOutCubic, time));
	}

	public void OnTriggerEnter(Collider other)
	{
		mCurrentCamera.OnTriggerEnter(other);
		if (mTransitionData.Count > 0)
		{
			mTransitionData.Peek().CameraTo.OnTriggerEnter(other);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		mCurrentCamera.OnTriggerExit(other);
		if (mTransitionData.Count > 0)
		{
			mTransitionData.Peek().CameraTo.OnTriggerExit(other);
		}
	}

	public void OnTriggerStay(Collider other)
	{
		mCurrentCamera.OnTriggerStay(other);
		if (mTransitionData.Count > 0)
		{
			mTransitionData.Peek().CameraTo.OnTriggerStay(other);
		}
	}

	private void OnDrawGizmosSelected()
	{
	}
}
