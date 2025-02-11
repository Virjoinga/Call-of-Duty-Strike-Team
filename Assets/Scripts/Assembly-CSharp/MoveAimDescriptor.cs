using UnityEngine;

public class MoveAimDescriptor : ScriptableObject
{
	public float mStandardShuffleSpeed;

	public float mStandardWalkSpeed;

	public float mStandardSaunterSpeed;

	public float mWalkRunThreshold;

	public float mStandardRunSpeed;

	public LerpOver mWalkingDirectionBlend;

	public LerpOver mWalkingXFade;

	public TweenTimeMatrix mTweenTimes;

	public CharacterType CharacterType = CharacterType.Human;
}
