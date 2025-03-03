public class ArcticAmbientSharedHighSFX : SFXBank
{
	public SoundFXData WindGustArShared;

	public static ArcticAmbientSharedHighSFX Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<ArcticAmbientSharedHighSFX>();
		}
	}

	public static bool HasInstance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<ArcticAmbientSharedHighSFX>() != null;
		}
	}
}
