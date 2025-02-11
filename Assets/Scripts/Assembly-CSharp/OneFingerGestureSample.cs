using UnityEngine;

public class OneFingerGestureSample : SampleBase
{
	public GameObject longPressObject;

	public GameObject tapObject;

	public GameObject doubleTapObject;

	public GameObject swipeObject;

	public GameObject dragObject;

	private int dragFingerIndex = -1;

	protected override string GetHelpText()
	{
		return "This sample demonstrates some of the supported single-finger gestures:\r\n\r\n- Drag: press the red sphere and move your finger to drag it around  \r\n\r\n- LongPress: keep your finger pressed on the cyan sphere for at least " + FingerGestures.Defaults.Fingers[0].LongPress.Duration + " seconds\r\n\r\n- Tap: press & release the purple sphere \r\n\r\n- Double Tap: quickly press & release the green sphere twice in a row\r\n\r\n- Swipe: press the yellow sphere and move your finger in one of the four cardinal directions, then release. The speed of the motion is taken into account.";
	}

	private void OnEnable()
	{
		Debug.Log("Registering finger gesture events from C# script");
		FingerGestures.OnFingerLongPress += FingerGestures_OnFingerLongPress;
		FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
		FingerGestures.OnFingerDoubleTap += FingerGestures_OnFingerDoubleTap;
		FingerGestures.OnFingerSwipe += FingerGestures_OnFingerSwipe;
		FingerGestures.OnFingerDragBegin += FingerGestures_OnFingerDragBegin;
		FingerGestures.OnFingerDragMove += FingerGestures_OnFingerDragMove;
		FingerGestures.OnFingerDragEnd += FingerGestures_OnFingerDragEnd;
	}

	private void OnDisable()
	{
		FingerGestures.OnFingerLongPress -= FingerGestures_OnFingerLongPress;
		FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
		FingerGestures.OnFingerDoubleTap -= FingerGestures_OnFingerDoubleTap;
		FingerGestures.OnFingerSwipe -= FingerGestures_OnFingerSwipe;
		FingerGestures.OnFingerDragBegin -= FingerGestures_OnFingerDragBegin;
		FingerGestures.OnFingerDragMove -= FingerGestures_OnFingerDragMove;
		FingerGestures.OnFingerDragEnd -= FingerGestures_OnFingerDragEnd;
	}

	private void FingerGestures_OnFingerLongPress(int fingerIndex, Vector2 fingerPos)
	{
		if (CheckSpawnParticles(fingerPos, longPressObject))
		{
			base.UI.StatusText = "Performed a long-press with finger " + fingerIndex;
		}
	}

	private void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		if (CheckSpawnParticles(fingerPos, tapObject))
		{
			base.UI.StatusText = "Tapped with finger " + fingerIndex;
		}
	}

	private void FingerGestures_OnFingerDoubleTap(int fingerIndex, Vector2 fingerPos)
	{
		if (CheckSpawnParticles(fingerPos, doubleTapObject))
		{
			base.UI.StatusText = "Double-Tapped with finger " + fingerIndex;
		}
	}

	private void FingerGestures_OnFingerSwipe(int fingerIndex, Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
	{
		GameObject gameObject = SampleBase.PickObject(startPos);
		if (gameObject == swipeObject)
		{
			base.UI.StatusText = string.Concat("Swiped ", direction, " with finger ", fingerIndex);
			SwipeParticlesEmitter componentInChildren = gameObject.GetComponentInChildren<SwipeParticlesEmitter>();
			if ((bool)componentInChildren)
			{
				componentInChildren.Emit(direction, velocity);
			}
		}
	}

	private void FingerGestures_OnFingerDragBegin(int fingerIndex, Vector2 fingerPos, Vector2 startPos)
	{
		GameObject gameObject = SampleBase.PickObject(startPos);
		if (gameObject == dragObject)
		{
			base.UI.StatusText = "Started dragging with finger " + fingerIndex;
			dragFingerIndex = fingerIndex;
			SpawnParticles(gameObject);
		}
	}

	private void FingerGestures_OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		if (fingerIndex == dragFingerIndex)
		{
			dragObject.transform.position = SampleBase.GetWorldPos(fingerPos);
		}
	}

	private void FingerGestures_OnFingerDragEnd(int fingerIndex, Vector2 fingerPos)
	{
		if (fingerIndex == dragFingerIndex)
		{
			base.UI.StatusText = "Stopped dragging with finger " + fingerIndex;
			dragFingerIndex = -1;
			SpawnParticles(dragObject);
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
