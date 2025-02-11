using UnityEngine;

public class NavigationSetPieceLogic : MonoBehaviour
{
	public enum ActionSetPieceType
	{
		kStealthKill = 0,
		kPickupBody = 1,
		kDropBody = 2,
		kPeekEnter = 3,
		kPeekExit = 4,
		kDeathFront = 5,
		kHeal = 6,
		kWalkVent = 7,
		kPlantC4 = 8
	}

	private const float kMantelLowUpDuration = 2.5f;

	private const float kMantleLow1MUpDuration = 2.5f;

	private const float kMantelHighUpDuration = 2.5f;

	private const float kMantelLowDownDuration = 1f;

	private const float kMantleLow1MDownDuration = 1f;

	private const float kMantelHighDownDuration = 1f;

	private const float kVentDuration = 2f;

	private const float kVaultDuration = 1.5f;

	private const float kVaultWindowDuration = 1.5f;

	public SetPieceModule WalkLowMantelUp;

	public SetPieceModule WalkLowMantelDown;

	public SetPieceModule WalkLowMantle1MUp;

	public SetPieceModule WalkLowMantle1MDown;

	public SetPieceModule WalkHighMantelDown;

	public SetPieceModule Walk3MMantelUp;

	public SetPieceModule Walk3MMantelDown;

	public SetPieceModule WalkHighMantelUp;

	public SetPieceModule WalkVent;

	public SetPieceModule WalkVentRev;

	public SetPieceModule MantleOverFromCover;

	public SetPieceModule WalkVault;

	public SetPieceModule WalkVaultRev;

	public SetPieceModule WalkVaultWindow;

	public SetPieceModule WalkVaultWindowRev;

	public SetPieceModule WalkTwoManVault;

	public SetPieceModule StealthKill1;

	public SetPieceModule StealthKill2;

	public SetPieceModule StealthKill3;

	public SetPieceModule StealthKill4;

	public SetPieceModule PickupBody;

	public SetPieceModule DropBody;

	public SetPieceModule PeekEnter;

	public SetPieceModule PeekEnter_FPP;

	public SetPieceModule PeekExit;

	public SetPieceModule PeekExit_FPP;

	public SetPieceModule DeathFront;

	public SetPieceModule Heal;

	public SetPieceModule PlantC4;

	public SetPieceModule PlantC4_FPP;

	public SetPieceLogic CurrentSetPiece;

	private static int[] sStealthKillOrder = new int[20]
	{
		2, 1, 0, 3, 2, 1, 0, 2, 3, 1,
		0, 2, 3, 0, 2, 1, 3, 0, 1, 3
	};

	private static int sStealthKillIndex = -1;

	private Vector3 mPoint1;

	private Vector3 mPoint2;

	private Vector3 mPoint3;

	private Vector3 mEntryPoint;

	private float mDelay;

	private Actor mWorkingActor;

	private float mAgentSpeed;

	private NavigationZone mWorkingNavZone;

	private SetPieceLogic retSetPiece;

	private SetPieceModule SetPieceMod;

	private float mBaseTime;

	private void Start()
	{
		if (sStealthKillIndex == -1)
		{
			sStealthKillIndex = Random.Range(0, 3);
		}
	}

