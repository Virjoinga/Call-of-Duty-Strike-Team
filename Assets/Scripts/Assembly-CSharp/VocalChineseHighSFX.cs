public class VocalChineseHighSFX : SFXBank
{
	public SoundFXData PainCry;

	public SoundFXData DeathCry;

	public SoundFXData GrenadeReaction;

	public SoundFXData OrderReceived;

	public static VocalChineseHighSFX Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<VocalChineseHighSFX>();
		}
	}

	public static bool HasInstance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<VocalChineseHighSFX>() != null;
		}
	}
}
