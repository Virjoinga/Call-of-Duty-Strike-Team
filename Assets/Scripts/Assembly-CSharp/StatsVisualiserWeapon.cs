using System.Collections.Generic;
using UnityEngine;

public class StatsVisualiserWeapon : StatsVisualiserBase
{
	private List<string> m_Weapons;

	private int m_CurrentWeapon;

	public override void Start()
	{
		xPos = 520;
		yPos = 140;
		height = 70;
		CreateWeaponList();
		m_CurrentWeapon = 0;
		base.Start();
	}

	private void CreateWeaponList()
	{
		m_Weapons = new List<string>();
		AddWeapons(WeaponManager.Instance.AssaultRifles);
		AddWeapons(WeaponManager.Instance.LightMachineGuns);
		AddWeapons(WeaponManager.Instance.Shotguns);
		AddWeapons(WeaponManager.Instance.SniperRifles);
		AddWeapons(WeaponManager.Instance.SMGs);
	}

	private void AddWeapons(WeaponDescriptor[] weaponList)
	{
		foreach (WeaponDescriptor weaponDescriptor in weaponList)
		{
			m_Weapons.Add(weaponDescriptor.name);
		}
	}

	private void OnGUI()
	{
		if (!(StatsVisualiser.Instance() == null) && !StatsVisualiser.Instance().Hidden())
		{
			if (GUI.Button(prevRect, "<") && --m_CurrentWeapon < 0)
			{
				m_CurrentWeapon = m_Weapons.Count - 1;
			}
			if (GUI.Button(nextRect, ">") && ++m_CurrentWeapon >= m_Weapons.Count)
			{
				m_CurrentWeapon = 0;
			}
			string text = m_Weapons[m_CurrentWeapon];
			WeaponStat gameTotalStat = StatsManager.Instance.WeaponStats().GetGameTotalStat(text);
			WeaponStat currentMissionStat = StatsManager.Instance.WeaponStats().GetCurrentMissionStat(text);
			string empty = string.Empty;
			empty += string.Format("Shots fired: {0} ({1})", gameTotalStat.ShotsFired, currentMissionStat.ShotsFired);
			GUI.TextField(titleRect, "Weapon: " + text);
			GUI.TextField(statsRect, empty);
		}
	}
}
