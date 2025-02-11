using UnityEngine;

public class CameraBase : MonoBehaviour
{
	public enum CamPreview
	{
		iPad_Landscape = 0,
		iPad_Portrait = 1,
		iPhone_Landscape = 2,
		iPhone_Portrait = 3
	}

	public CamPreview PreviewType;

	public float PreviewScale = 0.2f;

	protected float mFov = 45f;

	protected float mNearClip = 0.05f;

	public virtual float Fov
	{
		get
		{
			return mFov;
		}
		set
		{
			mFov = value;
		}
	}

	public virtual float NearClip
	{
		get
		{
			return mNearClip;
		}
		set
		{
			mNearClip = value;
		}
	}

	public virtual Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
		set
		{
			base.transform.position = value;
		}
	}

	public virtual Quaternion Rotation
	{
		get
		{
			return base.transform.rotation;
		}
		set
		{
			base.transform.rotation = value;
		}
	}

	public virtual Quaternion RotationOnlyY
	{
		get
		{
			Vector3 eulerAngles = base.transform.rotation.eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			return Quaternion.Euler(eulerAngles);
		}
	}

	public virtual void AllowInput(bool allow)
	{
	}

	public virtual void EnablePlacementInput(bool enable)
	{
	}

	public virtual Vector3 TargetPoint()
	{
		return Vector3.zero;
	}

	public virtual void OnTriggerEnter(Collider other)
	{
	}

	public virtual void OnTriggerExit(Collider other)
	{
	}

	public virtual void OnTriggerStay(Collider other)
	{
	}

	public virtual void AddShake(Vector3 origin, float radius, float duration)
	{
	}

	public virtual void AddShake(float scale, float duration)
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(Position, "camera");
		Gizmos.color = Color.white;
		Gizmos.DrawLine(Position, base.transform.position);
	}

	private void OnDrawGizmosSelected()
	{
	}
}
