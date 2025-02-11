using System;
using System.Collections.Generic;
using UnityEngine;

public class VolumeGroupFader : MonoBehaviour
{
	public SoundFXData.VolumeGroup VolumeGroupToFade;

	public float TimeToFade;

	public float DesiredVolume = 1f;

	private float mGroupStartVolume;

	private float mTimePassed;

	private static List<VolumeGroupFader> mActiveVolumeGroupFaders = new List<VolumeGroupFader>();

	private bool IsFinished
	{
		get
		{
			return mTimePassed >= TimeToFade;
		}
	}

	public void Awake()
	{
	}

	public void Start()
	{
		TBFAssert.DoAssert((int)VolumeGroupToFade < Enum.GetNames(typeof(SoundFXData.VolumeGroup)).Length, "Volume group out of range.");
		RemoveAnyDuplicateFader(VolumeGroupToFade);
		mActiveVolumeGroupFaders.Add(this);
		mTimePassed = 0f;
		mGroupStartVolume = SoundManager.Instance.VolumeGroups[(int)VolumeGroupToFade].VolumeScale;
	}

	public void Activate()
	{
		base.enabled = true;
	}

	public void Update()
	{
		mTimePassed += Time.deltaTime;
		if (mTimePassed > TimeToFade)
		{
			mTimePassed = TimeToFade;
		}
		float num = ((TimeToFade != 0f) ? (mTimePassed / TimeToFade) : 1f);
		float volume = DesiredVolume + (mGroupStartVolume - DesiredVolume) * (1f - num);
		SoundManager.Instance.SetVolumeGroup(VolumeGroupToFade, volume, false);
		if (IsFinished)
		{
			DoFinished();
		}
	}

	public void OnDestroy()
	{
		RemoveFromActiveList();
	}

	private void DoFinished()
	{
		UnityEngine.Object.DestroyImmediate(this);
	}

	public static void RemoveAnyDuplicateFader(SoundFXData.VolumeGroup volumeGroupToRemove)
	{
		foreach (VolumeGroupFader mActiveVolumeGroupFader in mActiveVolumeGroupFaders)
		{
			if (mActiveVolumeGroupFader.VolumeGroupToFade == volumeGroupToRemove)
			{
				mActiveVolumeGroupFader.DoFinished();
				break;
			}
		}
	}

	private void RemoveFromActiveList()
	{
		int num = mActiveVolumeGroupFaders.IndexOf(this);
		if (num >= 0)
		{
			mActiveVolumeGroupFaders.RemoveAt(num);
		}
	}
}
