using UnityEngine;

public class RiotShield_HLT : HitLocationTranslator
{
	private const float kStandingHeadZone = 0.85f;

	private HitLocation head;

	private HitLocation shield;

	public override void Initialise(HitLocation source, HitLocation[] dest, Transform matchingXZ, Transform matchingY, Collider col)
	{
		sourceLocation = source;
		matchingTransformXZ = matchingXZ;
		matchingTransformY = matchingY;
		sourceCollider = col;
		toLocationList = new HitLocation[dest.Length - 2];
		int num = 0;
		for (int i = 0; i < dest.Length; i++)
		{
			if (dest[i].Location == "Head")
			{
				head = dest[i];
			}
			else if (dest[i].Location == "Shield")
			{
				shield = dest[i];
			}
			else
			{
				toLocationList[num++] = dest[i];
			}
		}
	}

	public override void DelegateImpact(ref SurfaceImpact impact, Actor shooter)
	{
		if (Vector3.Dot(impact.position - sourceCollider.transform.position, head.Actor.model.transform.forward) > 0f)
		{
			MakeShieldImpact(ref impact);
			return;
		}
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

	private void MakeShieldImpact(ref SurfaceImpact impact)
	{
		impact.gameobject = shield.gameObject;
		bool noDecal;
		impact.material = HitBoxUtils.GetSurfaceMaterial(impact.gameobject, out noDecal);
		impact.noDecal = noDecal;
	}

	private bool RollForHeadShot()
	{
		return Random.Range(1, 3) == 1;
	}
}
