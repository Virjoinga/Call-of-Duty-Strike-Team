using UnityEngine;

public class Human_HLT : HitLocationTranslator
{
	private const float kStandingHeadZone = 0.85f;

	private HitLocation head;

	private float sourceHeight;

	public override void Initialise(HitLocation source, HitLocation[] dest, Transform matchingXZ, Transform matchingY, Collider col)
	{
		sourceLocation = source;
		matchingTransformXZ = matchingXZ;
		matchingTransformY = matchingY;
		sourceCollider = col;
		toLocationList = new HitLocation[dest.Length - 1];
		int num = 0;
		for (int i = 0; i < dest.Length; i++)
		{
			if (dest[i].Location == "Head")
			{
				head = dest[i];
			}
			else
			{
				toLocationList[num++] = dest[i];
			}
		}
	}

	public override void DelegateImpact(ref SurfaceImpact impact, Actor shooter)
	{
		if (!head.Actor.realCharacter.IsFirstPerson)
		{
			bool flag = false;
			if (shooter != null)
			{
				flag = shooter.behaviour.aimedShotTarget == head.Actor;
			}
			if (flag)
			{
				DelegateTo(ref impact, head);
			}
			else
			{
				base.DelegateImpact(ref impact, shooter);
			}
		}
	}

	private bool RollForHeadShot()
	{
		return Random.Range(1, 3) == 1;
	}
}