	public SetPieceModule GetActionSetPieceModule(ActionSetPieceType type)
	{
		SetPieceModule setPieceModule = null;
		switch (type)
		{
		case ActionSetPieceType.kStealthKill:
			if (sStealthKillIndex >= sStealthKillOrder.Length)
			{
				sStealthKillIndex = 0;
			}
			switch (sStealthKillOrder[sStealthKillIndex])
			{
			case 0:
				setPieceModule = StealthKill1;
				break;
			case 1:
				setPieceModule = StealthKill2;
				break;
			case 2:
				setPieceModule = StealthKill3;
				break;
			default:
				setPieceModule = StealthKill4;
				break;
			}
			sStealthKillIndex++;
			break;
		case ActionSetPieceType.kPickupBody:
			setPieceModule = PickupBody;
			break;
		case ActionSetPieceType.kDropBody:
			setPieceModule = DropBody;
			break;
		case ActionSetPieceType.kPeekEnter:
			setPieceModule = ((!(GameController.Instance != null) || !GameController.Instance.IsFirstPerson) ? PeekEnter : PeekEnter_FPP);
			break;
		case ActionSetPieceType.kPeekExit:
			setPieceModule = ((!(GameController.Instance != null) || !GameController.Instance.IsFirstPerson) ? PeekExit : PeekExit_FPP);
			break;
		case ActionSetPieceType.kDeathFront:
			setPieceModule = DeathFront;
			break;
		case ActionSetPieceType.kHeal:
			setPieceModule = Heal;
			break;
		case ActionSetPieceType.kWalkVent:
			setPieceModule = WalkVent;
			break;
		case ActionSetPieceType.kPlantC4:
			setPieceModule = ((!(GameController.Instance != null) || !GameController.Instance.IsFirstPerson) ? PlantC4 : PlantC4_FPP);
			break;
		}
		if (setPieceModule == null)
		{
			Debug.Log("Cannot find set piece: " + type);
		}
		return setPieceModule;
	}

	public SetPieceLogic CreateSetPiece()
	{
		CurrentSetPiece = SceneNanny.Instantiate(GetComponent<SetPieceLogic>()) as SetPieceLogic;
		return CurrentSetPiece;
	}

	public SetPieceLogic CreateSetPiece(Actor actor, Vector3 Point1, Vector3 Point2, Vector3 Point3, Vector3 Point4, NavigationZone NavZone, BaseCharacter.MovementStyle gait, float baseTime, out float delay)
	{
		mDelay = 0f;
		retSetPiece = null;
		SetPieceMod = null;
		mPoint1 = Point1;
		mPoint2 = Point2;
		mPoint3 = Point3;
		mBaseTime = baseTime;
		mWorkingNavZone = NavZone;
		mWorkingActor = actor;
		if (NavZone != null)
		{
			mAgentSpeed = actor.realCharacter.GetGaitSpeed(gait);
			switch (NavZone.Type)
			{
			case NavigationZone.NavigationType.MantelLow:
				ChooseSetPiece_MantelLow();
				break;
			case NavigationZone.NavigationType.MantelLowDownOnly:
				ChooseSetPiece_MantelLow();
				break;
			case NavigationZone.NavigationType.MantelLowUpOnly:
				ChooseSetPiece_MantelLow();
				break;
			case NavigationZone.NavigationType.MantleLow1M:
				ChooseSetPiece_MantleLow1M();
				break;
			case NavigationZone.NavigationType.MantleLow1MDownOnly:
				ChooseSetPiece_MantleLow1M();
				break;
			case NavigationZone.NavigationType.MantleLow1MUpOnly:
				ChooseSetPiece_MantleLow1M();
				break;
			case NavigationZone.NavigationType.MantelHigh:
				ChooseSetPiece_MantelHigh();
				break;
			case NavigationZone.NavigationType.MantelHighDownOnly:
				ChooseSetPiece_MantelHigh();
				break;
			case NavigationZone.NavigationType.MantelHighUpOnly:
				ChooseSetPiece_MantelHigh();
				break;
			case NavigationZone.NavigationType.Mantel3M:
				ChooseSetPiece_Mantel3M();
				break;
			case NavigationZone.NavigationType.Mantel3MDownOnly:
				ChooseSetPiece_Mantel3M();
				break;
			case NavigationZone.NavigationType.Mantel3MUpOnly:
				ChooseSetPiece_Mantel3M();
				break;
			case NavigationZone.NavigationType.Vent:
				ChooseSetPiece_Vent();
				break;
			case NavigationZone.NavigationType.VentOpen:
				ChooseSetPiece_Vent();
				break;
			case NavigationZone.NavigationType.VaultLowWall:
				ChooseSetPiece_Vault();
				break;
			case NavigationZone.NavigationType.VaultLowWallOneWay:
				ChooseSetPiece_Vault();
				break;
			case NavigationZone.NavigationType.VaultWindow:
				ChooseSetPiece_VaultWindow();
				break;
			case NavigationZone.NavigationType.VaultWindowOneWay:
				ChooseSetPiece_VaultWindow();
				break;
			}
		}
		if (SetPieceMod != null)
		{
			CurrentSetPiece = SceneNanny.Instantiate(GetComponent<SetPieceLogic>()) as SetPieceLogic;
			CurrentSetPiece.SetModule(SetPieceMod);
			Vector3 pos;
			Quaternion rot;
			NavZone.GetReferencePositionRotation(mEntryPoint, out pos, out rot);
			CurrentSetPiece.PlaceSetPiece(pos, rot);
			CurrentSetPiece.SetActor_IndexOnlyCharacters(0, actor);
			retSetPiece = CurrentSetPiece;
		}
		delay = mDelay;
		return retSetPiece;
	}

