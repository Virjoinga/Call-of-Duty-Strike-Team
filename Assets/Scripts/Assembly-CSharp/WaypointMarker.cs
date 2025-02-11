using UnityEngine;

public class WaypointMarker : MonoBehaviour
{
	public enum Type
	{
		Undefined = 0,
		OpenGround = 1,
		LowCover = 2,
		HighCoverLeft = 3,
		HighCoverRight = 4
	}

	public enum State
	{
		Undefined = 0,
		Walk = 1,
		Run = 2
	}

	public bool Highlighted;

	private CoverPointCore coverPoint;

	public Type type;

	public State state;

	private Renderer rend;

	private LineRenderer pathRend;

	private Animation anim;

	private AnimationState animState;

	public GameObject owner;

	public bool spin;

	public HudBlipIcon HudBlip;

	private Vector3 lastPosition;

	public static bool ShowPaths = true;

	public bool RefreshPaths;

	public int accessKey;

	private Vector3[] pathRemainder;

	private bool m_HasBeenInTransit;

	public bool HasBeenInTransit
	{
		get
		{
			return m_HasBeenInTransit;
		}
	}

	private void Awake()
	{
		type = Type.Undefined;
		state = State.Undefined;
		anim = base.animation;
		if (anim == null)
		{
			anim = GetComponentInChildren<Animation>();
		}
		rend = base.renderer;
		if (rend == null)
		{
			rend = GetComponentInChildren<Renderer>();
		}
		pathRend = new GameObject("PathRenderer").AddComponent<LineRenderer>();
		pathRend.material = EffectsController.Instance.Effects.SoldierPathMaterial;
		pathRend.SetWidth(0.4f, 0.4f);
		pathRend.SetColors(Color.green.Alpha(1f), Color.green.Alpha(1f));
		pathRend.transform.parent = base.transform;
		m_HasBeenInTransit = false;
		HudBlip = Object.Instantiate(HudBlip) as HudBlipIcon;
		HudBlip.Target = base.transform;
	}

	public void EnablePathRender(bool visible)
	{
		pathRend.enabled = visible;
	}

	private Vector3 GetCorner(NavMeshPath path, int index)
	{
		if (index < path.corners.Length)
		{
			return path.corners[index];
		}
		index -= path.corners.Length;
		if (pathRemainder != null && pathRemainder.Length > 0)
		{
			if (index < pathRemainder.Length)
			{
				return pathRemainder[index];
			}
			return pathRemainder[pathRemainder.Length - 1];
		}
		return path.corners[path.corners.Length - 1];
	}

	private void CalcPath(NavMeshPath path)
	{
		Actor actor = ((!(owner != null)) ? null : owner.GetComponent<Actor>());
		if (!(actor != null))
		{
			return;
		}
		path = path ?? actor.navAgent.path;
		if (path.status != 0)
		{
			return;
		}
		int num = path.corners.Length;
		if (pathRemainder != null)
		{
			num += pathRemainder.Length;
		}
		int num2 = num;
		if (num >= 2)
		{
			if (num2 > 2)
			{
				num2 += (num2 - 2) * 2;
			}
			Vector3 up = Vector3.up;
			pathRend.SetVertexCount(num2);
			pathRend.SetPosition(0, GetCorner(path, 0) + up);
			pathRend.SetPosition(num2 - 1, GetCorner(path, num - 1) + up);
			float magnitude = (GetCorner(path, 1) - GetCorner(path, 0)).magnitude;
			float magnitude2 = (GetCorner(path, num - 1) - GetCorner(path, num - 2)).magnitude;
			int num3 = 1;
			for (int i = 1; i < num - 1; i++)
			{
				Vector3 corner = GetCorner(path, i);
				Vector3 vector = Vector3.Lerp(corner, GetCorner(path, i - 1), 0.2f);
				Vector3 vector2 = Vector3.Lerp(corner, GetCorner(path, i + 1), 0.2f);
				pathRend.SetPosition(num3++, vector + up);
				pathRend.SetPosition(num3++, (corner + (vector + vector2) * 0.5f) * 0.5f + up);
				pathRend.SetPosition(num3++, vector2 + up);
			}
			float num4 = (float)num3 * 2f;
			pathRend.material.SetFloat("_EndProp", magnitude2 * num4);
			pathRend.material.SetFloat("_StartProp", magnitude * num4);
		}
	}

