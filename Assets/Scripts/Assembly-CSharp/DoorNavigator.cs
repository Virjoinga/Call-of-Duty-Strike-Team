using System.Collections.Generic;
using UnityEngine;

public class DoorNavigator : MonoBehaviour
{
	private enum NavigationState
	{
		IdleShut = 0,
		IdleAjar = 1,
		IdleOpen = 2,
		BusyTowards = 3,
		BusyAway = 4,
		BusyClosing = 5
	}

	private const float kLeaveAjarThreshold = 6f;

	private const int kWaitSlots = 3;

	private static List<DoorNavigator> cDoorNavigators = new List<DoorNavigator>();

	public DoorZoneDescriptor descriptor;

	[HideInInspector]
	public GameObject DoorMesh;

	[HideInInspector]
	public BuildingDoor BuildingDoorRef;

	[HideInInspector]
	public Transform setPieceTransform;

	private NavigationState mNavigationState;

	private NavigationState mNextNavState;

	private float busyAwayUntil;

	private float busyTowardsUntil;

	private List<Actor> arrivedList;

	private List<Actor> towardsPending;

	private List<Actor> awayPending;

	private NavGate mNavGate;

	private static List<Vector3> waitSlotOffsets;

	private BookingForm form;

	private Vector3 originalDoorParentPos;

	public static DoorNavigator GetNavigatorFromLayerMask(int mask, Vector3 pos)
	{
		if (mask != 0)
		{
			for (int i = 0; i < cDoorNavigators.Count; i++)
			{
				if ((cDoorNavigators[i].mNavGate.m_Interface.NavLayerID & mask) != 0)
				{
					return cDoorNavigators[i];
				}
			}
		}
		else
		{
			for (int j = 0; j < cDoorNavigators.Count; j++)
			{
				if (cDoorNavigators[j] != null && Vector3.SqrMagnitude(cDoorNavigators[j].mNavGate.transform.position - pos) < 2f)
				{
					return cDoorNavigators[j];
				}
			}
		}
		return null;
	}

	public void CacheDoorPosition()
	{
		originalDoorParentPos = DoorMesh.transform.parent.position;
	}

	private void Awake()
	{
		form = new BookingForm(8);
		mNavigationState = NavigationState.IdleShut;
		mNextNavState = NavigationState.IdleShut;
		arrivedList = new List<Actor>();
		towardsPending = new List<Actor>();
		awayPending = new List<Actor>();
		if (waitSlotOffsets == null)
		{
			waitSlotOffsets = new List<Vector3>();
			waitSlotOffsets.Add(new Vector3(0f, 0f, -1.5f));
			waitSlotOffsets.Add(new Vector3(-1f, 0f, -1.5f));
			waitSlotOffsets.Add(new Vector3(1f, 0f, -1.5f));
			waitSlotOffsets.Add(new Vector3(0f, 0f, 1.5f));
			waitSlotOffsets.Add(new Vector3(-1f, 0f, 1.5f));
			waitSlotOffsets.Add(new Vector3(1f, 0f, 1.5f));
		}
	}

	private void OnDestroy()
	{
		cDoorNavigators.Remove(this);
	}

	private void Start()
	{
		mNavGate = GetComponentInChildren<NavGate>();
		if (descriptor != null && mNavGate != null)
		{
			cDoorNavigators.Add(this);
		}
		setPieceTransform = new GameObject().transform;
		setPieceTransform.parent = mNavGate.transform;
		setPieceTransform.localPosition = Vector3.zero;
		setPieceTransform.localEulerAngles = Vector3.zero;
	}

	private void Update()
	{
		if (mNavigationState < NavigationState.BusyTowards || !(WorldHelper.ThisFrameTime > busyAwayUntil) || !(WorldHelper.ThisFrameTime > busyTowardsUntil))
		{
			return;
		}
		mNavigationState = mNextNavState;
		if (mNextNavState == NavigationState.IdleOpen)
		{
			if (BuildingDoorRef != null)
			{
				BuildingDoorRef.SetState(BuildingDoor.DoorSate.Open);
				BuildingDoorRef.RemoveCMDoor();
			}
		}
		else
		{
			BuildingDoorRef.SetState(BuildingDoor.DoorSate.Closed);
		}
	}