	private void ChooseSetPiece_MantelLow()
	{
		if (mWorkingNavZone.MatchLink(mWorkingActor, mBaseTime, mPoint1, mAgentSpeed, GetLowDuration, mPoint2, mPoint3, out mEntryPoint, out mDelay))
		{
			if (mPoint2.y < mPoint3.y)
			{
				SetPieceMod = WalkLowMantelUp;
			}
			else
			{
				SetPieceMod = WalkLowMantelDown;
			}
		}
	}

	private float GetLowDuration(Vector3 point1, Vector3 point2)
	{
		if (point1.y < point2.y)
		{
			return 2.5f;
		}
		return 1f;
	}

	private void ChooseSetPiece_MantleLow1M()
	{
		if (mWorkingNavZone.MatchLink(mWorkingActor, mBaseTime, mPoint1, mAgentSpeed, GetLow1MDuration, mPoint2, mPoint3, out mEntryPoint, out mDelay))
		{
			if (mPoint2.y < mPoint3.y)
			{
				SetPieceMod = WalkLowMantle1MUp;
			}
			else
			{
				SetPieceMod = WalkLowMantle1MDown;
			}
		}
	}

	private float GetLow1MDuration(Vector3 point1, Vector3 point2)
	{
		return (!(point1.y < point2.y)) ? 1f : 2.5f;
	}

	private void ChooseSetPiece_MantelHigh()
	{
		if (mWorkingNavZone.MatchLink(mWorkingActor, mBaseTime, mPoint1, mAgentSpeed, GetHighDuration, mPoint2, mPoint3, out mEntryPoint, out mDelay))
		{
			if (mPoint2.y < mPoint3.y)
			{
				SetPieceMod = WalkHighMantelUp;
			}
			else
			{
				SetPieceMod = WalkHighMantelDown;
			}
		}
	}

	private float GetHighDuration(Vector3 point1, Vector3 point2)
	{
		if (point1.y < point2.y)
		{
			return 2.5f;
		}
		return 1f;
	}

	private void ChooseSetPiece_Mantel3M()
	{
		if (mWorkingNavZone.MatchLink(mWorkingActor, mBaseTime, mPoint1, mAgentSpeed, Get3MDuration, mPoint2, mPoint3, out mEntryPoint, out mDelay))
		{
			if (mPoint2.y < mPoint3.y)
			{
				SetPieceMod = Walk3MMantelUp;
			}
			else
			{
				SetPieceMod = Walk3MMantelDown;
			}
		}
	}

	private float Get3MDuration(Vector3 point1, Vector3 point2)
	{
		return 1f;
	}

	private void ChooseSetPiece_Vent()
	{
		if (mWorkingNavZone.MatchLink(mWorkingActor, mBaseTime, mPoint1, mAgentSpeed, GetVentDuration, mPoint2, mPoint3, out mEntryPoint, out mDelay))
		{
			if (Vector3.Dot(mEntryPoint - mWorkingNavZone.transform.position, mWorkingNavZone.transform.forward) > 0f)
			{
				SetPieceMod = WalkVentRev;
			}
			else
			{
				SetPieceMod = WalkVent;
			}
		}
	}

