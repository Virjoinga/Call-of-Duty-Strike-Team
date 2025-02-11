using System;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalSequenceCreator : BaseCreator
{
	public static Type[] Types = new Type[2]
	{
		typeof(Condition_InRange),
		typeof(Condition_TargetsDead)
	};

	public Condition m_EditableCondition;

	private ConditionalSequence m_conditionalSequence;

	public override Type[] GetTypes()
	{
		return Types;
	}

	public override List<Condition> GetConditions()
	{
		if (m_conditionalSequence.conditions != null)
		{
			return m_conditionalSequence.conditions;
		}
		return null;
	}

	public override void PopulateItems()
	{
		m_conditionalSequence = GetComponent<ConditionalSequence>();
		if (m_conditionalSequence != null && m_conditionalSequence.conditions.Count > 0)
		{
			m_EditableCondition = m_conditionalSequence.conditions[0];
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
			m_conditionalSequence.conditions.Add(condition);
			SetCurrentItem(m_conditionalSequence.conditions.Count - 1);
		}
		else if (m_HighlightedTaskIndex == -1)
		{
			m_conditionalSequence.conditions.Add(condition);
			SetCurrentItem(m_conditionalSequence.conditions.Count - 1);
		}
		else
		{
			m_conditionalSequence.conditions.Insert(m_HighlightedTaskIndex + 1, condition);
			SetCurrentItem(m_HighlightedTaskIndex + 1);
		}
		condition.hideFlags |= HideFlags.HideInInspector;
	}

	public override void RemoveItem(Condition conditionToRemove)
	{
		m_conditionalSequence.conditions.Remove(conditionToRemove);
		UnityEngine.Object.DestroyImmediate(conditionToRemove);
		m_HighlightedTaskIndex--;
		m_EditableCondition = null;
	}

	public override void SwapItems(int Descriptor1Index, int Descriptor2Index)
	{
		if (m_conditionalSequence.conditions != null && Descriptor1Index < m_conditionalSequence.conditions.Count && Descriptor2Index < m_conditionalSequence.conditions.Count)
		{
			Condition value = m_conditionalSequence.conditions[Descriptor1Index];
			m_conditionalSequence.conditions[Descriptor1Index] = m_conditionalSequence.conditions[Descriptor2Index];
			m_conditionalSequence.conditions[Descriptor2Index] = value;
		}
	}

	public override void SetCurrentItem(int currentDescriptorIndex)
	{
		m_EditableCondition = m_conditionalSequence.conditions[currentDescriptorIndex];
		m_HighlightedTaskIndex = currentDescriptorIndex;
	}

	public override Condition GetCurrentCondition()
	{
		return m_EditableCondition;
	}
}
