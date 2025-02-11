using UnityEngine;

public class TwoFingerGestureSample : SampleBase
{
	public GameObject longPressObject;

	public GameObject tapObject;

	public GameObject swipeObject;

	public GameObject dragObject;

	public int requiredTapCount = 2;

	private bool dragging;

	protected override string GetHelpText()
	{
		return "This sample demonstrates some of the supported two-finger gestures:\r\n\r\n- Drag: press the red sphere with two fingers and move them to drag the sphere around  \r\n\r\n- LongPress: keep your two fingers pressed on the cyan sphere for at least " + FingerGestures.Defaults.Fingers[0].LongPress.Duration + " seconds\r\n\r\n- Tap: rapidly press & release the purple sphere times with two fingers\r\n\r\n- Swipe: press the yellow sphere with two fingers and move them in one of the four cardinal directions, then release your fingers. The speed of the motion is taken into account.";
	}

	private void OnEnable()
	{
		Debug.Log("Registering finger gesture events from C# script");
		FingerGestures.OnTwoFingerLongPress += FingerGestures_OnTwoFingerLongPress;
		FingerGestures.OnTwoFingerTap += FingerGestures_OnTwoFingerTap;
		FingerGestures.OnTwoFingerSwipe += FingerGestures_OnTwoFingerSwipe;
		FingerGestures.OnTwoFingerDragBegin += FingerGestures_OnTwoFingerDragBegin;
		FingerGestures.OnTwoFingerDragMove += FingerGestures_OnTwoFingerDragMove;
		FingerGestures.OnTwoFingerDragEnd += FingerGestures_OnTwoFingerDragEnd;
	}

	private void OnDisable()
	{
		FingerGestures.OnTwoFingerLongPress -= FingerGestures_OnTwoFingerLongPress;
		FingerGestures.OnTwoFingerTap -= FingerGestures_OnTwoFingerTap;
		FingerGestures.OnTwoFingerSwipe -= FingerGestures_OnTwoFingerSwipe;
		FingerGestures.OnTwoFingerDragBegin -= FingerGestures_OnTwoFingerDragBegin;
		FingerGestures.OnTwoFingerDragMove -= FingerGestures_OnTwoFingerDragMove;
		FingerGestures.OnTwoFingerDragEnd -= FingerGestures_OnTwoFingerDragEnd;
	}

	private void FingerGestures_OnTwoFingerLongPress(Vector2 fingerPos)
	{
		if (CheckSpawnParticles(fingerPos, longPressObject))
		{
			base.UI.StatusText = "Performed a two-finger long-press";
		}
	}

	private void FingerGestures_OnTwoFingerTap(Vector2 fingerPos)
	{
		if (CheckSpawnParticles(fingerPos, tapObject))
		{
			base.UI.StatusText = "Tapped times with two fingers";
		}
	}

	private void FingerGestures_OnTwoFingerSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
	{
		GameObject gameObject = SampleBase.PickObject(startPos);
		if (gameObject == swipeObject)
		{
			base.UI.StatusText = string.Concat("Swiped ", direction, " with two fingers");
			SwipeParticlesEmitter componentInChildren = gameObject.GetComponentInChildren<SwipeParticlesEmitter>();
			if ((bool)componentInChildren)
			{
				componentInChildren.Emit(direction, velocity);
			}
		}
	}

	private void FingerGestures_OnTwoFingerDragBegin(Vector2 fingerPos, Vector2 startPos)
	{
		GameObject gameObject = SampleBase.PickObject(startPos);
		if (gameObject == dragObject)
		{
			dragging = true;
			base.UI.StatusText = "Started dragging with two fingers";
			SpawnParticles(gameObject);
		}
	}

	private void FingerGestures_OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta)
	{
		if (dragging)
		{
			dragObject.transform.position = SampleBase.GetWorldPos(fingerPos);
		}
	}

	private void FingerGestures_OnTwoFingerDragEnd(Vector2 fingerPos)
	{
		if (dragging)
		{
			base.UI.StatusText = "Stopped dragging with two fingers";
			SpawnParticles(dragObject);
			dragging = false;
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
