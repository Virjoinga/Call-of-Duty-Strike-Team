using System.Collections;
using UnityEngine;

public class SharpShooterTest : MonoBehaviour
{
	private const int TOTAL_WEAPONS = 7;

	public GameObject Actor;

	public float TimeBetweenWeapons = 30f;

	public WeaponDescriptor WeaponA;

	public WeaponDescriptor WeaponB;

	public WeaponDescriptor WeaponC;

	public WeaponDescriptor WeaponD;

	public WeaponDescriptor WeaponE;

	public WeaponDescriptor WeaponF;

	public WeaponDescriptor WeaponG;

	private WeaponDescriptor mWeaponInstanceA;

	private WeaponDescriptor mWeaponInstanceB;

	private WeaponDescriptor mWeaponInstanceC;

	private WeaponDescriptor mWeaponInstanceD;

	private WeaponDescriptor mWeaponInstanceE;

	private WeaponDescriptor mWeaponInstanceF;

	private WeaponDescriptor mWeaponInstanceG;

	private IWeapon[] mWeapon = new IWeapon[7];

	private int currentWeapon;

	private bool mWeaponSwitchOnTimer;

	private float mTimeToNextSwitch;

	private void Start()
	{
		mWeaponSwitchOnTimer = TimeBetweenWeapons != 0f;
		mTimeToNextSwitch = TimeBetweenWeapons;
		ActorWrapper componentInChildren = Actor.GetComponentInChildren<ActorWrapper>();
		if (!(componentInChildren == null))
		{
			Actor actor = componentInChildren.GetActor();
			if (!(actor == null))
			{
				mWeaponInstanceA = Object.Instantiate(WeaponA) as WeaponDescriptor;
				mWeaponInstanceA.UnlimitedAmmo = true;
				mWeaponInstanceB = Object.Instantiate(WeaponB) as WeaponDescriptor;
				mWeaponInstanceB.UnlimitedAmmo = true;
				mWeaponInstanceC = Object.Instantiate(WeaponC) as WeaponDescriptor;
				mWeaponInstanceC.UnlimitedAmmo = true;
				mWeaponInstanceD = Object.Instantiate(WeaponD) as WeaponDescriptor;
				mWeaponInstanceD.UnlimitedAmmo = true;
				mWeaponInstanceE = Object.Instantiate(WeaponE) as WeaponDescriptor;
				mWeaponInstanceE.UnlimitedAmmo = true;
				mWeaponInstanceF = Object.Instantiate(WeaponF) as WeaponDescriptor;
				mWeaponInstanceF.UnlimitedAmmo = true;
				mWeaponInstanceG = Object.Instantiate(WeaponG) as WeaponDescriptor;
				mWeaponInstanceG.UnlimitedAmmo = true;
				mWeapon[0] = mWeaponInstanceA.Create(actor.model, 1f, 1f, 1f);
				mWeapon[1] = mWeaponInstanceB.Create(actor.model, 1f, 1f, 1f);
				mWeapon[2] = mWeaponInstanceC.Create(actor.model, 1f, 1f, 1f);
				mWeapon[3] = mWeaponInstanceD.Create(actor.model, 1f, 1f, 1f);
				mWeapon[4] = mWeaponInstanceE.Create(actor.model, 1f, 1f, 1f);
				mWeapon[5] = mWeaponInstanceF.Create(actor.model, 1f, 1f, 1f);
				mWeapon[6] = mWeaponInstanceG.Create(actor.model, 1f, 1f, 1f);
				StartCoroutine(Init());
			}
		}
	}

	private void Update()
	{
		if (mWeaponSwitchOnTimer)
		{
			mTimeToNextSwitch -= Time.deltaTime;
			if (mTimeToNextSwitch <= 0f)
			{
				SwitchWeapon();
				mTimeToNextSwitch = TimeBetweenWeapons;
			}
		}
	}

	private IEnumerator Init()
	{
		ActorWrapper actorWrapper = null;
		while (actorWrapper == null)
		{
			yield return null;
			actorWrapper = Actor.GetComponentInChildren<ActorWrapper>();
		}
		Actor actor = null;
		while (actor == null)
		{
			yield return null;
			actor = actorWrapper.GetActor();
		}
		actor.weapon.SwapTo(mWeapon[0], 1f);
		CommonHudController.Instance.PreventWeaponSwap = true;
	}

	public void SwitchWeapon()
	{
		ActorWrapper componentInChildren = Actor.GetComponentInChildren<ActorWrapper>();
		if (componentInChildren == null)
		{
			return;
		}
		Actor actor = componentInChildren.GetActor();
		if (!(actor == null))
		{
			actor.realCharacter.IsAimingDownSights = false;
			int num;
			for (num = Random.Range(0, 7); num == currentWeapon; num = Random.Range(0, 7))
			{
			}
			actor.weapon.SwapTo(mWeapon[num], 1f);
			currentWeapon = num;
		}
	}

	public void Deactivate()
	{
	}
}
