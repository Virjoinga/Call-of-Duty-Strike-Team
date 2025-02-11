using UnityEngine;

public class SentryGunWeaponModel : IWeaponModel
{
	private Transform mMuzzle;

	private Transform mCasings;

	public SentryGunWeaponModel(Actor sentry)
	{
		Transform parent = sentry.model.transform.FindInHierarchy("muzzle_flash") ?? sentry.transform;
		Transform transform = new GameObject("AimingMuzzle").transform;
		transform.parent = parent;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		mMuzzle = transform;
		Transform sentryModel = sentry.model.transform.FindInHierarchy("Sentry_Gun") ?? sentry.transform;
		TrackingRobotRealCharacter trackingRobotRealCharacter = sentry.realCharacter as TrackingRobotRealCharacter;
		if (trackingRobotRealCharacter != null)
		{
			trackingRobotRealCharacter.AimingMuzzle = mMuzzle;
			trackingRobotRealCharacter.SentryModel = sentryModel;
		}
		mCasings = sentry.model.transform.FindInHierarchy("cartridge");
	}

	public Transform GetMuzzleLocator()
	{
		return mMuzzle;
	}

	public Transform GetCasingsLocator()
	{
		return mCasings;
	}
}
