using System.Collections;

public class ToggleUnlimitedAmmoCommand : Command
{
	public bool Unlimited = true;

	public bool ApplyToSecondary;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		ActorIdentIterator aii = new ActorIdentIterator(GKM.PlayerControlledMask);
		Actor actor;
		while (aii.NextActor(out actor))
		{
			actor.weapon.PrimaryWeapon.GetWeaponAmmo().UnlimitedAmmo = Unlimited;
			if (!actor.weapon.PrimaryWeapon.GetWeaponAmmo().Available)
			{
				actor.weapon.PrimaryWeapon.GetWeaponAmmo().AddAmmo(1);
			}
			if (ApplyToSecondary)
			{
				actor.weapon.SecondaryWeapon.GetWeaponAmmo().UnlimitedAmmo = Unlimited;
				if (!actor.weapon.SecondaryWeapon.GetWeaponAmmo().Available)
				{
					actor.weapon.SecondaryWeapon.GetWeaponAmmo().AddAmmo(1);
				}
			}
		}
		if (GameController.Instance.IsFirstPerson)
		{
			RealCharacter.SetUpdateFPPHudText();
		}
		yield break;
	}
}
