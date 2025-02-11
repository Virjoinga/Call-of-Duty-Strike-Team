using UnityEngine;

public class StandardWeaponMovement : IWeaponMovement
{
	private float mLeftRightLean;

	private float mUpDownLean;

	private float mMovementAmount;

	public float GetLeftRightLeanAmount()
	{
		return mLeftRightLean;
	}

	public float GetUpDownLeanAmount()
	{
		return mUpDownLean;
	}

	public float GetMovementAmount()
	{
		return mMovementAmount;
	}

	public void Reset()
	{
		mLeftRightLean = 0f;
		mUpDownLean = 0f;
		mMovementAmount = 0f;
	}

	public void Update(float deltaTime)
	{
		float num = (1f - Mathf.Abs(mLeftRightLean)) * 1f;
		mLeftRightLean = Mathf.Clamp(mLeftRightLean + deltaTime * num * GameController.Instance.LastViewRotation.y, -1f, 1f);
		mLeftRightLean = Mathf.Lerp(mLeftRightLean, 0f, 10f * deltaTime);
		float num2 = (1f - Mathf.Abs(mUpDownLean)) * 1f;
		mUpDownLean = Mathf.Clamp(mUpDownLean + deltaTime * num2 * GameController.Instance.LastViewRotation.x, -1f, 1f);
		mUpDownLean = Mathf.Lerp(mUpDownLean, 0f, 10f * deltaTime);
		float t = Mathf.InverseLerp(0f, 5f, GameController.Instance.LastVelocity.magnitude);
		mMovementAmount = Mathf.Lerp(0f, 1f, t);
	}
}
