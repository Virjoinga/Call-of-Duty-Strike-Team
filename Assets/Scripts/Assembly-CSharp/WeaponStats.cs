using System.Collections.Generic;

public class WeaponStats : Stats<WeaponStat>
{
	protected override void SetEventListeners()
	{
		EventHub.Instance.OnWeaponFired += WeaponFired;
	}

	private void WeaponFired(object sender, Events.WeaponFired args)
	{
		if (args.Attacker.PlayerControlled && !ActStructure.Instance.CurrentMissionIsVTOL())
		{
			WeaponStat currentMissionStat = GetCurrentMissionStat(args.Attacker.WeaponId);
			currentMissionStat.ShotsFired++;
		}
	}

	public override void Create()
	{
		WeaponDescriptor[] assaultRifles = WeaponManager.Instance.AssaultRifles;
		foreach (WeaponDescriptor weaponDescriptor in assaultRifles)
		{
			CreateStat(weaponDescriptor.name);
		}
		WeaponDescriptor[] lightMachineGuns = WeaponManager.Instance.LightMachineGuns;
		foreach (WeaponDescriptor weaponDescriptor2 in lightMachineGuns)
		{
			CreateStat(weaponDescriptor2.name);
		}
		WeaponDescriptor[] sniperRifles = WeaponManager.Instance.SniperRifles;
		foreach (WeaponDescriptor weaponDescriptor3 in sniperRifles)
		{
			CreateStat(weaponDescriptor3.name);
		}
		WeaponDescriptor[] shotguns = WeaponManager.Instance.Shotguns;
		foreach (WeaponDescriptor weaponDescriptor4 in shotguns)
		{
			CreateStat(weaponDescriptor4.name);
		}
		WeaponDescriptor[] sMGs = WeaponManager.Instance.SMGs;
		foreach (WeaponDescriptor weaponDescriptor5 in sMGs)
		{
			CreateStat(weaponDescriptor5.name);
		}
	}

	public string MostFiredWeapon()
	{
		int num = -1;
		string result = "--";
		foreach (KeyValuePair<string, WeaponStat> gameTotalStat in m_GameTotalStats)
		{
			if (gameTotalStat.Value.ShotsFired > num && gameTotalStat.Key != "Minigun")
			{
				num = gameTotalStat.Value.ShotsFired;
				result = gameTotalStat.Key;
			}
		}
		return result;
	}
}
