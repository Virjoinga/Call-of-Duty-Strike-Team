public class CharacterHighSFX : SFXBank
{
	public SoundFXData DamageHealthMedium;

	public static CharacterHighSFX Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<CharacterHighSFX>();
		}
	}

	public static bool HasInstance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<CharacterHighSFX>() != null;
		}
	}
}