	private float GetVentDuration(Vector3 point1, Vector3 point2)
	{
		return 2f;
	}

	private void ChooseSetPiece_Vault()
	{
		if (mWorkingNavZone.MatchLink(mWorkingActor, mBaseTime, mPoint1, mAgentSpeed, GetVaultDuration, mPoint2, mPoint3, out mEntryPoint, out mDelay))
		{
			if (Vector3.Dot(mEntryPoint - mWorkingNavZone.transform.position, mWorkingNavZone.transform.forward) > 0f)
			{
				SetPieceMod = WalkVaultRev;
			}
			else
			{
				SetPieceMod = WalkVault;
			}
		}
	}

	private float GetVaultDuration(Vector3 point1, Vector3 point2)
	{
		return 1.5f;
	}

	private void ChooseSetPiece_VaultWindow()
	{
		if (mWorkingNavZone.MatchLink(mWorkingActor, mBaseTime, mPoint1, mAgentSpeed, GetVaultWindowDuration, mPoint2, mPoint3, out mEntryPoint, out mDelay))
		{
			if (Vector3.Dot(mEntryPoint - mWorkingNavZone.transform.position, mWorkingNavZone.transform.forward) > 0f)
			{
				SetPieceMod = WalkVaultWindowRev;
			}
			else
			{
				SetPieceMod = WalkVaultWindow;
			}
		}
	}

	private float GetVaultWindowDuration(Vector3 point1, Vector3 point2)
	{
		return 1.5f;
	}

	public SetPieceLogic CreateFirstPersonSetPiece(Actor actor, NavigationZone NavZone, BaseCharacter.MovementStyle gait, float baseTime, out float delay)
	{
		mDelay = 0f;
		retSetPiece = null;
		SetPieceMod = null;
		mBaseTime = baseTime;
		mEntryPoint = actor.GetPosition();
		mWorkingNavZone = NavZone;
		mWorkingActor = actor;
		if (NavZone != null)
		{
			mAgentSpeed = actor.realCharacter.GetGaitSpeed(gait);
			switch (NavZone.Type)
			{
			case NavigationZone.NavigationType.MantelLow:
				ChooseFirstPersonSetPiece_MantelLow();
				break;
			case NavigationZone.NavigationType.MantelLowDownOnly:
				ChooseFirstPersonSetPiece_MantelLow();
				break;
			case NavigationZone.NavigationType.MantelLowUpOnly:
				ChooseFirstPersonSetPiece_MantelLow();
				break;
			case NavigationZone.NavigationType.MantleLow1M:
				ChooseFirstPersonSetPiece_MantleLow1M();
				break;
			case NavigationZone.NavigationType.MantleLow1MDownOnly:
				ChooseFirstPersonSetPiece_MantleLow1M();
				break;
			case NavigationZone.NavigationType.MantleLow1MUpOnly:
				ChooseFirstPersonSetPiece_MantleLow1M();
				break;
			case NavigationZone.NavigationType.MantelHigh:
				ChooseFirstPersonSetPiece_MantelHigh();
				break;
			case NavigationZone.NavigationType.MantelHighDownOnly:
				ChooseFirstPersonSetPiece_MantelHigh();
				break;
			case NavigationZone.NavigationType.MantelHighUpOnly:
				ChooseFirstPersonSetPiece_MantelHigh();
				break;
			case NavigationZone.NavigationType.Mantel3M:
				ChooseFirstPersonSetPiece_Mantel3M();
				break;
			case NavigationZone.NavigationType.Mantel3MDownOnly:
				ChooseFirstPersonSetPiece_Mantel3M();
				break;
			case NavigationZone.NavigationType.Mantel3MUpOnly:
				ChooseFirstPersonSetPiece_Mantel3M();
				break;
			case NavigationZone.NavigationType.Vent:
				ChooseFirstPersonSetPiece_Vent();
				break;
			case NavigationZone.NavigationType.VentOpen:
				ChooseFirstPersonSetPiece_Vent();
				break;
			case NavigationZone.NavigationType.VaultLowWall:
				ChooseFirstPersonSetPiece_Vault();
				break;
			case NavigationZone.NavigationType.VaultLowWallOneWay:
				ChooseFirstPersonSetPiece_Vault();
				break;
			case NavigationZone.NavigationType.VaultWindow:
				ChooseFirstPersonSetPiece_VaultWindow();
				break;
			case NavigationZone.NavigationType.VaultWindowOneWay:
				ChooseFirstPersonSetPiece_VaultWindow();
				break;
			}
		}
		if (SetPieceMod != null)
		{
			CurrentSetPiece = SceneNanny.Instantiate(GetComponent<SetPieceLogic>()) as SetPieceLogic;
			CurrentSetPiece.SetModule(SetPieceMod);
			Vector3 pos;
			Quaternion rot;
			NavZone.GetReferencePositionRotation(mEntryPoint, out pos, out rot);
			CurrentSetPiece.PlaceSetPiece(pos, rot);
			CurrentSetPiece.SetActor_IndexOnlyCharacters(0, actor);
			retSetPiece = CurrentSetPiece;
		}
		delay = mDelay;
		return retSetPiece;
	}

