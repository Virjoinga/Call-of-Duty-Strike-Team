using System.Collections.Generic;
using UnityEngine;

public class AudioZoneOverride : ContainerOverride
{
	public List<VolumeGroupFaderDetails> m_VolumeGroupFaderDetailsOverride = new List<VolumeGroupFaderDetails>();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		AudioZone audioZone = cont.FindComponentOfType(typeof(AudioZone)) as AudioZone;
		if (!(audioZone != null))
		{
			return;
		}
		audioZone.m_VolumeGroupFaderDetails.Clear();
		foreach (VolumeGroupFaderDetails item in m_VolumeGroupFaderDetailsOverride)
		{
			audioZone.m_VolumeGroupFaderDetails.Add(item);
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		AudioZone componentInChildren = gameObj.GetComponentInChildren<AudioZone>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
