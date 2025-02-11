using UnityEngine;

public class MultiFingerSwipeSample : SampleBase
{
	public SwipeGestureRecognizer swipeGesture;

	public GameObject sphereObject;

	public float baseEmitSpeed = 4f;

	public float swipeVelocityEmitSpeedScale = 0.001f;

	protected override string GetHelpText()
	{
		return "Swipe: press the yellow sphere with " + swipeGesture.RequiredFingerCount + " fingers and move them in one of the four cardinal directions, then release. The speed of the motion is taken into account.";
	}

	protected override void Start()
	{
		base.Start();
		swipeGesture.OnSwipe += OnSwipe;
	}

	private void OnSwipe(SwipeGestureRecognizer source)
	{
		GameObject gameObject = SampleBase.PickObject(source.StartPosition);
		if (gameObject == sphereObject)
		{
			base.UI.StatusText = string.Concat("Swiped ", source.Direction, " with velocity: ", source.Velocity);
			SwipeParticlesEmitter componentInChildren = gameObject.GetComponentInChildren<SwipeParticlesEmitter>();
			if ((bool)componentInChildren)
			{
				componentInChildren.Emit(source.Direction, source.Velocity);
			}
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
