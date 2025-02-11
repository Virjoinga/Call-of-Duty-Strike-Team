using UnityEngine;

public abstract class BasePoseModule : BaseActorComponent
{
	public const float sin45 = 0.707f;

	protected int[] mCategoryHandle;

	protected AnimDirector.ActionHandle[] mActionHandle;

	private PoseModuleSharedData.CommandCode[] CommandRemappingTable;

	protected Vector3 modeltransforward
	{
		get
		{
			return myActor.Pose.onAxisTrans.forward;
		}
		set
		{
			myActor.Pose.onAxisTrans.forward = value;
		}
	}

	protected Vector3 modeltransposition
	{
		get
		{
			return myActor.Pose.onAxisTrans.position;
		}
		set
		{
			myActor.Pose.onAxisTrans.position = value;
		}
	}

	protected Vector3 mAimDir
	{
		get
		{
			return myActor.Pose.mAimDir;
		}
		set
		{
			myActor.Pose.mAimDir = value;
		}
	}

	protected float mAimWeight
	{
		get
		{
			return myActor.Pose.mAimWeight;
		}
		set
		{
			myActor.Pose.mAimWeight = value;
		}
	}

	protected float mTargetAimWeight
	{
		get
		{
			return myActor.Pose.mTargetAimWeight;
		}
		set
		{
			myActor.Pose.mTargetAimWeight = value;
		}
	}

	public AnimDirector mAnimDirector
	{
		get
		{
			return myActor.animDirector;
		}
	}

	public GameObject Model
	{
		get
		{
			return myActor.model;
		}
	}

	public abstract PoseModuleSharedData.Modules UpdatePose(Vector3 destination, Vector3 newPos, Vector3 newVel, Vector3 aimDir, ref string newStateStr, bool expensiveTick);

	public abstract PoseModuleSharedData.Modules Command(string com);

	public virtual void OnInactive(PoseModuleSharedData.Modules to)
	{
	}

	public virtual void OnActive(PoseModuleSharedData.Modules fr)
	{
	}

	protected virtual void Internal_Init()
	{
	}

	protected PoseModuleSharedData.CommandCode ParseCommand(string s)
	{
		PoseModuleSharedData.CommandCode value;
		if (s != null && PoseModuleSharedData.mTranslationTable.TryGetValue(s, out value))
		{
			if (CommandRemappingTable != null)
			{
				return CommandRemappingTable[(int)value];
			}
			return value;
		}
		return PoseModuleSharedData.CommandCode.Undefined;
	}

	public virtual Vector3 IdealFirstPersonAngles()
	{
		return new Vector3(0f, myActor.Pose.onAxisTrans.eulerAngles.y, 0f);
	}

	private void CreateRemappingTable()
	{
		CommandRemappingTable = new PoseModuleSharedData.CommandCode[45];
		for (PoseModuleSharedData.CommandCode commandCode = PoseModuleSharedData.CommandCode.CancelCarried; commandCode < PoseModuleSharedData.CommandCode.Count; commandCode++)
		{
			CommandRemappingTable[(int)commandCode] = commandCode;
		}
	}

	protected void Remap(PoseModuleSharedData.CommandCode fr, PoseModuleSharedData.CommandCode to)
	{
		if (CommandRemappingTable == null)
		{
			CreateRemappingTable();
		}
		CommandRemappingTable[(int)fr] = to;
	}

	public BasePoseModule ConnectModule(Actor actor)
	{
		myActor = actor;
		Internal_Init();
		return this;
	}
}
