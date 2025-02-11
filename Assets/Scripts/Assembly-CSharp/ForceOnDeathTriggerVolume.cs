using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceOnDeathTriggerVolume : MonoBehaviour
{
	private Vector3 DirectionOfForce = Vector3.zero;

	public float Angle;

	public float ForceAmount = 100f;

	private void OnTriggerEnter(Collider other)
	{
		RealCharacter realCharacter = other.gameObject.GetComponent<RealCharacter>();
		if (realCharacter == null)
		{
			HitLocation component = other.gameObject.GetComponent<HitLocation>();
			if (component == null)
			{
				return;
			}
			Actor actor = component.Actor;
			if (actor == null)
			{
				return;
			}
			realCharacter = actor.realCharacter;
		}
		if (!(realCharacter == null))
		{
			if (realCharacter.ForceOnDeathTriggers == null)
			{
				realCharacter.ForceOnDeathTriggers = new List<ForceOnDeathTriggerVolume>();
			}
			if (!realCharacter.ForceOnDeathTriggers.Contains(this))
			{
				realCharacter.ForceOnDeathTriggers.Add(this);
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		HitLocation component = other.gameObject.GetComponent<HitLocation>();
		if (component == null)
		{
			return;
		}
		Actor actor = component.Actor;
		if (actor == null)
		{
			return;
		}
		RealCharacter componentInChildren = actor.gameObject.GetComponentInChildren<RealCharacter>();
		if (!(componentInChildren == null))
		{
			if (componentInChildren.ForceOnDeathTriggers == null)
			{
				componentInChildren.ForceOnDeathTriggers = new List<ForceOnDeathTriggerVolume>();
			}
			if (componentInChildren.ForceOnDeathTriggers.Contains(this))
			{
				componentInChildren.ForceOnDeathTriggers.Remove(this);
			}
		}
	}

	public bool ApplyForce(HitLocation hitLocation, SurfaceImpact impact)
	{
		if (hitLocation != null && impact != null && !hitLocation.rigidbody.isKinematic)
		{
			float f = Angle * ((float)Math.PI / 180f);
			DirectionOfForce = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f);
			float num = Vector3.Dot(DirectionOfForce, impact.direction);
			if (num < -0.2f)
			{
				hitLocation.rigidbody.AddForceAtPosition(ForceAmount * DirectionOfForce, hitLocation.transform.position, ForceMode.Impulse);
				return true;
			}
		}
		return false;
	}

	public void ApplySilentKillForce(Rigidbody rigidbody, Vector3 facingDirection)
	{
		if (rigidbody != null)
		{
			StartCoroutine(ApplyForce(rigidbody, facingDirection));
		}
	}

	private IEnumerator ApplyForce(Rigidbody rigidbody, Vector3 facingDirection)
	{
		while (rigidbody.isKinematic)
		{
			yield return null;
		}
		float radians = Angle * ((float)Math.PI / 180f);
		DirectionOfForce = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians));
		float dot = Vector3.Dot(DirectionOfForce, facingDirection);
		if (dot < -0.2f)
		{
			rigidbody.AddForce(ForceAmount / 2f * DirectionOfForce, ForceMode.Impulse);
		}
	}
}
