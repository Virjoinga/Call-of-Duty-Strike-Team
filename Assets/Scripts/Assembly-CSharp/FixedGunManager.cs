using System.Collections.Generic;
using UnityEngine;

public class FixedGunManager : MonoBehaviour
{
	private const float kSqrSearchRadius = 100f;

	public static FixedGunManager instance;

	private List<FixedGun> mFixedGuns;

	public static FixedGunManager Instance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
		mFixedGuns = new List<FixedGun>();
	}

	private void Start()
	{
		Object[] array = Object.FindObjectsOfType(typeof(FixedGun));
		Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			FixedGun fixedGun = (FixedGun)array2[i];
			FixedGun item = fixedGun;
			mFixedGuns.Add(item);
		}
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public FixedGun GetNearAvailableFixedGun(Vector3 positionToSearchAround)
	{
		Vector3 zero = Vector3.zero;
		foreach (FixedGun mFixedGun in mFixedGuns)
		{
			if (!mFixedGun.IsAvailableForUse() || !((positionToSearchAround - mFixedGun.transform.position).sqrMagnitude < 100f))
			{
				continue;
			}
			return mFixedGun;
		}
		return null;
	}
}
