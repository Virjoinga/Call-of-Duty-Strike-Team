using System;
using UnityEngine;

[Serializable]
public class QuickDestination
{
	public GuidRef goToOnSpawn = new GuidRef();

	public BaseCharacter.MovementStyle gait;

	public bool takeCover;

	public bool holdCover;

	public GuidRef coverCluster = new GuidRef();

	public GuidRef magnet = new GuidRef();

	public string behaviour1;

	public string behaviour2;

	public string behaviour3;

	public void CloneFrom(QuickDestination qd)
	{
		goToOnSpawn.theObject = qd.goToOnSpawn.theObject;
		gait = qd.gait;
		takeCover = qd.takeCover;
		holdCover = qd.holdCover;
		coverCluster.theObject = qd.coverCluster.theObject;
		magnet.theObject = qd.magnet.theObject;
	}

	public void ResolveGuidLinks()
	{
		if (goToOnSpawn != null)
		{
			goToOnSpawn.ResolveLink();
		}
		if (coverCluster != null)
		{
			coverCluster.ResolveLink();
		}
		if (magnet != null)
		{
			magnet.ResolveLink();
		}
	}

	public TaskDescriptor GenerateRoutine(ref RoutineDescriptorData rd, GameObject parent, AssaultParams assaultParams)
	{
		behaviour1 = "No routine generated.";
		behaviour2 = string.Empty;
		behaviour3 = string.Empty;
		rd.Magnet.theObject = magnet.theObject;
		if (goToOnSpawn.theObject == null && magnet.theObject == null && coverCluster.theObject != null && (takeCover || holdCover))
		{
			behaviour1 = "Soldier will be given a TaskOccupyCoverCluster";
			behaviour2 = "( " + coverCluster.theObject.name + " )";
			behaviour3 = "and move between cover points at intervals.";
			OccupyCoverClusterDescriptor occupyCoverClusterDescriptor = parent.AddComponent<OccupyCoverClusterDescriptor>();
			occupyCoverClusterDescriptor.Parameters.mMovementStyle = gait;
			occupyCoverClusterDescriptor.Parameters.coverCluster = coverCluster.GetComponent<CoverCluster>();
			rd.OneShotRoutineTasks = true;
			return occupyCoverClusterDescriptor;
		}
		if (goToOnSpawn.theObject != null && (takeCover || holdCover))
		{
			MoveToCoverDescriptor moveToCoverDescriptor = parent.AddComponent<MoveToCoverDescriptor>();
			if (goToOnSpawn.GetComponent<GeneralArea>() != null)
			{
				behaviour1 = "First go to general area of " + goToOnSpawn.theObject.name + " and take cover.";
			}
			else if (goToOnSpawn.GetComponent<CoverCluster>() != null)
			{
				behaviour1 = "First go to random cover point specified in " + goToOnSpawn.theObject.name + ".";
			}
			else if (goToOnSpawn.GetComponent<NewCoverPoint>() != null)
			{
				behaviour1 = "First go to cover point " + goToOnSpawn.theObject.name + ".";
			}
			else
			{
				behaviour1 = "First take cover near " + goToOnSpawn.theObject.name + ".";
			}
			if (magnet.theObject != null)
			{
				moveToCoverDescriptor.ConfigFlags = new TaskConfigDescriptor();
				moveToCoverDescriptor.ConfigFlags.AbortOnAlert = true;
				behaviour2 = "When alerted, assault " + magnet.theObject.name;
			}
			else
			{
				behaviour2 = "When alerted, enter standard combat AI.";
			}
			moveToCoverDescriptor.Parameters.holdCoverWhenBored = holdCover;
			moveToCoverDescriptor.Parameters.mMovementStyle = gait;
			moveToCoverDescriptor.Target.theObject = goToOnSpawn.theObject;
			moveToCoverDescriptor.Parameters.coverCluster = coverCluster.GetComponent<CoverCluster>();
			if (moveToCoverDescriptor.Parameters.coverCluster != null)
			{
				behaviour3 = "Use only cover points in " + moveToCoverDescriptor.Parameters.coverCluster.name;
			}
			else
			{
				behaviour3 = "Use any available cover points.";
			}
			rd.OneShotRoutineTasks = true;
			return moveToCoverDescriptor;
		}
		if (goToOnSpawn.theObject != null)
		{
			if (goToOnSpawn.GetComponent<GeneralArea>() != null)
			{
				behaviour1 = "Go to general area of " + goToOnSpawn.theObject.name + ".";
			}
			else
			{
				behaviour1 = "First go to " + goToOnSpawn.theObject.name + ".";
			}
			if (magnet.theObject != null)
			{
				behaviour2 = "When alerted, assault " + magnet.theObject.name;
			}
			else
			{
				behaviour2 = "When alerted, enter standard combat AI.";
			}
			MoveToDescriptor moveToDescriptor = parent.AddComponent<MoveToDescriptor>();
			moveToDescriptor.Parameters.mMovementStyle = gait;
			moveToDescriptor.Target = goToOnSpawn.theObject.transform;
			moveToDescriptor.Parameters.coverCluster = coverCluster.GetComponent<CoverCluster>();
			if (moveToDescriptor.Parameters.coverCluster != null)
			{
				behaviour3 = "Use only cover points in " + moveToDescriptor.Parameters.coverCluster.name;
			}
			else
			{
				behaviour3 = "Use any available cover points.";
			}
			rd.OneShotRoutineTasks = true;
			return moveToDescriptor;
		}
		if (magnet.theObject != null)
		{
			if (magnet.GetComponent<GeneralArea>() != null)
			{
				behaviour1 = "Assault general area of " + magnet.theObject.name + ".";
			}
			else
			{
				behaviour1 = "Assault exact location of " + magnet.theObject.name + ".";
			}
			AssaultDescriptor assaultDescriptor = parent.AddComponent<AssaultDescriptor>();
			assaultDescriptor.assaultParams.CopyFrom(assaultParams);
			assaultDescriptor.assaultParams.Target.theObject = magnet.theObject;
			assaultDescriptor.Parameters.mMovementStyle = gait;
			assaultDescriptor.Parameters.coverCluster = coverCluster.GetComponent<CoverCluster>();
			if (assaultDescriptor.Parameters.coverCluster != null)
			{
				behaviour2 = "Use only cover points in " + assaultDescriptor.Parameters.coverCluster.name;
			}
			else
			{
				behaviour2 = "Use any available cover points.";
			}
			rd.OneShotRoutineTasks = true;
			return assaultDescriptor;
		}
		return null;
	}
}
