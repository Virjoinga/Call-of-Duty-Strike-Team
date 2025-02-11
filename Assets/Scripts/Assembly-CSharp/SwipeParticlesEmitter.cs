using UnityEngine;

public class SwipeParticlesEmitter : MonoBehaviour
{
	public ParticleEmitter emitter;

	public float baseSpeed = 4f;

	public float swipeVelocityScale = 0.001f;

	private void Start()
	{
		if (!emitter)
		{
			emitter = base.particleEmitter;
		}
		emitter.emit = false;
	}

	public void Emit(FingerGestures.SwipeDirection direction, float swipeVelocity)
	{
		Vector3 forward;
		switch (direction)
		{
		case FingerGestures.SwipeDirection.Up:
			forward = Vector3.up;
			break;
		case FingerGestures.SwipeDirection.Down:
			forward = Vector3.down;
			break;
		case FingerGestures.SwipeDirection.Right:
			forward = Vector3.right;
			break;
		default:
			forward = Vector3.left;
			break;
		}
		emitter.transform.rotation = Quaternion.LookRotation(forward);
		Vector3 localVelocity = emitter.localVelocity;
		localVelocity.z = baseSpeed * swipeVelocityScale * swipeVelocity;
		emitter.localVelocity = localVelocity;
		emitter.Emit();
	}
}
