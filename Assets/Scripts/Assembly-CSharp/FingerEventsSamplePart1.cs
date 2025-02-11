using UnityEngine;

public class FingerEventsSamplePart1 : SampleBase
{
	public GameObject fingerDownObject;

	public GameObject fingerStationaryObject;

	public GameObject fingerUpObject;

	public float chargeDelay = 0.5f;

	public float chargeTime = 5f;

	public float minSationaryParticleEmissionCount = 5f;

	public float maxSationaryParticleEmissionCount = 50f;

	public Material stationaryMaterial;

	public int requiredTapCount = 2;

	private ParticleEmitter stationaryParticleEmitter;

	private int stationaryFingerIndex = -1;

	private Material originalMaterial;

	protected override string GetHelpText()
	{
		return "This sample lets you visualize and understand the FingerDown, FingerStationary and FingerUp events.\r\n\r\nINSTRUCTIONS:\r\n- Press, hold and release the red and blue spheres\r\n- Press & hold the green sphere without moving for a few seconds";
	}

	protected override void Start()
	{
		base.Start();
		if ((bool)fingerStationaryObject)
		{
			stationaryParticleEmitter = fingerStationaryObject.GetComponentInChildren<ParticleEmitter>();
		}
	}

	private void StopStationaryParticleEmitter()
	{
		stationaryParticleEmitter.emit = false;
		base.UI.StatusText = string.Empty;
	}

	private void OnEnable()
	{
		Debug.Log("Registering finger gesture events from C# script");
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp += FingerGestures_OnFingerUp;
		FingerGestures.OnFingerStationaryBegin += FingerGestures_OnFingerStationaryBegin;
		FingerGestures.OnFingerStationary += FingerGestures_OnFingerStationary;
		FingerGestures.OnFingerStationaryEnd += FingerGestures_OnFingerStationaryEnd;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
		FingerGestures.OnFingerUp -= FingerGestures_OnFingerUp;
		FingerGestures.OnFingerStationaryBegin -= FingerGestures_OnFingerStationaryBegin;
		FingerGestures.OnFingerStationary -= FingerGestures_OnFingerStationary;
		FingerGestures.OnFingerStationaryEnd -= FingerGestures_OnFingerStationaryEnd;
	}

	private void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		CheckSpawnParticles(fingerPos, fingerDownObject);
	}

	private void FingerGestures_OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
	{
		CheckSpawnParticles(fingerPos, fingerUpObject);
		FingerGestures.Finger finger = FingerGestures.GetFinger(fingerIndex);
		Debug.Log(string.Concat("Finger was lifted up at ", finger.Position, " and moved ", finger.DistanceFromStart.ToString("N0"), " pixels from its initial position at ", finger.StartPosition));
	}

	private void FingerGestures_OnFingerStationaryBegin(int fingerIndex, Vector2 fingerPos)
	{
		if (stationaryFingerIndex == -1)
		{
			GameObject gameObject = SampleBase.PickObject(fingerPos);
			if (gameObject == fingerStationaryObject)
			{
				base.UI.StatusText = "Begin stationary on finger " + fingerIndex;
				stationaryFingerIndex = fingerIndex;
				originalMaterial = gameObject.renderer.sharedMaterial;
				gameObject.renderer.sharedMaterial = stationaryMaterial;
			}
		}
	}

	private void FingerGestures_OnFingerStationary(int fingerIndex, Vector2 fingerPos, float elapsedTime)
	{
		if (!(elapsedTime < chargeDelay))
		{
			GameObject gameObject = SampleBase.PickObject(fingerPos);
			if (gameObject == fingerStationaryObject)
			{
				float num = Mathf.Clamp01((elapsedTime - chargeDelay) / chargeTime);
				float num2 = Mathf.Lerp(minSationaryParticleEmissionCount, maxSationaryParticleEmissionCount, num);
				stationaryParticleEmitter.minEmission = num2;
				stationaryParticleEmitter.maxEmission = num2;
				stationaryParticleEmitter.emit = true;
				base.UI.StatusText = "Charge: " + (100f * num).ToString("N1") + "%";
			}
		}
	}

	private void FingerGestures_OnFingerStationaryEnd(int fingerIndex, Vector2 fingerPos, float elapsedTime)
	{
		if (fingerIndex == stationaryFingerIndex)
		{
			base.UI.StatusText = "Stationary ended on finger " + fingerIndex + " - " + elapsedTime.ToString("N1") + " seconds elapsed";
			StopStationaryParticleEmitter();
			fingerStationaryObject.renderer.sharedMaterial = originalMaterial;
			stationaryFingerIndex = -1;
		}
	}

	private bool CheckSpawnParticles(Vector2 fingerPos, GameObject requiredObject)
	{
		GameObject gameObject = SampleBase.PickObject(fingerPos);
		if (!gameObject || gameObject != requiredObject)
		{
			return false;
		}
		SpawnParticles(gameObject);
		return true;
	}

	private void SpawnParticles(GameObject obj)
	{
		ParticleEmitter componentInChildren = obj.GetComponentInChildren<ParticleEmitter>();
		if ((bool)componentInChildren)
		{
			componentInChildren.Emit();
		}
	}
}