	public bool NavigableBy(Actor a)
	{
		return (a.navAgent.walkableMask & mNavGate.m_Interface.m_NavLayerID) != 0;
	}

	public Vector3 Approach(Actor bc, float eta, Vector3 pos)
	{
		towardsPending.Remove(bc);
		awayPending.Remove(bc);
		form.Cancel(bc);
		form.Book(bc, eta, 0f, null);
		int index;
		if (OpenTowards(pos))
		{
			index = Mathf.Min(towardsPending.Count, 2);
			towardsPending.Add(bc);
		}
		else
		{
			index = Mathf.Min(awayPending.Count, 2) + 3;
			awayPending.Add(bc);
		}
		return mNavGate.transform.position + mNavGate.transform.rotation * waitSlotOffsets[index];
	}

	public bool Manipulable()
	{
		return !DoorIsLockedInPosition();
	}

	public void Arrive(Actor bc)
	{
		if (!arrivedList.Contains(bc))
		{
			arrivedList.Add(bc);
			form.Cancel(bc);
		}
	}

	public bool CanRunThrough(Vector3 pos)
	{
		if ((mNavigationState == NavigationState.IdleOpen || mNavigationState == NavigationState.IdleAjar) && form.FirstBookingTime() == 0f && arrivedList.Count == 0)
		{
			return true;
		}
		if (mNavigationState == NavigationState.IdleShut && !OpenTowards(pos))
		{
			return true;
		}
		return false;
	}

	private bool DoorIsLockedInPosition()
	{
		if (BuildingDoorRef == null)
		{
			return false;
		}
		if (BuildingDoorRef.m_Interface.State == BuildingDoor.DoorSate.LockedClosed || BuildingDoorRef.m_Interface.State == BuildingDoor.DoorSate.LockedOpen)
		{
			return true;
		}
		return false;
	}

	public bool TellMeWhatToDo(Actor bc, out SetPieceModule spm, ref bool canRun)
	{
		if (descriptor == null || DoorIsLockedInPosition())
		{
			spm = null;
			return true;
		}
		switch (mNavigationState)
		{
		case NavigationState.IdleShut:
			return ProcessShut(bc, out spm, ref canRun);
		case NavigationState.IdleAjar:
			return ProcessAjar(bc, out spm, ref canRun);
		case NavigationState.IdleOpen:
			return ProcessOpen(bc, out spm, ref canRun);
		case NavigationState.BusyAway:
			return ProcessBusyAway(bc, out spm, ref canRun);
		case NavigationState.BusyTowards:
			return ProcessBusyTowards(bc, out spm, ref canRun);
		default:
			spm = null;
			canRun = false;
			return false;
		}
	}

	private void Busy(Actor bc, Vector2 delay, NavigationState busyState, NavigationState nextState)
	{
		form.Cancel(bc);
		arrivedList.Remove(bc);
		towardsPending.Remove(bc);
		awayPending.Remove(bc);
		busyAwayUntil = WorldHelper.ThisFrameTime + delay.x;
		busyTowardsUntil = WorldHelper.ThisFrameTime + delay.y;
		mNavigationState = busyState;
		mNextNavState = nextState;
	}

