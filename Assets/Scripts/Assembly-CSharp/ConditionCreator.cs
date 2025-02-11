using System;
using System.Collections.Generic;
using UnityEngine;

public class ConditionCreator : BaseCreator
{
	public static Type[] Types = new Type[11]
	{
		typeof(Condition_Alive),
		typeof(Condition_All),
		typeof(Condition_AllSpawnedDead),
		typeof(Condition_Dead),
		typeof(Condition_DoorOpen),
		typeof(Condition_EnteredGame),
		typeof(Condition_NotSpawned),
		typeof(Condition_Probability),
		typeof(Condition_Proximity),
		typeof(Condition_Scripted),
		typeof(Condition_Spawned)
	};

	public Condition m_EditableCondition;

	private ConditionalDescriptor m_conditionalDescriptor;

	public override Type[] GetTypes()
	{
		return Types;
	}

	public override List<Condition> GetConditions()
	{
		if (m_conditionalDescriptor.Conditions != null)
		{
			return m_conditionalDescriptor.Conditions;
		}
		return null;
	}

	public override void PopulateItems()
	{
		m_conditionalDescriptor = GetComponent<ConditionalDescriptor>();
		if (m_conditionalDescriptor != null && m_conditionalDescriptor.Conditions.Count > 0)
		{
			m_EditableCondition = m_conditionalDescriptor.Conditions[0];
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
		Condition condition = base.gameObject.AddComponent(Types[descriptorTypeIndex]) as Condition;
		if (GetCurrentDescriptor() == null)
		{
			m_conditionalDescriptor.Conditions.Add(condition);
			SetCurrentItem(m_conditionalDescriptor.Conditions.Count - 1);
		}
		else if (m_HighlightedTaskIndex == -1)
		{
			m_conditionalDescriptor.Conditions.Add(condition);
			SetCurrentItem(m_conditionalDescriptor.Conditions.Count - 1);
		}
		else
		{
			m_conditionalDescriptor.Conditions.Insert(m_HighlightedTaskIndex + 1, condition);
			SetCurrentItem(m_HighlightedTaskIndex + 1);
		}
		condition.hideFlags |= HideFlags.HideInInspector;
	}

	public override void RemoveItem(Condition conditionToRemove)
	{
		m_conditionalDescriptor.Conditions.Remove(conditionToRemove);
		UnityEngine.Object.DestroyImmediate(conditionToRemove);
		m_HighlightedTaskIndex--;
		m_EditableCondition = null;
	}

	public override void SwapItems(int Descriptor1Index, int Descriptor2Index)
	{
		if (m_conditionalDescriptor.Conditions != null && Descriptor1Index < m_conditionalDescriptor.Conditions.Count && Descriptor2Index < m_conditionalDescriptor.Conditions.Count)
		{
			Condition value = m_conditionalDescriptor.Conditions[Descriptor1Index];
			m_conditionalDescriptor.Conditions[Descriptor1Index] = m_conditionalDescriptor.Conditions[Descriptor2Index];
			m_conditionalDescriptor.Conditions[Descriptor2Index] = value;
		}
	}

	public override void SetCurrentItem(int currentDescriptorIndex)
	{
		m_EditableCondition = m_conditionalDescriptor.Conditions[currentDescriptorIndex];
		m_HighlightedTaskIndex = currentDescriptorIndex;
	}

	public override Condition GetCurrentCondition()
	{
		return m_EditableCondition;
	}
}
