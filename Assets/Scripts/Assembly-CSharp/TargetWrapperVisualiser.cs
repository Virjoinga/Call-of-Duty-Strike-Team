using UnityEngine;

public class TargetWrapperVisualiser : MonoBehaviour
{
	public static TargetWrapperVisualiser instance;

	private bool mHidden;

	public static TargetWrapperVisualiser Instance()
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
		TargetWrapperManager targetWrapperManager = TargetWrapperManager.Instance();
		foreach (TargetWrapper targetWrapper in targetWrapperManager.TargetWrappers)
		{
			WaypointGameObject waypointGObj = targetWrapper.InternalCoverPoint().WaypointGObj;
			float y = waypointGObj.GetPosition().y;
			Vector3 vector = new Vector3(waypointGObj.GetPosition().x, y + 1f, waypointGObj.GetPosition().z);
			Vector3 inGameForward = waypointGObj.inGameForward;
			Vector3 vector2 = vector + inGameForward * 2f;
			if ((waypointGObj.Configuration & WaypointGameObject.Flavour.Cover) != 0)
			{
				Vector3 inGamePosition = waypointGObj.inGamePosition;
				if (targetWrapper.HasBeenSearched())
				{
					GL.Color(Color.white);
				}
				else if (targetWrapper.HasBeenReserved())
				{
					GL.Color(Color.green);
				}
				else
				{
					GL.Color(Color.blue);
				}
				if (targetWrapper.IsPartOfASearchArea)
				{
					GL.Color(Color.black);
				}
				GL.Vertex3(inGamePosition.x, inGamePosition.y, inGamePosition.z);
				GL.Vertex3(vector.x, vector.y, vector.z);
				GL.Vertex3(vector.x, vector.y, vector.z);
				GL.Vertex3(vector2.x, vector2.y, vector2.z);
				Quaternion quaternion = Quaternion.AngleAxis(90f, Vector3.up);
				vector2 = vector + quaternion * inGameForward * 0.5f;
				GL.Vertex3(vector.x, vector.y, vector.z);
				GL.Vertex3(vector2.x, vector2.y, vector2.z);
				vector2 = vector - quaternion * inGameForward * 0.5f;
				GL.Vertex3(vector.x, vector.y, vector.z);
				GL.Vertex3(vector2.x, vector2.y, vector2.z);
			}
		}
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.AIMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.realCharacter.mSearchArea != null && a.tasks.IsRunningTask(typeof(TaskFlushOutEnemies)))
			{
				a.realCharacter.mSearchArea.GLDebugVisualise();
			}
		}
		TetheringVisualiser.Instance().Show();
		TetheringVisualiser.Instance().GLDebugVisualise();
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
