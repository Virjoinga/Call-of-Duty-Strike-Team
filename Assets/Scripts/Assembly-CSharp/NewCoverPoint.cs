using System;
using UnityEngine;

[ExecuteInEditMode]
public class NewCoverPoint : MonoBehaviour
{
	public enum Type
	{
		OpenGround = 0,
		ShootOver = 1,
		HighWall = 2,
		HighCornerLeft = 3,
		HighCornerRight = 4
	}

	public const float kDistanceAwayFromWall = 0.4f;

	public const float kDistanceAwayFromLowCover = 0.6f;

	public const float kAdjustmentToCentreCrouch = -0.2f;

	public const float kAdjustmentToCentreCrouchLeftEnd = -0.1f;

	public const float kAdjustmentToCentreCrouchRightEnd = -0.3f;

	private const float kDistanceBackFromCorner = 0.45f;

	private const float kPreviewHeight = 0.3f;

	[HideInInspector]
	public bool isChild;

	public bool GenerateChild;

	public bool snap;

	public Type type;

	public int index;

	public Vector3 snappedPos;

	public Vector3 snappedNormal;

	public Vector3 snappedTangent;

	public Vector3 gamePos;

	public Vector3 coverCheckPos;

	public int subsectionMask;

	public CoverPointCore core;

	private GameObject snappedTo;

	private Vector3 snappedToPos;

	private Quaternion snappedToAngle;

	private Vector3 oldPos;

	private Quaternion oldRot;

	private bool moveWithObject;

	private bool oldSnap;

	private bool oldGenerateChild;

	private bool makedirty;

	private bool firstUpdate;

	private static RaycastHit[] rch;

	public CoverPointCore cpc
	{
		get
		{
			return core;
		}
	}

	public bool dirty
	{
		set
		{
			if (!Application.isPlaying)
			{
				NewCoverPointManager owner = GetOwner();
				makedirty = false;
				if (owner != null)
				{
					owner.dirty = true;
				}
				else
				{
					makedirty = true;
				}
			}
		}
	}

	private void OnDestroy()
	{
		dirty = true;
	}

	private void EnsureTheCore()
	{
		if (core == null)
		{
			index = -1;
			core = ScriptableObject.CreateInstance<CoverPointCore>();
			core.type = (CoverPointCore.Type)type;
			core.snappedPos = snappedPos;
			core.snappedNormal = snappedNormal;
			core.snappedTangent = snappedTangent;
			core.gamePos = gamePos;
			core.coverCheckPos = coverCheckPos;
			dirty = true;
		}
		else
		{
			index = core.index;
		}
	}

	private void Awake()
	{
		if (core != null && !Application.isPlaying)
		{
			type = (Type)core.type;
			NewCoverPoint[] array = (NewCoverPoint[])UnityEngine.Object.FindObjectsOfType(typeof(NewCoverPoint));
			if (array != null)
			{
				for (int i = 0; i < array.GetLength(0); i++)
				{
					if (array[i] != this && array[i].core == core)
					{
						core = null;
						dirty = true;
						break;
					}
				}
			}
		}
		EnsureTheCore();
		oldSnap = snap;
		oldGenerateChild = GenerateChild;
		if (cpc.type != 0)
		{
			base.transform.forward = cpc.snappedNormal * -1f;
		}
		oldPos = base.transform.position;
		oldRot = base.transform.rotation;
	}

	private void Start()
	{
		oldPos = Vector3.zero;
		firstUpdate = true;
		if (makedirty)
		{
			dirty = true;
		}
	}

	private void TransformPositionRelative(ref Vector3 v)
	{
		v = snappedTo.transform.rotation * (Quaternion.Inverse(snappedToAngle) * (v - snappedToPos)) + snappedTo.transform.position;
	}

	private void TransformVectorRelative(ref Vector3 v)
	{
		v = snappedTo.transform.rotation * (Quaternion.Inverse(snappedToAngle) * v);
	}

