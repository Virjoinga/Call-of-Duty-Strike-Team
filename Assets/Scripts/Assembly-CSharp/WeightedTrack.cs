using UnityEngine;

public class WeightedTrack : MonoBehaviour
{
	public Transform[] targets;

	public bool OverrideDefaults;

	private Vector2[] averages;

	private Vector2[] velocities;

	private Vector2[] oldaverages;

	private Vector2[] screenpos;

	private float[] weights;

	private float[] oldweights;

	private float[,] separation;

	private int regcounter;

	public float kIndividualWeight;

	public float kPairWeight;

	public float kPairSeparationWeight;

	public float kMinViableWeight;

	public float kTotalWeightFloorScale;

	private CameraController camcon;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private void Awake()
	{
		averages = new Vector2[10];
		oldaverages = new Vector2[10];
		velocities = new Vector2[10];
		separation = new float[4, 4];
		weights = new float[10];
		oldweights = new float[10];
		screenpos = new Vector2[10];
		targets = new Transform[4];
		camcon = GetComponent<CameraController>();
		regcounter = 0;
		if (!OverrideDefaults)
		{
			kIndividualWeight = 150f;
			kPairWeight = 200f;
			kPairSeparationWeight = 0.05f;
			kMinViableWeight = 0.05f;
			kTotalWeightFloorScale = 0.2f;
		}
	}

	private void Update()
	{
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
		regcounter = 0;
		bool flag = true;
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a != null && a.realCharacter.IsUsingNavMesh())
			{
				if (targets[regcounter++] != a.transform)
				{
					targets[regcounter - 1] = a.transform;
					flag = false;
				}
				if (regcounter > 3)
				{
					break;
				}
			}
		}
		while (regcounter < 4)
		{
			targets[regcounter++] = null;
		}
		for (int i = 0; i < 10; i++)
		{
			oldweights[i] = weights[i];
			oldaverages[i] = averages[i];
		}
		CalcAverage(0);
		CalcAverage(1);
		CalcAverage(2);
		CalcAverage(3);
		CalcAverage(0, 1, 4);
		CalcAverage(0, 2, 5);
		CalcAverage(0, 3, 6);
		CalcAverage(1, 2, 7);
		CalcAverage(1, 3, 8);
		CalcAverage(2, 3, 9);
		if (!flag)
		{
			return;
		}
		for (int i = 0; i < 10; i++)
		{
			if (oldweights[i] == 0f)
			{
				oldaverages[i] = averages[i];
			}
		}
		NavMeshCamera navMeshCamera = camcon.CurrentCameraBase as NavMeshCamera;
		if (navMeshCamera == null)
		{
			return;
		}
		float num = 0.001f;
		for (int i = 0; i < 10; i++)
		{
			velocities[i] = averages[i] - oldaverages[i];
			num += oldweights[i];
			oldweights[i] *= Mathf.Clamp(1f + Vector2.Dot(navMeshCamera.mLastPanDirection.xz(), velocities[i].normalized), 0f, 1f);
		}
		navMeshCamera.mLastPanDirection *= 0.995f;
		float num2 = num;
		for (int i = 0; i < 10; i++)
		{
			if (oldweights[i] < kMinViableWeight)
			{
				num2 -= oldweights[i];
				oldweights[i] = 0f;
			}
		}
		num2 = Mathf.Max(num2, kTotalWeightFloorScale);
		Vector2 zero = Vector2.zero;
		if (num2 > 0f)
		{
			for (int i = 0; i < 10; i++)
			{
				zero += velocities[i] * (oldweights[i] / num2);
			}
		}
		if (navMeshCamera != null)
		{
			navMeshCamera.WorldSpacePan(zero);
		}
	}

	private void CalcAverage(int a, int b, int i)
	{
		if (targets[a] != null && targets[b] != null)
		{
			averages[i] = (averages[a] + averages[b]) * 0.5f;
			separation[a, b] = (averages[a] - averages[b]).sqrMagnitude;
			screenpos[i] = (screenpos[a] + screenpos[b]) * 0.5f;
			weights[i] = 1f / (1f + screenpos[i].sqrMagnitude * kPairWeight + separation[a, b] * kPairSeparationWeight);
		}
		else
		{
			weights[i] = 0f;
		}
	}

	private void CalcAverage(int i)
	{
		if (targets[i] != null)
		{
			averages[i] = targets[i].position.xz();
			screenpos[i] = camcon.camera.WorldToViewportPoint(targets[i].position).xy() - new Vector2(0.5f, 0.5f);
			weights[i] = 1f / (1f + screenpos[i].sqrMagnitude * kIndividualWeight);
		}
		else
		{
			weights[i] = 0f;
		}
	}
}
