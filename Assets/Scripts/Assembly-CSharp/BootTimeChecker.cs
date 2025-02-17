using System;
using System.Collections;
using UnityEngine;

public class BootTimeChecker : MonoBehaviour
{
	private bool started;

	private string expPath;

	private string mainPath;

	private void Start()
	{
		started = true;
		if (0 == 0)
		{
			LVLChecker instance = LVLChecker.Instance;
			instance.OnGotLicenseStatus = (LVLChecker.LVLCheckerEventHandler)Delegate.Combine(instance.OnGotLicenseStatus, new LVLChecker.LVLCheckerEventHandler(OnGotLicenseStatus));
			LVLChecker.Instance.StartCheckLicense();
			MoveToNextScreen();
		}
	}

	private void OnGotLicenseStatus(LVLChecker.LicenseCheckState state)
	{
		LVLChecker instance = LVLChecker.Instance;
		instance.OnGotLicenseStatus = (LVLChecker.LVLCheckerEventHandler)Delegate.Remove(instance.OnGotLicenseStatus, new LVLChecker.LVLCheckerEventHandler(OnGotLicenseStatus));
		switch (state)
		{
		case LVLChecker.LicenseCheckState.LICENSED:
			IsAnUpstandingCitizen();
			break;
		case LVLChecker.LicenseCheckState.NOT_LICENSED:
			IsAPirate();
			break;
		}
	}

	private void IsAnUpstandingCitizen()
	{
		Debug.Log("IsAnUpstandingCitizen");
	}

	private void IsAPirate()
	{
		Debug.Log("IsAPirate");
	}

	private IEnumerator WaitForOBB()
	{
		while (true)
		{
			Debug.Log("Waiting for OBB");
			if (mainPath != null)
			{
				MoveToNextScreen();
			}
			yield return null;
		}
	}

	private void MoveToNextScreen()
	{
		Application.LoadLevel("StingMovie");
	}

	private void OnApplicationFocus(bool focused)
	{
		Debug.Log(focused);
	}
}
