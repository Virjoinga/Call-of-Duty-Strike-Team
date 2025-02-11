using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class SnapTarget : MonoBehaviour
{
	public enum LockOnType
	{
		LockOn = 0,
		DoTrace = 1,
		DontLockOn = 2
	}

	public delegate Vector3 SnapPositionDelegate();

	public delegate LockOnType LockOnDelegate();

	public GameObject LockOnDetector;

	public SnapPositionDelegate SnapPositionOverride;

	public LockOnDelegate LockOnOverride;

	private int mInstance;

	private static List<SnapTarget> mInstances = new List<SnapTarget>();

	public static ReadOnlyCollection<SnapTarget> Instances
	{
		get
		{
			return mInstances.AsReadOnly();
		}
	}

	public float LastUsedTime { get; set; }

	public float LastDamageTime { get; set; }

	public Vector3 GetSnapPosition()
	{
		return (SnapPositionOverride == null) ? base.transform.position : SnapPositionOverride();
	}

	public LockOnType GetLockOnType()
	{
		return (LockOnOverride == null) ? LockOnType.DoTrace : LockOnOverride();
	}

	private void OnEnable()
	{
		mInstance = mInstances.Count;
		mInstances.Add(this);
	}

	private void OnDisable()
	{
		mInstances.RemoveAt(mInstance);
		if (mInstances.Count > 0 && mInstance < mInstances.Count)
		{
			int index = mInstances.Count - 1;
			SnapTarget snapTarget = mInstances[index];
			mInstances.RemoveAt(index);
			mInstances.Insert(mInstance, snapTarget);
			snapTarget.mInstance = mInstance;
		}
	}
}
