using System;

[Flags]
public enum ContextMenuOptionType
{
	Undefined = 0,
	Open = 1,
	Close = 2,
	BreachByForce = 4,
	BreachByExplosive = 8,
	Lock = 0x10,
	Unlock = 0x20,
	Enter = 0x40,
	HideBody = 0x80,
	MeleeAttack = 0x100,
	TakeCover = 0x200,
	Hack = 0x400,
	PlaceMine = 0x800,
	Follow = 0x1000,
	Defend = 0x2000,
	FirstPerson = 0x4000,
	Halt = 0x8000,
	Peep = 0x10000,
	Exit = 0x20000,
	Shoot = 0x40000,
	Suppress = 0x80000,
	ThrowGrenade = 0x100000,
	Carry = 0x200000,
	Drop = 0x400000,
	Diffuse = 0x800000,
	PickUp = 0x1000000,
	SetUpExplosives = 0x2000000,
	HackSentryGun = 0x4000000,
	UseFixedGun = 0x8000000,
	FlagsMax = 0x1C,
	Everything = 0xFFFFFFF
}
