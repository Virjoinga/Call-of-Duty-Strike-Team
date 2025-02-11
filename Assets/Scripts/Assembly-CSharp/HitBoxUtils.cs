using System;
using UnityEngine;

public class HitBoxUtils
{
	public static HitLocation CreateHitLocation(GameObject model, HitBoxDescriptor.HitBox hitBox)
	{
		return CreateHitLocation(model, hitBox, LayerMask.NameToLayer("HitBox"));
	}

	public static HitLocation CreateHitLocation(GameObject model, HitBoxDescriptor.HitBox hitBox, int layer)
	{
		Transform transform = ((hitBox.Bone.Length != 0) ? model.transform.Find(hitBox.Bone) : model.transform);
		if (transform == null)
		{
			throw new Exception(string.Format("Could not find bone in model when setting up hit boxes: {0}", hitBox.Bone));
		}
		Transform transform2 = new GameObject(hitBox.Location).transform;
		transform2.position = transform.position;
		transform2.rotation = transform.rotation;
		transform2.gameObject.layer = layer;
		HitLocation hitLocation = transform2.gameObject.AddComponent<HitLocation>();
		hitLocation.Location = hitBox.Location;
		hitLocation.Bone = transform;
		hitLocation.DamageMultiplier = hitBox.DamageMultiplier;
		hitLocation.SiblingForceFraction = hitBox.SiblingForceFraction;
		switch (hitBox.Type)
		{
		case HitBoxDescriptor.HitBoxType.Box:
		{
			BoxCollider boxCollider = transform2.gameObject.AddComponent<BoxCollider>();
			boxCollider.center = hitBox.Center;
			boxCollider.size = hitBox.Size;
			boxCollider.enabled = true;
			break;
		}
		case HitBoxDescriptor.HitBoxType.Capsule:
		{
			CapsuleCollider capsuleCollider = transform2.gameObject.AddComponent<CapsuleCollider>();
			capsuleCollider.center = hitBox.Center;
			capsuleCollider.radius = hitBox.Radius;
			capsuleCollider.height = hitBox.Height;
			capsuleCollider.direction = (int)hitBox.Direction;
			capsuleCollider.enabled = true;
			break;
		}
		case HitBoxDescriptor.HitBoxType.Sphere:
		{
			SphereCollider sphereCollider = transform2.gameObject.AddComponent<SphereCollider>();
			sphereCollider.center = hitBox.Center;
			sphereCollider.radius = hitBox.Radius;
			sphereCollider.enabled = true;
			break;
		}
		}
		DefaultSurfaceMarkup.CreateComponent(transform2.gameObject, hitBox.DefaultSurfaceMaterial, hitBox.SurfaceMarkup);
		hitLocation.Mass = hitBox.Mass;
		hitLocation.LocationName = hitBox.Location;
		return hitLocation;
	}

	public static void DelegateImpact(ref SurfaceImpact impact, Actor shooter)
	{
		if (!(impact.gameobject == null) && impact.material == SurfaceMaterial.None)
		{
			HitLocationTranslator component = impact.gameobject.GetComponent<HitLocationTranslator>();
			if (component != null)
			{
				component.DelegateImpact(ref impact, shooter);
			}
		}
	}

	public static SurfaceMaterial GetSurfaceMaterial(GameObject go, out bool noDecal)
	{
		DefaultSurfaceMarkup component = go.GetComponent<DefaultSurfaceMarkup>();
		if (component != null)
		{
			if (component.Markup != null)
			{
				noDecal = component.Markup.NoDecals;
			}
			else
			{
				noDecal = false;
			}
			return component.Material;
		}
		noDecal = false;
		return SurfaceMaterial.Default;
	}

	public static void SetSurfaceMaterial(GameObject gameObject, SurfaceMaterial material, bool noDecal)
	{
		DefaultSurfaceMarkup component = gameObject.GetComponent<DefaultSurfaceMarkup>();
		if (component != null)
		{
			component.Material = material;
			component.Markup.NoDecals = noDecal;
		}
	}
}
