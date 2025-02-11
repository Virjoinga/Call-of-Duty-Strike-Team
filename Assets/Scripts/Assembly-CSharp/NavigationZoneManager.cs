using System;
using UnityEngine;

public class NavigationZoneManager : MonoBehaviour
{
	public delegate void OnClearBookingHandler(Actor client);

	private static NavigationZoneManager smInstance;

	public static NavigationZoneManager Instance
	{
		get
		{
			return smInstance;
		}
	}

	public event OnClearBookingHandler OnClearBookings;

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple NavigationZoneManagers");
		}
		smInstance = this;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	public static void ClearBookings(Actor client)
	{
		if (smInstance != null && smInstance.OnClearBookings != null)
		{
			smInstance.OnClearBookings(client);
		}
	}
}
