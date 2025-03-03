public class ArcticAmbientSFX : SFXBank
{
	public SoundFXData BoatWaterIdle;

	public SoundFXData BuildingRattle;

	public SoundFXData DistantMechLoop;

	public SoundFXData GeneratorMotor;

	public SoundFXData GeneratorMotorSmall;

	public SoundFXData InteriorMetalCabin;

	public SoundFXData MetalShipStress;

	public SoundFXData WaterLoop;

	public SoundFXData WindGust;

	public SoundFXData WindLoop;

	public SoundFXData WoodenPierStress;

	public SoundFXData LightBuzz;

	public SoundFXData CraneRattle;

	public SoundFXData PoleRattle;

	public SoundFXData PylonLoop;

	public SoundFXData ShoreFlatLoop;

	public SoundFXData ShoreDipLoop;

	public SoundFXData ShoreBigLoop;

	public SoundFXData VentPipe;

	public SoundFXData VentChimney;

	public SoundFXData LargeBuildingIntLoop;

	public SoundFXData XLargeBuildingIntLoop;

	public SoundFXData AlertAlarm;

	public SoundFXData MuffledChat;

	public SoundFXData Tannoy;

	public SoundFXData ArcDistantHeliLoop;

	public static ArcticAmbientSFX Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<ArcticAmbientSFX>();
		}
	}

	public static bool HasInstance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<ArcticAmbientSFX>() != null;
		}
	}
}
