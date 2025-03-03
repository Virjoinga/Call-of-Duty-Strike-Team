public class VocalFriendlyChineseVtolSFX : SFXBank
{
	public SoundFXData PainCry;

	public SoundFXData DeathCry;

	public SoundFXData ClaymoreDropped;

	public SoundFXData GrenadeThrown;

	public SoundFXData ManDown;

	public SoundFXData KillConfirm;

	public SoundFXData GrenadeReaction;

	public SoundFXData OrderReceived;

	public SoundFXData StealthKillConfirm;

	public SoundFXData HelicopterExit;

	public SoundFXData Injured;

	public SoundFXData Reload;

	public SoundFXData SniperDown;

	public SoundFXData SniperSpotted;

	public SoundFXData SquadRevived;

	public SoundFXData EnemyRPG;

	public SoundFXData LostAimedShot;

	public static VocalFriendlyChineseVtolSFX Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<VocalFriendlyChineseVtolSFX>();
		}
	}

	public static bool HasInstance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<VocalFriendlyChineseVtolSFX>() != null;
		}
	}
}