	private void MoveRelative()
	{
		NewCoverPoint newCoverPoint = null;
		if (base.transform.childCount > 0)
		{
			newCoverPoint = base.transform.GetChild(0).GetComponent<NewCoverPoint>();
		}
		TransformPositionRelative(ref cpc.snappedPos);
		TransformVectorRelative(ref cpc.snappedNormal);
		TransformVectorRelative(ref cpc.snappedTangent);
		base.transform.position = cpc.snappedPos + cpc.snappedNormal * 0.4f + Vector3.up * 0.5f;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			newCoverPoint = base.transform.GetChild(i).GetComponent<NewCoverPoint>();
			if (newCoverPoint != null)
			{
				newCoverPoint.snappedToPos = snappedToPos;
				newCoverPoint.snappedToAngle = snappedToAngle;
				newCoverPoint.snappedTo = snappedTo;
				newCoverPoint.MoveRelative();
			}
		}
		snappedToPos = snappedTo.transform.position;
		snappedToAngle = snappedTo.transform.rotation;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			base.enabled = false;
			return;
		}
		if (makedirty)
		{
			makedirty = false;
			dirty = true;
		}
		if (!snap)
		{
			snappedTo = null;
		}
		if (!isChild)
		{
			if (moveWithObject)
			{
				if (base.transform.position != oldPos || snap != oldSnap)
				{
					dirty = true;
					moveWithObject = false;
					SnapToSurface();
				}
				else if (snappedToPos != snappedTo.transform.position || snappedToAngle != snappedTo.transform.rotation)
				{
					dirty = true;
					MoveRelative();
				}
			}
			else if (snappedTo != null && (snappedToPos != snappedTo.transform.position || snappedToAngle != snappedTo.transform.rotation))
			{
				dirty = true;
				moveWithObject = true;
				MoveRelative();
			}
			else if (base.transform.position != oldPos || base.transform.rotation != oldRot || snap != oldSnap || oldGenerateChild != GenerateChild)
			{
				SnapToSurface();
				if (firstUpdate)
				{
					firstUpdate = false;
				}
				else
				{
					dirty = true;
				}
			}
		}
		oldSnap = snap;
		oldGenerateChild = GenerateChild;
		oldPos = base.transform.position;
		oldRot = base.transform.rotation;
		cpc.gamePos = cpc.snappedPos;
		cpc.coverCheckPos = cpc.gamePos;
		if (cpc.type == CoverPointCore.Type.HighCornerRight)
		{
			cpc.snappedTangent = new Vector3(cpc.snappedNormal.z, 0f, 0f - cpc.snappedNormal.x);
		}
		else
		{
			cpc.snappedTangent = new Vector3(0f - cpc.snappedNormal.z, 0f, cpc.snappedNormal.x);
		}
		switch (cpc.type)
		{
		case CoverPointCore.Type.HighWall:
			cpc.gamePos += cpc.snappedNormal * 0.4f;
			cpc.coverCheckPos = cpc.gamePos;
			break;
		case CoverPointCore.Type.HighCornerLeft:
		case CoverPointCore.Type.HighCornerRight:
			cpc.gamePos += cpc.snappedNormal * 0.4f;
			cpc.coverCheckPos = cpc.gamePos + cpc.snappedTangent * 1f;
			break;
		case CoverPointCore.Type.ShootOver:
		{
			cpc.gamePos += cpc.snappedNormal * 0.6f;
			cpc.coverCheckPos = cpc.gamePos;
			Vector3 vector = new Vector3(0f - cpc.snappedNormal.z, 0f, cpc.snappedNormal.x);
			float num = -0.2f;
			if (cpc.stepOutLeft && !cpc.stepOutRight)
			{
				num = -0.1f;
			}
			if (!cpc.stepOutLeft && cpc.stepOutRight)
			{
				num = -0.3f;
			}
			cpc.gamePos += vector * num;
			NavMeshHit hit;
			if (NavMesh.SamplePosition(cpc.gamePos, out hit, 0.5f, -1))
			{
				cpc.gamePos = hit.position;
			}
			break;
		}
		case CoverPointCore.Type.OpenGround:
		case CoverPointCore.Type.InAir:
			snappedTo = null;
			break;
		}
	}

	public void PositionFromCore()
	{
		index = cpc.index;
		Vector3 position = cpc.gamePos;
		switch (cpc.type)
		{
		case CoverPointCore.Type.HighWall:
			position -= cpc.snappedNormal * 0.4f;
			break;
		case CoverPointCore.Type.HighCornerLeft:
		case CoverPointCore.Type.HighCornerRight:
			position -= cpc.snappedNormal * 0.4f;
			break;
		case CoverPointCore.Type.ShootOver:
			position -= cpc.snappedNormal * 0.6f;
			break;
		}
		base.transform.position = position;
		base.transform.forward = cpc.snappedNormal * -1f;
	}

	public NewCoverPointManager GetOwner()
	{
		if (base.transform.parent != null)
		{
			if (isChild && base.transform.parent.parent != null)
			{
				return base.transform.parent.parent.GetComponent<NewCoverPointManager>();
			}
			return base.transform.parent.GetComponent<NewCoverPointManager>();
		}
		return null;
	}

	private void OnDrawGizmos()
	{
		if (NewCoverPointManager.Instance() != null)
		{
			if (Application.isPlaying && index >= 0 && index < NewCoverPointManager.Instance().coverPoints.Length && NewCoverPointManager.Instance() != null && !NewCoverPointManager.Instance().dirty)
			{
				core = NewCoverPointManager.Instance().coverPoints[index];
			}
		}
		else
		{
			EnsureTheCore();
		}
	}

	private void SnapToSurface()
	{
		NewCoverPoint ncp = null;
		if (base.transform.childCount > 0)
		{
			ncp = base.transform.GetChild(0).GetComponent<NewCoverPoint>();
		}
		bool flag = true;
		float num = (float)Math.PI / 8f;
		float num2 = 0f;
		Vector3 zero = Vector3.zero;
		Vector3 origin = base.transform.position;
		RaycastHit hitInfo;
		if (Physics.Raycast(origin, Vector3.down, out hitInfo, 4f, 1))
		{
			origin = hitInfo.point + new Vector3(0f, 0.3f, 0f);
		}
		if (!snap)
		{
			if (ncp != null)
			{
				UnityEngine.Object.DestroyImmediate(ncp.gameObject);
			}
			if (cpc.type == CoverPointCore.Type.InAir)
			{
				cpc.snappedPos = base.transform.position;
				cpc.snappedNormal = base.transform.forward;
				return;
			}
			cpc.snappedPos = origin;
			cpc.snappedPos.y -= 0.3f;
			if (oldSnap)
			{
				base.transform.forward = cpc.snappedNormal * -1f;
			}
			else
			{
				cpc.snappedNormal = base.transform.forward * -1f;
			}
			return;
		}
		if (!GenerateChild && ncp != null)
		{
			UnityEngine.Object.DestroyImmediate(ncp.gameObject);
			ncp = null;
		}
		if (rch == null)
		{
			rch = new RaycastHit[16];
		}
		bool flag2 = false;
		for (int i = 0; i < 16; i++)
		{
			zero.x = Mathf.Cos(num2);
			zero.z = Mathf.Sin(num2);
			zero.y = 0f;
			if (Physics.Raycast(origin, zero, out rch[i], 1.5f, 1) && Mathf.Abs(rch[i].normal.y) < 0.3f)
			{
				flag2 = true;
			}
			else
			{
				rch[i].distance = 100f;
			}
			num2 += num;
		}
		if (!flag2)
		{
			if (ncp != null)
			{
				UnityEngine.Object.DestroyImmediate(ncp.gameObject);
			}
			cpc.snappedPos = origin;
			cpc.snappedPos.y -= 0.3f;
			cpc.snappedNormal = base.transform.forward * -1f;
			cpc.type = CoverPointCore.Type.OpenGround;
			return;
		}
		int num3 = 0;
		for (int i = 1; i < 16; i++)
		{
			if (rch[i].distance < rch[num3].distance)
			{
				num3 = i;
			}
		}
		cpc.snappedNormal = rch[num3].normal;
		cpc.snappedNormal.y = 0f;
		cpc.snappedPos = rch[num3].point;
		cpc.coverCheckPos = cpc.snappedPos;
		snappedTo = rch[num3].transform.gameObject;
		snappedToPos = snappedTo.transform.position;
		snappedToAngle = snappedTo.transform.rotation;
		cpc.type = CoverPointCore.Type.ShootOver;
		cpc.stepOutLeft = false;
		cpc.stepOutRight = false;
		cpc.snappedTangent = new Vector3(0f - cpc.snappedNormal.z, 0f, cpc.snappedNormal.x);
		Vector3 origin2 = cpc.snappedPos + cpc.snappedTangent * 0.5f - cpc.snappedNormal * 0.05f;
		Vector3 vector;
		if (Physics.Raycast(origin2, -cpc.snappedTangent, out hitInfo, 1f))
		{
			if (Vector3.Dot(hitInfo.normal, cpc.snappedNormal) < 0.8f)
			{
				vector = new Vector3(hitInfo.normal.z, 0f, 0f - hitInfo.normal.x);
				cpc.snappedPos += cpc.snappedTangent * (0.5f - hitInfo.distance);
				cpc.type = CoverPointCore.Type.HighCornerLeft;
				Vector3 vector2 = cpc.snappedPos;
				cpc.snappedPos -= cpc.snappedTangent * 0.45f;
				if (GenerateChild)
				{
					MakeChildCover(ref ncp);
					ncp.cpc.stepOutLeft = false;
					ncp.cpc.stepOutRight = false;
					ncp.cpc.snappedPos = vector2 - vector * 0.45f;
					ncp.transform.position = ncp.cpc.snappedPos;
					ncp.cpc.snappedNormal = hitInfo.normal;
					ncp.transform.forward = hitInfo.normal * -1f;
					ncp.cpc.snappedTangent = vector;
					ncp.cpc.type = CoverPointCore.Type.HighCornerRight;
					flag = false;
				}
			}
		}
		else
		{
			origin2 = cpc.snappedPos - cpc.snappedTangent * 0.5f - cpc.snappedNormal * 0.05f;
			if (Physics.Raycast(origin2, cpc.snappedTangent, out hitInfo, 1f) && Vector3.Dot(hitInfo.normal, cpc.snappedNormal) < 0.8f)
			{
				cpc.snappedTangent *= -1f;
				vector = new Vector3(0f - hitInfo.normal.z, 0f, hitInfo.normal.x);
				cpc.snappedPos += cpc.snappedTangent * (0.5f - hitInfo.distance);
				Vector3 vector2 = cpc.snappedPos;
				cpc.type = CoverPointCore.Type.HighCornerRight;
				cpc.snappedPos -= cpc.snappedTangent * 0.45f;
				if (GenerateChild)
				{
					MakeChildCover(ref ncp);
					ncp.cpc.stepOutLeft = false;
					ncp.cpc.stepOutRight = false;
					ncp.cpc.snappedPos = vector2 - vector * 0.45f;
					ncp.transform.position = ncp.cpc.snappedPos;
					ncp.cpc.snappedNormal = hitInfo.normal;
					ncp.transform.forward = hitInfo.normal * -1f;
					ncp.cpc.snappedTangent = vector;
					ncp.cpc.type = CoverPointCore.Type.HighCornerLeft;
					flag = false;
				}
			}
		}
		if (cpc.type != 0)
		{
			base.transform.forward = cpc.snappedNormal * -1f;
		}
		if (!NewCoverPointManager.ClearToShootOver(cpc.snappedPos))
		{
			if (cpc.type == CoverPointCore.Type.ShootOver)
			{
				cpc.type = CoverPointCore.Type.HighWall;
			}
		}
		else if (cpc.type == CoverPointCore.Type.HighCornerLeft)
		{
			if (ncp != null)
			{
				ncp.cpc.type = CoverPointCore.Type.ShootOver;
				ncp.cpc.stepOutLeft = true;
			}
			cpc.stepOutRight = true;
			cpc.type = CoverPointCore.Type.ShootOver;
		}
		else if (cpc.type == CoverPointCore.Type.HighCornerRight)
		{
			if (ncp != null)
			{
				ncp.cpc.type = CoverPointCore.Type.ShootOver;
				ncp.cpc.stepOutRight = true;
			}
			cpc.stepOutLeft = true;
			cpc.type = CoverPointCore.Type.ShootOver;
		}
		cpc.snappedPos.y -= 0.3f;
		if (ncp != null)
		{
			if (flag)
			{
				UnityEngine.Object.DestroyImmediate(ncp.gameObject);
			}
			else
			{
				ncp.cpc.snappedPos.y -= 0.3f;
			}
		}
	}

	private void MakeChildCover(ref NewCoverPoint ncp)
	{
		if (!(ncp != null))
		{
			ncp = new GameObject().AddComponent<NewCoverPoint>();
			ncp.isChild = true;
			ncp.transform.parent = base.transform;
		}
	}
}
