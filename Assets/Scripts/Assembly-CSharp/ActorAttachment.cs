using System.Collections.Generic;
using UnityEngine;

public class ActorAttachment : MonoBehaviour
{
	public Transform AttachPoint;

	public GameObject Model;

	public HitBoxDescriptor HitBoxRig;

	private bool mDropped;

	public void Start()
	{
		UpdateAttachment();
	}

	public void UpdateAttachment()
	{
		if (AttachPoint != null)
		{
			base.transform.ParentAndZeroLocalPositionAndRotation(AttachPoint);
		}
	}

	public void Drop()
	{
		if (mDropped)
		{
			return;
		}
		AttachPoint = null;
		GameObject gameObject = SceneNanny.NewGameObject("DroppedAttachment");
		base.transform.parent = gameObject.transform;
		mDropped = true;
		if (HitBoxRig != null)
		{
			int layer = LayerMask.NameToLayer("Default");
			List<HitLocation> list = new List<HitLocation>();
			foreach (HitBoxDescriptor.HitBox hitBox in HitBoxRig.HitBoxes)
			{
				HitLocation hitLocation = HitBoxUtils.CreateHitLocation(Model, hitBox, layer);
				hitLocation.transform.parent = Model.transform;
				hitLocation.Owner = Model;
				list.Add(hitLocation);
			}
			Rigidbody rigidbody = Model.AddComponent<Rigidbody>();
			rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			foreach (HitLocation item in list)
			{
				rigidbody.mass += item.Mass;
			}
		}
		float seconds = 3f;
		if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(EffectUtils.FadeAndDestroyAfter(Model, gameObject, seconds));
		}
	}
}