	private bool ProcessShut(Actor bc, out SetPieceModule spm, ref bool canRun)
	{
		bool flag = GameController.Instance.IsFirstPerson && bc == GameController.Instance.mFirstPersonActor;
		if (bc.behaviour.PlayerControlled)
		{
			if (OpenTowards(bc.transform.position))
			{
				spm = ((!flag) ? descriptor.PlayerOpenTowardsNormal : descriptor.PlayerOpenTowardsNormal_FPP);
				Busy(bc, descriptor.delay_PlayerOpenTowardsNormal, NavigationState.BusyTowards, NavigationState.IdleOpen);
				setPieceTransform.localEulerAngles = Vector3.zero;
				PositionSetPiece(descriptor.offset_PlayerOpenTowardsNormal);
			}
			else
			{
				spm = ((!flag) ? descriptor.PlayerOpenAwayNormal : descriptor.PlayerOpenAwayNormal_FPP);
				Busy(bc, descriptor.delay_PlayerOpenAwayNormal, NavigationState.BusyAway, NavigationState.IdleOpen);
				setPieceTransform.localEulerAngles = new Vector3(0f, 180f, 0f);
				PositionSetPiece(descriptor.offset_PlayerOpenAwayNormal);
			}
			BuildingDoorRef.SetState(BuildingDoor.DoorSate.Opening);
			return true;
		}
		if (bc.behaviour.alertState > BehaviourController.AlertState.Reacting)
		{
			BuildingDoorRef.SetState(BuildingDoor.DoorSate.Opening);
			if (bc.baseCharacter.IsRunning())
			{
				if (OpenTowards(bc.transform.position))
				{
					spm = descriptor.SP_FlingOpenTowards;
					Busy(bc, descriptor.delay_FlingOpenTowards, NavigationState.BusyAway, NavigationState.IdleOpen);
					setPieceTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
					PositionSetPiece(descriptor.offset_FlingOpenTowards);
				}
				else
				{
					spm = descriptor.SP_BurstThrough;
					Busy(bc, descriptor.delay_BurstThrough, NavigationState.BusyAway, NavigationState.IdleOpen);
					setPieceTransform.localEulerAngles = new Vector3(0f, 180f, 0f);
					PositionSetPiece(descriptor.offset_BurstThrough);
				}
			}
			else if (OpenTowards(bc.transform.position))
			{
				spm = ((!flag) ? descriptor.PlayerOpenTowardsNormal : descriptor.PlayerOpenTowardsNormal_FPP);
				Busy(bc, descriptor.delay_PlayerOpenTowardsNormal, NavigationState.BusyTowards, NavigationState.IdleOpen);
				setPieceTransform.localEulerAngles = Vector3.zero;
				PositionSetPiece(descriptor.offset_PlayerOpenTowardsNormal);
			}
			else
			{
				spm = ((!flag) ? descriptor.PlayerOpenAwayNormal : descriptor.PlayerOpenAwayNormal_FPP);
				Busy(bc, descriptor.delay_PlayerOpenAwayNormal, NavigationState.BusyAway, NavigationState.IdleOpen);
				setPieceTransform.localEulerAngles = new Vector3(0f, 180f, 0f);
				PositionSetPiece(descriptor.offset_PlayerOpenAwayNormal);
			}
			return true;
		}
		canRun = false;
		bool flag2 = false;
		if (arrivedList.Count > 1)
		{
			flag2 = true;
		}
		else if (form.FirstBookingTime() - WorldHelper.ThisFrameTime < 6f)
		{
			flag2 = true;
		}
		if (flag2)
		{
			if (OpenTowards(bc.transform.position))
			{
				spm = descriptor.SP_OpenTowardsLeaveAjar;
				Busy(bc, descriptor.delay_OpenTowardsLeaveAjar, NavigationState.BusyTowards, NavigationState.IdleAjar);
				setPieceTransform.localEulerAngles = Vector3.zero;
				PositionSetPiece(descriptor.offset_OpenTowardsLeaveAjar);
			}
			else
			{
				spm = descriptor.SP_OpenAwayLeaveAjar;
				Busy(bc, descriptor.delay_OpenAwayLeaveAjar, NavigationState.BusyAway, NavigationState.IdleAjar);
				setPieceTransform.localEulerAngles = new Vector3(0f, 180f, 0f);
				PositionSetPiece(descriptor.offset_OpenAwayLeaveAjar);
			}
		}
		else if (OpenTowards(bc.transform.position))
		{
			spm = descriptor.SP_OpenTowardsAndClose;
			Busy(bc, descriptor.delay_OpenTowardsAndClose, NavigationState.BusyClosing, NavigationState.IdleShut);
			setPieceTransform.localEulerAngles = Vector3.zero;
			PositionSetPiece(descriptor.offset_OpenTowardsAndClose);
		}
		else
		{
			spm = descriptor.SP_OpenAwayAndClose;
			Busy(bc, descriptor.delay_OpenAwayAndClose, NavigationState.BusyClosing, NavigationState.IdleShut);
			setPieceTransform.localEulerAngles = new Vector3(0f, 180f, 0f);
			PositionSetPiece(descriptor.offset_OpenAwayAndClose);
		}
		return true;
	}

