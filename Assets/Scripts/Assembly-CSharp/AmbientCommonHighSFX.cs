public class AmbientCommonHighSFX : SFXBank
{
	public SoundFXData FireGusts;

	public SoundFXData TankerFireGusts;

	public SoundFXData ElectricSparks;

	public static AmbientCommonHighSFX Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<AmbientCommonHighSFX>();
		}
	}

	public static bool HasInstance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<AmbientCommonHighSFX>() != null;
		}
	}
}
