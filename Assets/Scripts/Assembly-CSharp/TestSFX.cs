public class TestSFX : SFXBank
{
	public SoundFXData FlashingZoomButton;

	public SoundFXData SelectUnit;

	public static TestSFX Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<TestSFX>();
		}
	}

	public static bool HasInstance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<TestSFX>() != null;
		}
	}
}