	private bool ProcessAjar(Actor bc, out SetPieceModule spm, ref bool canRun)
	{
		bool flag = false;
		if (arrivedList.Count > 1)
		{
			flag = true;
		}
		else if (form.FirstBookingTime() - WorldHelper.ThisFrameTime < 6f)
		{
			flag = true;
		}
		if (flag)
		{
			spm = null;
			if (OpenTowards(bc.transform.position))
			{
				Busy(bc, descriptor.delay_WalkThroughTowardsLeaveAjar, NavigationState.BusyTowards, NavigationState.IdleAjar);
			}
			else
			{
				Busy(bc, descriptor.delay_WalkThroughAwayLeaveAjar, NavigationState.BusyAway, NavigationState.IdleAjar);
			}
			return true;
		}
		if (OpenTowards(bc.transform.position))
		{
			spm = descriptor.SP_WalkThroughAndCloseTowards;
			Busy(bc, descriptor.delay_WalkThroughAndCloseTowards, NavigationState.BusyClosing, NavigationState.IdleShut);
			setPieceTransform.localEulerAngles = Vector3.zero;
			PositionSetPiece(descriptor.offset_WalkThroughAndCloseTowards);
		}
		else
		{
			spm = descriptor.SP_WalkThroughAndCloseAway;
			Busy(bc, descriptor.delay_WalkThroughAndCloseAway, NavigationState.BusyClosing, NavigationState.IdleShut);
			setPieceTransform.localEulerAngles = new Vector3(0f, 180f, 0f);
			PositionSetPiece(descriptor.offset_WalkThroughAndCloseAway);
		}
		return true;
	}

	private bool ProcessOpen(Actor bc, out SetPieceModule spm, ref bool canRun)
	{
		spm = null;
		return true;
	}

	private bool ProcessBusyAway(Actor bc, out SetPieceModule spm, ref bool canRun)
	{
		if (OpenTowards(bc.transform.position))
		{
			spm = null;
			return false;
		}
		if (busyAwayUntil < WorldHelper.ThisFrameTime)
		{
			canRun = false;
			return ProcessAjar(bc, out spm, ref canRun);
		}
		spm = null;
		return false;
	}

	private bool ProcessBusyTowards(Actor bc, out SetPieceModule spm, ref bool canRun)
	{
		if (!OpenTowards(bc.transform.position))
		{
			spm = null;
			return false;
		}
		if (busyTowardsUntil < WorldHelper.ThisFrameTime)
		{
			canRun = false;
			return ProcessAjar(bc, out spm, ref canRun);
		}
		spm = null;
		return false;
	}

	private bool OpenTowards(Vector3 pos)
	{
		return Vector3.Dot(mNavGate.transform.forward, pos - mNavGate.transform.position) < 0f;
	}

	public void PermanentlyOpen()
	{
		mNavigationState = NavigationState.IdleOpen;
	}

	private void PositionSetPiece(Vector2 offset)
	{
		Vector3 zero = Vector3.zero;
		if (offset.x != 0f)
		{
			zero.x = 0.6181334f - offset.x;
		}
		zero.z = 0.01966543f - offset.y;
		setPieceTransform.localPosition = zero;
		Vector3 position = DoorMesh.transform.position;
		DoorMesh.transform.parent.position = originalDoorParentPos + DoorMesh.transform.parent.right * zero.x + DoorMesh.transform.parent.forward * zero.z;
		DoorMesh.transform.position = position;
	}
}
