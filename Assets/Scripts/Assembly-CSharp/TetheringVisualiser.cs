using UnityEngine;

public class TetheringVisualiser : MonoBehaviour
{
	public static TetheringVisualiser instance;

	private bool mHidden;

	public static TetheringVisualiser Instance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		mHidden = true;
	}

	private void Update()
	{
	}

	public void GLDebugVisualise()
	{
		if (mHidden)
		{
			return;
		}
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.ActorsInPlay);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.tether.Active)
			{
				Vector3 position = a.tether.Position;
				Vector3 vector = a.GetPosition() + Vector3.up;
				if (a.tether.IsWithinTether())
				{
					GL.Color(Color.green);
				}
				else
				{
					GL.Color(Color.red);
				}
				GL.Vertex3(position.x, position.y, position.z);
				GL.Vertex3(vector.x, vector.y, vector.z);
			}
		}
	}

	public void Hide()
	{
		mHidden = true;
	}

	public void Show()
	{
		mHidden = false;
	}

	public void Toggle()
	{
		mHidden = !mHidden;
	}
}
