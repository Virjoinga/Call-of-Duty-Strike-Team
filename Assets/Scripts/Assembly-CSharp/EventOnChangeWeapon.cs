using UnityEngine;

public class EventOnChangeWeapon : EventDescriptor
{
	public bool CheckSecondaryWeapon;

	private Actor mActorRef;

	public override void Initialise(GameObject gameObj)
	{
		base.Initialise(gameObj);
		Actor component = gameObj.GetComponent<Actor>();
		if (component != null && component.weapon != null)
		{
			mActorRef = component;
		}
	}

	public void Update()
	{
		if (CheckSecondaryWeapon)
		{
			CheckSecondarySwitch();
		}
		else
		{
			CheckPrimarySwitch();
		}
	}

	private void CheckSecondarySwitch()
	{
		if (mActorRef != null && mActorRef.weapon.DesiredWeapon == mActorRef.weapon.SecondaryWeapon && mActorRef.weapon.ActiveWeapon != mActorRef.weapon.DesiredWeapon)
		{
			FireEvent();
		}
	}

	private void CheckPrimarySwitch()
	{
		if (mActorRef != null && mActorRef.weapon.DesiredWeapon == mActorRef.weapon.PrimaryWeapon && mActorRef.weapon.ActiveWeapon != mActorRef.weapon.DesiredWeapon)
		{
			FireEvent();
		}
	}
}
