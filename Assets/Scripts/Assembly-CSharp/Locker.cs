using UnityEngine;

[RequireComponent(typeof(CMLocker))]
public class Locker : HidingPlace
{
	private void Awake()
	{
		mContextMenu = GetComponent<CMLocker>();
	}
}
