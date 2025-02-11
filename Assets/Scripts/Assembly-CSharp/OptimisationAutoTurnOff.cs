using UnityEngine;

public class OptimisationAutoTurnOff : MonoBehaviour
{
	public OptimisationManager.OptimisationType TurnOffType;

	private void Awake()
	{
		if (TurnOffType == OptimisationManager.OptimisationType.LightHalos)
		{
			if (base.gameObject.GetComponent("Halo") != null && !OptimisationManager.CanUseOptmisation(TurnOffType))
			{
				Object.Destroy(base.gameObject.GetComponent("Halo"));
			}
			Object.Destroy(this);
		}
		else if (!OptimisationManager.CanUseOptmisation(TurnOffType))
		{
			Object.Destroy(base.gameObject);
		}
	}
}