	private void ChooseFirstPersonSetPiece_MantelLow()
	{
		Vector3 pos;
		Quaternion rot;
		mWorkingNavZone.GetReferencePositionRotation(mEntryPoint, out pos, out rot);
		if (Mathf.Abs(mWorkingActor.GetPosition().y - pos.y) < 0.5f)
		{
			SetPieceMod = WalkLowMantelUp;
		}
		else
		{
			SetPieceMod = WalkLowMantelDown;
		}
	}

	private void ChooseFirstPersonSetPiece_MantleLow1M()
	{
		Vector3 pos;
		Quaternion rot;
		mWorkingNavZone.GetReferencePositionRotation(mEntryPoint, out pos, out rot);
		SetPieceMod = ((!(Mathf.Abs(mWorkingActor.GetPosition().y - pos.y) < 0.5f)) ? WalkLowMantle1MDown : WalkLowMantle1MUp);
	}

	private void ChooseFirstPersonSetPiece_MantelHigh()
	{
		Vector3 pos;
		Quaternion rot;
		mWorkingNavZone.GetReferencePositionRotation(mEntryPoint, out pos, out rot);
		if (Mathf.Abs(mWorkingActor.GetPosition().y - pos.y) < 0.5f)
		{
			SetPieceMod = WalkHighMantelUp;
		}
		else
		{
			SetPieceMod = WalkHighMantelDown;
		}
	}

	private void ChooseFirstPersonSetPiece_Mantel3M()
	{
		Vector3 pos;
		Quaternion rot;
		mWorkingNavZone.GetReferencePositionRotation(mEntryPoint, out pos, out rot);
		if (Mathf.Abs(mWorkingActor.GetPosition().y - pos.y) < 0.5f)
		{
			SetPieceMod = Walk3MMantelUp;
		}
		else
		{
			SetPieceMod = Walk3MMantelDown;
		}
	}

	private void ChooseFirstPersonSetPiece_Vent()
	{
		if (Vector3.Dot(mEntryPoint - mWorkingNavZone.transform.position, mWorkingNavZone.transform.forward) > 0f)
		{
			SetPieceMod = WalkVentRev;
		}
		else
		{
			SetPieceMod = WalkVent;
		}
	}

	private void ChooseFirstPersonSetPiece_Vault()
	{
		if (Vector3.Dot(mEntryPoint - mWorkingNavZone.transform.position, mWorkingNavZone.transform.forward) > 0f)
		{
			SetPieceMod = WalkVaultRev;
		}
		else
		{
			SetPieceMod = WalkVault;
		}
	}

	private void ChooseFirstPersonSetPiece_VaultWindow()
	{
		if (Vector3.Dot(mEntryPoint - mWorkingNavZone.transform.position, mWorkingNavZone.transform.forward) > 0f)
		{
			SetPieceMod = WalkVaultWindowRev;
		}
		else
		{
			SetPieceMod = WalkVaultWindow;
		}
	}
}
