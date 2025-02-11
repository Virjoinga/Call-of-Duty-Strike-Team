using UnityEngine;

public class LightSwitch : MonoBehaviour
{
	public Light LightToSwitch;

	public Color OnColour = Color.red;

	public Color OffColour = Color.green;

	public bool StartOn;

	private void Start()
	{
		if (LightToSwitch == null)
		{
			LightToSwitch = base.gameObject.light;
		}
		if (StartOn)
		{
			SwitchOn();
		}
		else
		{
			SwitchOff();
		}
	}

	private void SwitchOn()
	{
		LightToSwitch.color = OnColour;
	}

	private void SwitchOff()
	{
		LightToSwitch.color = OffColour;
	}
}
