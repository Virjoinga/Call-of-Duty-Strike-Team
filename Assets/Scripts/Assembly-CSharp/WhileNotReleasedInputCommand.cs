using System.Collections;
using UnityEngine;

public class WhileNotReleasedInputCommand : Command
{
	public enum InputType
	{
		Move = 0,
		Look = 1
	}

	public InputType inputType = InputType.Look;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		bool pressed = false;
		bool released = false;
		while (!pressed || !released)
		{
			if (pressed)
			{
				released = ((inputType != InputType.Look) ? (CommonHudController.Instance.MoveAmount == Vector2.zero) : (CommonHudController.Instance.LookAmountTouch == Vector2.zero && !CommonHudController.Instance.HoldingView && CommonHudController.Instance.LookAmountGamepad == Vector2.zero));
			}
			else
			{
				pressed = ((inputType != InputType.Look) ? (CommonHudController.Instance.MoveAmount != Vector2.zero) : (CommonHudController.Instance.LookAmountTouch != Vector2.zero || CommonHudController.Instance.LookAmountGamepad != Vector2.zero));
			}
			yield return null;
		}
	}
}
