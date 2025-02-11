using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SequenceCreator : BaseCreator
{
	public Type[] Types = GetCommandTypes().ToArray();

	public Command m_EditableCommand;

	private ScriptedSequence m_ScriptedSequence;

	public override Type[] GetTypes()
	{
		return Types;
	}

	public override List<Command> GetCommands()
	{
		if (m_ScriptedSequence == null)
		{
			return null;
		}
		if (m_ScriptedSequence.Commands != null)
		{
			return m_ScriptedSequence.Commands;
		}
		return null;
	}

	public override void PopulateItems()
	{
		m_ScriptedSequence = base.transform.GetComponentInChildren<ScriptedSequence>();
		if (!(m_ScriptedSequence != null))
		{
			return;
		}
		if (m_ScriptedSequence.Commands != null && m_ScriptedSequence.Commands.Count > 0)
		{
			m_EditableCommand = m_ScriptedSequence.Commands[0];
		}
		Command[] components = GetComponents<Command>();
		Command[] array = components;
		foreach (Command item in array)
		{
			if (!m_ScriptedSequence.Commands.Contains(item))
			{
				m_ScriptedSequence.Commands.Add(item);
			}
		}
	}

	public override void SetItemVisibility()
	{
		if (!(base.transform.parent != null))
		{
			return;
		}
		Command[] componentsInChildren = base.transform.parent.GetComponentsInChildren<Command>();
		Command[] array = componentsInChildren;
		foreach (Command command in array)
		{
			if (command.GetType() != typeof(ScriptedSequence))
			{
				command.hideFlags |= HideFlags.HideInInspector;
			}
			else
			{
				command.hideFlags &= ~HideFlags.HideInInspector;
			}
		}
	}

	public override void AddItem(int commandTypeIndex)
	{
		Command command = base.gameObject.AddComponent(GetTypes()[commandTypeIndex]) as Command;
		if (GetCurrentCommand() == null)
		{
			m_ScriptedSequence.Commands.Add(command);
			SetCurrentItem(m_ScriptedSequence.Commands.Count - 1);
		}
		else if (m_HighlightedTaskIndex == -1)
		{
			m_ScriptedSequence.Commands.Add(command);
			SetCurrentItem(m_ScriptedSequence.Commands.Count - 1);
		}
		else
		{
			m_ScriptedSequence.Commands.Insert(m_HighlightedTaskIndex + 1, command);
			SetCurrentItem(m_HighlightedTaskIndex + 1);
		}
		UpdateParent();
		command.hideFlags |= HideFlags.HideInInspector;
	}

	public override void RemoveItem(Command commandToRemove)
	{
		m_ScriptedSequence.Commands.Remove(commandToRemove);
		UnityEngine.Object.DestroyImmediate(commandToRemove);
		m_HighlightedTaskIndex--;
		UpdateParent();
		m_EditableCommand = null;
	}

	public override void SwapItems(int Command1Index, int Command2Index)
	{
		if (m_ScriptedSequence.Commands != null && Command1Index < m_ScriptedSequence.Commands.Count && Command2Index < m_ScriptedSequence.Commands.Count)
		{
			Command value = m_ScriptedSequence.Commands[Command1Index];
			m_ScriptedSequence.Commands[Command1Index] = m_ScriptedSequence.Commands[Command2Index];
			m_ScriptedSequence.Commands[Command2Index] = value;
			UpdateParent();
		}
	}

	public override void SetCurrentItem(int currentCommandIndex)
	{
		m_EditableCommand = m_ScriptedSequence.Commands[currentCommandIndex];
		m_HighlightedTaskIndex = currentCommandIndex;
	}

	public override Command GetCurrentCommand()
	{
		return m_EditableCommand;
	}

	public void UpdateParent()
	{
		ScriptedCutsceneOverride[] array = UnityEngine.Object.FindObjectsOfType(typeof(ScriptedCutsceneOverride)) as ScriptedCutsceneOverride[];
		ScriptedCutsceneOverride[] array2 = array;
		foreach (ScriptedCutsceneOverride scriptedCutsceneOverride in array2)
		{
			if (scriptedCutsceneOverride.m_OverrideData.Commands.Contains(m_ScriptedSequence.gameObject))
			{
				Container component = scriptedCutsceneOverride.GetComponent<Container>();
				if (component != null)
				{
					scriptedCutsceneOverride.ApplyOverride(component);
				}
			}
		}
	}

	private static List<Type> GetCommandTypes()
	{
		List<Type> list = new List<Type>();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (!type.IsAbstract && typeof(Command).IsAssignableFrom(type))
				{
					list.Add(type);
				}
			}
		}
		list.Sort((Type x, Type y) => StringComparer.InvariantCultureIgnoreCase.Compare(x.Name, y.Name));
		return list;
	}
}