	private void Update()
	{
		if (spin)
		{
			Vector3 eulerAngles = base.gameObject.transform.eulerAngles;
			eulerAngles.y += TimeManager.DeltaTime * 360f;
			base.gameObject.transform.eulerAngles = eulerAngles;
		}
		if ((lastPosition - base.transform.position).sqrMagnitude > 1f)
		{
			Type type = this.type;
			this.type = Type.Undefined;
			SetType(type);
			lastPosition = base.transform.position;
		}
		else if (ShowPaths && RefreshPaths)
		{
			CalcPath(null);
		}
		RefreshPaths = false;
		Color color = rend.material.color;
		Color color2 = rend.material.color;
		float num = Time.realtimeSinceStartup % 1f;
		float num2 = 1f - num * 2f;
		num2 *= num2;
		color = (color2 = ColourBasedOnMovementSpeed());
		color.a = num;
		pathRend.SetColors(color2.Alpha(1f), color2.Alpha(1f));
		rend.material.color = color;
		if (animState != null)
		{
			animState.time += TimeManager.DeltaTime;
		}
	}

	public void SetType(Type t)
	{
		if (t == Type.Undefined || t == type)
		{
			return;
		}
		type = t;
		AnimationState animationState = null;
		switch (t)
		{
		case Type.OpenGround:
			animationState = anim["ADS_Idle"];
			HudBlip.SwitchOff();
			break;
		case Type.LowCover:
			animationState = anim["CrouchWallInto"];
			HudBlip.SwitchOn();
			break;
		case Type.HighCoverLeft:
			animationState = anim["AimWallLeftGhostInto"];
			HudBlip.SwitchOn();
			break;
		case Type.HighCoverRight:
			animationState = anim["AimWallRightGhostInto"];
			HudBlip.SwitchOn();
			break;
		default:
			TBFAssert.DoAssert(false, string.Format("Unknown movementType {0}", t));
			break;
		}
		if (animationState != null)
		{
			if (animState != null)
			{
				animState.enabled = false;
				animState.weight = 0f;
			}
			animState = animationState;
			animState.enabled = true;
			animState.weight = 1f;
			animState.speed = 0f;
			animState.time = 0f;
		}
	}

	public void SetPathRemainder(NavMeshPath path, int startIndex)
	{
		if (startIndex > path.corners.Length - 1)
		{
			pathRemainder = null;
			return;
		}
		pathRemainder = new Vector3[path.corners.Length - startIndex];
		for (int i = 0; i < pathRemainder.Length; i++)
		{
			pathRemainder[i] = path.corners[startIndex + i];
		}
		accessKey = Time.frameCount;
		RefreshPaths = false;
	}

	public void ClearPathRemainder()
	{
		pathRemainder = null;
	}

	public void PreviewPath()
	{
		bool flag = false;
		if (ShowPaths)
		{
			Actor actor = ((!(owner != null)) ? null : owner.GetComponent<Actor>());
			if (actor != null)
			{
				NavMeshPath navMeshPath = new NavMeshPath();
				if (WorldHelper.CalculatePath_AvoidingMantlesWhenCarrying(actor, base.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
				{
					pathRemainder = null;
					CalcPath(navMeshPath);
					flag = true;
					accessKey = Time.frameCount;
				}
			}
		}
		if (!flag)
		{
			pathRend.SetVertexCount(0);
			accessKey = 0;
		}
	}

	public void SetState(State s)
	{
		if (s != state)
		{
			state = s;
			rend.material.color = ColourBasedOnMovementSpeed();
		}
	}

	public void Hide()
	{
		base.transform.position = new Vector3(0f, 5000f, 0f);
	}

	private Color ColourBasedOnMovementSpeed()
	{
		if (state == State.Run)
		{
			return ColourChart.WaypointMarkerRun;
		}
		return ColourChart.WaypointMarkerInTransit;
	}

	public void SetTypeAndState(Type t, State s)
	{
		SetType(t);
		SetState(s);
	}

	public bool IsPreview()
	{
		return true;
	}

	public void MutePreview()
	{
	}

	public void NowWalking()
	{
		if (state == State.Run)
		{
			SetState(State.Walk);
		}
	}

	public void NowRunning()
	{
		if (state == State.Walk)
		{
			SetState(State.Run);
		}
		rend.material.color = ColourChart.WaypointMarkerRun;
		pathRend.SetColors(ColourChart.WaypointMarkerRun, ColourChart.WaypointMarkerRun);
	}

	public void SetCoverPoint(CoverPointCore cp)
	{
		coverPoint = cp;
		if (cp != null)
		{
			base.transform.forward = cp.AnimationFacing;
			if (cp.type == CoverPointCore.Type.HighCornerLeft)
			{
				SetType(Type.HighCoverLeft);
			}
			if (cp.type == CoverPointCore.Type.HighCornerRight)
			{
				SetType(Type.HighCoverRight);
			}
			if (cp.type == CoverPointCore.Type.ShootOver)
			{
				SetType(Type.LowCover);
			}
			if (cp.type == CoverPointCore.Type.HighWall)
			{
				SetType(Type.LowCover);
			}
		}
	}

	public CoverPointCore GetCoverPoint()
	{
		return coverPoint;
	}
}
