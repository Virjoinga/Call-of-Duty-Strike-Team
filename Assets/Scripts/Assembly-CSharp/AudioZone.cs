using System.Collections.Generic;
using UnityEngine;

public class AudioZone : KillZone
{
	public static List<AudioZone> mLevelAudioZones = new List<AudioZone>();

	public List<VolumeGroupFaderDetails> m_VolumeGroupFaderDetails = new List<VolumeGroupFaderDetails>();

	private bool m_FadersSet;

	private bool m_ReverseFaders;

	public List<VolumeGroupFaderDetails> m_PreviousVolumeGroupFaderDetails = new List<VolumeGroupFaderDetails>();

	private bool m_HookedUpToGameController;

	private bool mCopiedFadeDetails;

	public void Awake()
	{
		mLevelAudioZones.Add(this);
	}

	public void Start()
	{
		if (!m_HookedUpToGameController)
		{
			AttemptToHookToGameController();
		}
		foreach (VolumeGroupFaderDetails volumeGroupFaderDetail in m_VolumeGroupFaderDetails)
		{
			volumeGroupFaderDetail.StartVolume = -1f;
		}
	}

	public void Destroy()
	{
		if (m_HookedUpToGameController && (bool)GameController.Instance)
		{
			GameController.Instance.OnExitFirstPerson -= OnZoomOutTriggered;
			GameController.Instance.OnSwitchToFirstPerson -= OnZoomInTriggered;
			m_HookedUpToGameController = false;
		}
	}

	private void AttemptToHookToGameController()
	{
		if ((bool)GameController.Instance)
		{
			GameController.Instance.OnExitFirstPerson += OnZoomOutTriggered;
			GameController.Instance.OnSwitchToFirstPerson += OnZoomInTriggered;
			m_HookedUpToGameController = true;
		}
	}

	public void OnZoomInTriggered()
	{
		m_FadersSet = false;
	}

	public void OnZoomOutTriggered()
	{
		if (m_FadersSet)
		{
			m_ReverseFaders = true;
		}
		mCopiedFadeDetails = false;
	}

	public override void OnTriggerEnter(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (!(component == null) && component.awareness.faction == FactionHelper.Category.Player && !Contains(component))
		{
			m_ActorsWithin.Add(component);
			AudioZone otherAudioZoneOccupied = GetOtherAudioZoneOccupied();
			if (otherAudioZoneOccupied != null)
			{
				m_PreviousVolumeGroupFaderDetails = otherAudioZoneOccupied.m_PreviousVolumeGroupFaderDetails;
				mCopiedFadeDetails = true;
			}
			else
			{
				mCopiedFadeDetails = false;
			}
		}
	}

	public override void OnTriggerExit(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (component == null || !Contains(component))
		{
			return;
		}
		m_ActorsWithin.Remove(component);
		AudioZone otherAudioZoneOccupied = GetOtherAudioZoneOccupied();
		if (otherAudioZoneOccupied != null)
		{
			if (m_PreviousVolumeGroupFaderDetails != null)
			{
				otherAudioZoneOccupied.m_PreviousVolumeGroupFaderDetails = m_PreviousVolumeGroupFaderDetails;
			}
		}
		else if (component.realCharacter != null && component.realCharacter.IsFirstPerson)
		{
			m_ReverseFaders = true;
		}
	}

	private AudioZone GetOtherAudioZoneOccupied()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			foreach (AudioZone mLevelAudioZone in mLevelAudioZones)
			{
				if (mLevelAudioZone != null && mLevelAudioZone != this && mLevelAudioZone.m_ActorsWithin.Contains(GameController.Instance.mFirstPersonActor))
				{
					return mLevelAudioZone;
				}
			}
		}
		return null;
	}

	public void Update()
	{
		if (!m_HookedUpToGameController)
		{
			AttemptToHookToGameController();
		}
		if (GameController.Instance.IsFirstPerson && !m_FadersSet && !mCopiedFadeDetails)
		{
			if (!m_ActorsWithin.Contains(GameController.Instance.mFirstPersonActor))
			{
				return;
			}
			foreach (VolumeGroupFaderDetails volumeGroupFaderDetail in m_VolumeGroupFaderDetails)
			{
				VolumeGroupFaderDetails volumeGroupFaderDetails = new VolumeGroupFaderDetails();
				volumeGroupFaderDetails.VolumeGroupToFade = volumeGroupFaderDetail.VolumeGroupToFade;
				volumeGroupFaderDetails.CurrentVolume = SoundManager.Instance.VolumeGroups[(int)volumeGroupFaderDetail.VolumeGroupToFade].VolumeScale;
				volumeGroupFaderDetails.TimeToFade = volumeGroupFaderDetail.TimeToFade;
				volumeGroupFaderDetails.StartVolume = volumeGroupFaderDetail.TargetVolume;
				if (volumeGroupFaderDetail.StartVolume == -1f)
				{
					volumeGroupFaderDetails.TargetVolume = volumeGroupFaderDetails.CurrentVolume;
					volumeGroupFaderDetail.StartVolume = volumeGroupFaderDetails.CurrentVolume;
				}
				else
				{
					volumeGroupFaderDetails.TargetVolume = volumeGroupFaderDetail.StartVolume;
				}
				m_PreviousVolumeGroupFaderDetails.Add(volumeGroupFaderDetails);
				VolumeGroupFader volumeGroupFader = base.gameObject.AddComponent<VolumeGroupFader>();
				volumeGroupFader.VolumeGroupToFade = volumeGroupFaderDetail.VolumeGroupToFade;
				volumeGroupFader.TimeToFade = volumeGroupFaderDetail.TimeToFade;
				volumeGroupFader.DesiredVolume = volumeGroupFaderDetail.TargetVolume;
			}
			m_FadersSet = true;
		}
		else
		{
			if (!m_ReverseFaders)
			{
				return;
			}
			foreach (VolumeGroupFaderDetails previousVolumeGroupFaderDetail in m_PreviousVolumeGroupFaderDetails)
			{
				VolumeGroupFader volumeGroupFader2 = base.gameObject.AddComponent<VolumeGroupFader>();
				volumeGroupFader2.VolumeGroupToFade = previousVolumeGroupFaderDetail.VolumeGroupToFade;
				volumeGroupFader2.TimeToFade = previousVolumeGroupFaderDetail.TimeToFade;
				volumeGroupFader2.DesiredVolume = previousVolumeGroupFaderDetail.TargetVolume;
			}
			m_FadersSet = false;
			m_ReverseFaders = false;
		}
	}
}
