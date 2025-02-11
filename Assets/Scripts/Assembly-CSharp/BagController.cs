using System;
using UnityEngine;

[ExecuteInEditMode]
public class BagController : MonoBehaviour
{
	[NonSerialized]
	public bool m_RefreshLightmaps = true;

	public bool m_CaptureAllThumbnails;

	public BagManager.ThumbDir m_ThumbnailDirection;

	private void Start()
	{
	}

	private void Update()
	{
		if (m_CaptureAllThumbnails)
		{
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(BagObject));
			for (int i = 0; i < array.Length; i++)
			{
				BagObject bagObject = (BagObject)array[i];
				bagObject.MakeThumbnail();
			}
			m_CaptureAllThumbnails = false;
		}
		BagManager.Instance.m_ThumbDirection = m_ThumbnailDirection;
		if (!BagManager.Instance.IsInMission())
		{
			if (m_RefreshLightmaps)
			{
			}
			m_RefreshLightmaps = false;
		}
	}
}
