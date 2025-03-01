public class VocalFriendly2SFX : SFXBank
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

	public SoundFXData FollowMeAnswered;

	public static VocalFriendly2SFX Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<VocalFriendly2SFX>();
		}
	}

	public static bool HasInstance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<VocalFriendly2SFX>() != null;
		}
	}
}
