public static class ActorExtensions
{
	public static Events.EventActor EventActor(this Actor actor)
	{
		if (actor == null)
		{
			return null;
		}
		Events.EventActor eventActor = new Events.EventActor();
		eventActor.Name = actor.name;
		if (actor.awareness != null)
		{
			eventActor.CharacterType = actor.awareness.ChDefCharacterType;
			eventActor.Faction = actor.awareness.faction;
			eventActor.IsInCover = actor.awareness.IsInCover();
		}
		if (actor.behaviour != null)
		{
			eventActor.PlayerControlled = actor.behaviour.PlayerControlled;
		}
		if (actor.health != null)
		{
			eventActor.HealthLow = actor.health.HealthLow;
		}
		if (actor.realCharacter != null)
		{
			eventActor.Id = actor.realCharacter.Id;
			eventActor.IsDead = actor.realCharacter.IsDead();
			eventActor.IsFirstPerson = actor.realCharacter.IsFirstPerson;
			eventActor.IsMortallyWounded = actor.realCharacter.IsMortallyWounded();
			eventActor.IsUsingFixedGun = actor.realCharacter.IsUsingFixedGun;
			eventActor.IsWindowLookout = actor.realCharacter.IsWindowLookout;
			eventActor.WasFirstPersonWhenMortallyWounded = actor.realCharacter.WasFirstPersonWhenMortallyWounded;
		}
		if (actor.weapon != null)
		{
			IWeaponStats weaponStats = WeaponUtils.GetWeaponStats(actor.weapon.ActiveWeapon);
			if (weaponStats != null)
			{
				eventActor.WeaponAccuracyStatAdjustment = weaponStats.AccuracyStatAdjustment();
			}
			eventActor.WeaponClass = actor.weapon.Class;
			eventActor.WeaponId = actor.weapon.Id;
			eventActor.WeaponSilenced = actor.weapon.Silenced;
		}
		return eventActor;
	}
}
