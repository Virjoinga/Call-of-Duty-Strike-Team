using System.Collections.Generic;
using UnityEngine;

public class FixedGunOverride : ContainerOverride
{
	public bool startContextDisabled;

	public MinigunDescriptor descriptor;

	public float TakeDamageMultiplier = 1f;

	public float PitchLimit = 40f;

	public bool PitchLimitUpDownSpecified;

	public float PitchLimitUp = 40f;

	public float PitchLimitDown = 40f;

	public float YawLimit = 90f;

	public bool UseAirborneCoverWhenDocked;

	public int ModifiedDifficultySettingsWhenDocked;

	public List<GameObject> ObjectsToCallOnUse;

	public List<string> FunctionsToCallOnUse;

	public List<GameObject> ObjectsToCallOnFire;

	public List<string> FunctionsToCallOnFire;

	public List<GameObject> ObjectsToCallOnLeave;

	public List<string> FunctionsToCallOnLeave;

	public bool oneShotMessages = true;

	public ObjectMessage MessageToSendOnOverheat;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		if (startContextDisabled)
		{
			CMFixedGun cMFixedGun = cont.FindComponentOfType(typeof(CMFixedGun)) as CMFixedGun;
			if (cMFixedGun != null)
			{
				cMFixedGun.enabled = false;
			}
		}
		else
		{
			CMFixedGun cMFixedGun2 = cont.FindComponentOfType(typeof(CMFixedGun)) as CMFixedGun;
			if (cMFixedGun2 != null)
			{
				cMFixedGun2.enabled = true;
			}
		}
		FixedGun fixedGun = cont.FindComponentOfType(typeof(FixedGun)) as FixedGun;
		if (fixedGun != null)
		{
			fixedGun.TakeDamageMultiplier = TakeDamageMultiplier;
			if (PitchLimitUpDownSpecified)
			{
				fixedGun.GunPitchLimitUp = PitchLimitUp;
				fixedGun.GunPitchLimitDown = PitchLimitDown;
			}
			else
			{
				fixedGun.GunPitchLimitUp = (fixedGun.GunPitchLimitDown = PitchLimit);
			}
			fixedGun.GunYawLimit = YawLimit;
			fixedGun.ObjectsToCallOnUse = ObjectsToCallOnUse;
			fixedGun.FunctionsToCallOnUse = FunctionsToCallOnUse;
			fixedGun.ObjectsToCallOnFire = ObjectsToCallOnFire;
			fixedGun.FunctionsToCallOnFire = FunctionsToCallOnFire;
			fixedGun.ObjectsToCallOnLeave = ObjectsToCallOnLeave;
			fixedGun.FunctionsToCallOnLeave = FunctionsToCallOnLeave;
			fixedGun.MessageToSendOnOverheat = MessageToSendOnOverheat;
			fixedGun.OneShotMessages = oneShotMessages;
			fixedGun.UseAirborneCoverWhenDocked = UseAirborneCoverWhenDocked;
			fixedGun.ModifiedDifficultySettingsWhenDocked = ModifiedDifficultySettingsWhenDocked;
			if (descriptor != null)
			{
				fixedGun.Descriptor = descriptor;
			}
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		FixedGun componentInChildren = gameObj.GetComponentInChildren<FixedGun>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
