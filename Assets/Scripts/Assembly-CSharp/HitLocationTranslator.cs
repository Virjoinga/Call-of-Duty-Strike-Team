using UnityEngine;

public class HitLocationTranslator : MonoBehaviour
{
	protected Transform matchingTransformXZ;

	protected Transform matchingTransformY;

	protected HitLocation sourceLocation;

	protected HitLocation[] toLocationList;

	protected Collider sourceCollider;

	public virtual void Initialise(HitLocation source, HitLocation[] dest, Transform matchingXZ, Transform matchingY, Collider col)
	{
		sourceLocation = source;
		toLocationList = dest;
		matchingTransformXZ = matchingXZ;
		matchingTransformY = matchingY;
		sourceCollider = col;
	}

	public virtual void DelegateImpact(ref SurfaceImpact impact, Actor shooter)
	{
		float num = impact.position.y - sourceCollider.transform.position.y;
		int num2 = -1;
		float num3 = float.MaxValue;
		for (int i = 0; i < toLocationList.Length; i++)
		{
			float num4 = toLocationList[i].Bone.transform.position.y - matchingTransformY.position.y;
			num4 = Mathf.Abs(num4 - num);
			if (num4 < num3)
			{
				num2 = i;
				num3 = num4;
			}
		}
		if (num2 != -1)
		{
			DelegateTo(ref impact, toLocationList[num2]);
		}
	}

	protected void DelegateTo(ref SurfaceImpact impact, HitLocation hitLoc)
	{
		impact.gameobject = hitLoc.gameObject;
		Vector3 position = matchingTransformXZ.position;
		position.y = matchingTransformY.position.y;
		position = hitLoc.Bone.transform.position - position;
		impact.position = position + sourceCollider.transform.position;
		bool noDecal;
		impact.material = HitBoxUtils.GetSurfaceMaterial(hitLoc.gameObject, out noDecal);
	}

	protected void Nullify(ref SurfaceImpact impact)
	{
		impact.gameobject = null;
		impact.material = SurfaceMaterial.None;
	}
}
