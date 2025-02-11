using UnityEngine;

public class CoverPointVisualiser : MonoBehaviour
{
	public static CoverPointVisualiser instance;

	private bool mHidden;

	public static CoverPointVisualiser Instance()
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
		CoverPointManager coverPointManager = CoverPointManager.Instance();
		foreach (CoverPoint coverPoint in coverPointManager.CoverPoints)
		{
			WaypointGameObject waypointGObj = coverPoint.WaypointGObj;
			float y = waypointGObj.GetPosition().y;
			Vector3 vector = new Vector3(waypointGObj.GetPosition().x, y + 1f, waypointGObj.GetPosition().z);
			Vector3 inGameForward = waypointGObj.inGameForward;
			Vector3 vector2 = vector + inGameForward * 2f;
			if ((waypointGObj.Configuration & WaypointGameObject.Flavour.Cover) != 0)
			{
				Vector3 inGamePosition = waypointGObj.inGamePosition;
				if (coverPoint.Owner != null)
				{
					Vector3 vector3 = coverPoint.WaypointGObj.snappedToPos - (coverPoint.Owner.myActor.GetPosition() + coverPoint.WaypointGObj.inGameTangent * -0.25f);
					vector3.y = 0f;
					Vector3 position = coverPoint.Owner.myActor.GetPosition();
					position.y += 1f;
					Vector3 vector4 = coverPoint.Owner.myActor.GetPosition() + vector3 * 10f;
					vector4.y += 1f;
					GL.Color(Color.cyan);
					GL.Vertex3(position.x, position.y, position.z);
					GL.Vertex3(vector4.x, vector4.y, vector4.z);
					GL.Color(Color.red);
					GL.Vertex3(inGamePosition.x, inGamePosition.y, inGamePosition.z);
					GL.Vertex3(coverPoint.Owner.myActor.GetPosition().x, coverPoint.Owner.myActor.GetPosition().y, coverPoint.Owner.myActor.GetPosition().z);
					GL.Color(Color.green);
				}
				else
				{
					GL.Color(Color.blue);
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
