using System;
using System.Collections.Generic;
using UnityEngine;

public class RoutineCreator : BaseCreator
{
	public static Type[] Types = new Type[26]
	{
		typeof(AssaultDescriptor),
		typeof(CloseDoorDescriptor),
		typeof(ConditionalDescriptor),
		typeof(DefendDescriptor),
		typeof(DropBodyDescriptor),
		typeof(DumbDescriptor),
		typeof(FollowDescriptor),
		typeof(MoveToCoverDescriptor),
		typeof(MoveToDescriptor),
		typeof(OccupyCoverClusterDescriptor),
		typeof(OpenDoorDescriptor),
		typeof(PatrolDescriptor),
		typeof(PlayAnimationDescriptor),
		typeof(PlayRandomAnimationDescriptor),
		typeof(RiotShieldDescriptor),
		typeof(RPGDescriptor),
		typeof(SendMessageDescriptor),
		typeof(SetPieceDescriptor),
		typeof(SniperDescriptor),
		typeof(TetherDescriptor),
		typeof(ThrowGrenadeDescriptor),
		typeof(UseFixedGunDescriptor),
		typeof(WaitDescriptor),
		typeof(WindowLookoutDescriptor),
		typeof(UseAlarmPanelDescriptor),
		typeof(HackDescriptor)
	};

	public TaskDescriptor m_EditableTask;

	private RoutineDescriptor m_RoutineDescriptor;

	public override Type[] GetTypes()
	{
		return Types;
	}

	public override List<TaskDescriptor> GetDescriptors()
	{
		if (m_RoutineDescriptor.Tasks != null)
		{
			return m_RoutineDescriptor.Tasks;
		}
		return null;
	}

	public override void PopulateItems()
	{
		m_RoutineDescriptor = GetComponent<RoutineDescriptor>();
		if (m_RoutineDescriptor != null && m_RoutineDescriptor.Tasks != null && m_RoutineDescriptor.Tasks.Count > 0)
		{
			m_EditableTask = m_RoutineDescriptor.Tasks[0];
		}
	}

	public override void SetItemVisibility()
	{
		TaskDescriptor[] components = GetComponents<TaskDescriptor>();
		TaskDescriptor[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].hideFlags &= ~HideFlags.HideInInspector;
		}
	}

	public override void AddItem(int descriptorTypeIndex)
	{
		TaskDescriptor taskDescriptor = base.gameObject.AddComponent(Types[descriptorTypeIndex]) as TaskDescriptor;
		if (m_RoutineDescriptor.Tasks == null)
		{
			m_RoutineDescriptor.Tasks = new List<TaskDescriptor>();
		}
		if (GetCurrentDescriptor() == null)
		{
			m_RoutineDescriptor.Tasks.Add(taskDescriptor);
			SetCurrentItem(m_RoutineDescriptor.Tasks.Count - 1);
		}
		else if (m_HighlightedTaskIndex == -1)
		{
			m_RoutineDescriptor.Tasks.Add(taskDescriptor);
			SetCurrentItem(m_RoutineDescriptor.Tasks.Count - 1);
		}
		else
		{
			m_RoutineDescriptor.Tasks.Insert(m_HighlightedTaskIndex + 1, taskDescriptor);
			SetCurrentItem(m_HighlightedTaskIndex + 1);
		}
		taskDescriptor.hideFlags |= HideFlags.HideInInspector;
	}

	public override void RemoveItem(TaskDescriptor taskToRemove)
	{
		m_RoutineDescriptor.Tasks.Remove(taskToRemove);
		UnityEngine.Object.DestroyImmediate(taskToRemove);
		m_HighlightedTaskIndex--;
		m_EditableTask = null;
	}

	public override void SwapItems(int Descriptor1Index, int Descriptor2Index)
	{
		if (m_RoutineDescriptor.Tasks != null && Descriptor1Index < m_RoutineDescriptor.Tasks.Count && Descriptor2Index < m_RoutineDescriptor.Tasks.Count)
		{
			TaskDescriptor value = m_RoutineDescriptor.Tasks[Descriptor1Index];
			m_RoutineDescriptor.Tasks[Descriptor1Index] = m_RoutineDescriptor.Tasks[Descriptor2Index];
			m_RoutineDescriptor.Tasks[Descriptor2Index] = value;
		}
	}

	public override void SetCurrentItem(int currentDescriptorIndex)
	{
		m_EditableTask = m_RoutineDescriptor.Tasks[currentDescriptorIndex];
		m_HighlightedTaskIndex = currentDescriptorIndex;
	}

	public override TaskDescriptor GetCurrentDescriptor()
	{
		return m_EditableTask;
	}
}
