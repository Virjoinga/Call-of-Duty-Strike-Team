using System;
using UnityEngine;

[Serializable]
public class PhoneDetails
{
	[SerializeField]
	public OptimisationManager.HardwareType OptimisationType;

	[SerializeField]
	public string Description;

	public PhoneDetails()
	{
		OptimisationType = OptimisationManager.HardwareType.AndroidLow;
		Description = null;
	}

	public PhoneDetails(OptimisationManager.HardwareType optimisationType, string description)
	{
		OptimisationType = optimisationType;
		Description = description;
	}
}
