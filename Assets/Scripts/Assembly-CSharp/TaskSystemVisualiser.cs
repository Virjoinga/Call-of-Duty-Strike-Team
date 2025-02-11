using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskSystemVisualiser : MonoBehaviour
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		SquadOnly = 1,
		NPCEnemiesOnly = 2,
		NPCFriendliesOnly = 4,
		TaskDescriptorIssue = 8
	}

	public static TaskSystemVisualiser instance;

	private static int BUTTON_INDENT_WIDTH = 50;

	private static int BUTTON_INDENT_WIDTH_TOP_ROW = 20;

	private static int BUTTON_WIDTH = 130;

	private static int BUTTON_HEIGHT = 25;

	private bool mHidden;

	private Flags mFlags;

	public static TaskSystemVisualiser Instance()
	{
		return instance;
	}

	private void Awake()
	{
		UnityEngine.Object.Destroy(this);
	}

	private void Start()
	{
		mHidden = true;
	}

	private void OnGUI()
	{
		if (mHidden)
		{
			return;
		}
		Actor[] array = UnityEngine.Object.FindObjectsOfType(typeof(Actor)) as Actor[];
		string text = "Task System Visualiser";
		int num = 600;
		Rect rect = new Rect(Screen.width - 700, Screen.height - (num + 20), 660f, num);
		GUI.Box(rect, text);
		Rect position = new Rect(Screen.width - 700, Screen.height - num, 660f, num);
		string text2 = string.Format("Global Actor Count = {0}", array.Length);
		GUI.Label(position, text2);
		float num2 = (float)BUTTON_HEIGHT * 0.5f;
		if (DoButton(rect.x + (float)BUTTON_INDENT_WIDTH_TOP_ROW, rect.y + (float)BUTTON_HEIGHT * 1.5f, BUTTON_WIDTH, BUTTON_HEIGHT, "Squad Only"))
		{
			if ((mFlags & Flags.SquadOnly) == 0)
			{
				mFlags = Flags.SquadOnly;
			}
			else
			{
				mFlags &= ~Flags.SquadOnly;
			}
		}
		if (DoButton(rect.x + (float)(BUTTON_INDENT_WIDTH_TOP_ROW * 2) + 120f, rect.y + (float)BUTTON_HEIGHT * 1.5f, BUTTON_WIDTH, BUTTON_HEIGHT, "Enemy NPCs Only"))
		{
			if ((mFlags & Flags.NPCEnemiesOnly) == 0)
			{
				mFlags = Flags.NPCEnemiesOnly;
			}
			else
			{
				mFlags &= ~Flags.NPCEnemiesOnly;
			}
		}
		if (DoButton(rect.x + (float)(BUTTON_INDENT_WIDTH_TOP_ROW * 3) + 240f, rect.y + (float)BUTTON_HEIGHT * 1.5f, BUTTON_WIDTH, BUTTON_HEIGHT, "Friendly NPCs Only"))
		{
			if ((mFlags & Flags.NPCFriendliesOnly) == 0)
			{
				mFlags = Flags.NPCFriendliesOnly;
			}
			else
			{
				mFlags &= ~Flags.NPCFriendliesOnly;
			}
		}
		if (DoButton(rect.x + (float)(BUTTON_INDENT_WIDTH_TOP_ROW * 4) + 360f, rect.y + (float)BUTTON_HEIGHT * 1.5f, (int)((float)BUTTON_WIDTH * 1.5f), BUTTON_HEIGHT, "TaskDescriptor Test"))
		{
			if ((mFlags & Flags.TaskDescriptorIssue) == 0)
			{
				mFlags = Flags.TaskDescriptorIssue;
			}
			else
			{
				mFlags &= ~Flags.TaskDescriptorIssue;
			}
		}
		if ((mFlags & Flags.TaskDescriptorIssue) != 0)
		{
			GameObject gameObject = GameObject.Find("GameEventMarker");
			MoveToDescriptor component = gameObject.GetComponent<MoveToDescriptor>();
			if (!(component != null))
			{
				return;
			}
			array = UnityEngine.Object.FindObjectsOfType(typeof(Actor)) as Actor[];
			for (int i = 0; i < array.Length; i++)
			{
				Actor actor = array[i];
				if (DoButton(rect.x + 5f, rect.y + (float)BUTTON_HEIGHT * 1.5f * (float)(3 + i), BUTTON_WIDTH, BUTTON_HEIGHT, actor.realCharacter.name))
				{
					component.CreateTask(actor.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.DenyPlayerInput);
				}
			}
			return;
		}
		num2 += (float)BUTTON_HEIGHT;
		array = UnityEngine.Object.FindObjectsOfType(typeof(Actor)) as Actor[];
		Actor[] array2 = array;
		foreach (Actor actor2 in array2)
		{
			if (((mFlags & Flags.SquadOnly) != 0 && !actor2.behaviour.PlayerControlled) || ((mFlags & Flags.NPCEnemiesOnly) != 0 && (actor2.behaviour.PlayerControlled || actor2.awareness.faction == FactionHelper.Category.Player)) || ((mFlags & Flags.NPCFriendliesOnly) != 0 && (actor2.behaviour.PlayerControlled || actor2.awareness.faction != 0)))
			{
				continue;
			}
			if (DoButton(rect.x + 5f, rect.y + (float)BUTTON_HEIGHT * 1.5f + num2, 40f, BUTTON_HEIGHT, "Invul."))
			{
				actor2.health.Invulnerable = !actor2.health.Invulnerable;
			}
			Rect rect2 = rect;
			rect2.x += BUTTON_INDENT_WIDTH;
			rect2.y += (float)BUTTON_HEIGHT * 1.5f + num2;
			rect2.width -= BUTTON_INDENT_WIDTH * 2;
			rect2.height = BUTTON_HEIGHT;
			string text3 = string.Format("{0}: Sprsd={1} Hlth={2} [{3}] Mov={4} Sht={5} Cvr={6} KD={7}", actor2.realCharacter.name, actor2.behaviour.Suppressed, (!actor2.health.Invulnerable) ? actor2.health.Health.ToString() : "INVUL", actor2.health.TakeDamageModifier, actor2.realCharacter.IsMoving(), actor2.weapon.IsFiring(), actor2.awareness.isInCover, actor2.realCharacter.IsKnockedDown());
			GUI.Label(rect2, text3);
			num2 += (float)BUTTON_HEIGHT;
			foreach (Task item in actor2.tasks.LongTerm.ParallelStack)
			{
				DrawTaskDebugInfo(rect, rect2, num2, item, "Long Term(Parallel)");
				num2 += (float)BUTTON_HEIGHT;
			}
			foreach (Task item2 in actor2.tasks.LongTerm.Stack)
			{
				DrawTaskDebugInfo(rect, rect2, num2, item2, "Long Term");
				num2 += (float)BUTTON_HEIGHT;
			}
			foreach (Task item3 in actor2.tasks.Immediate.ParallelStack)
			{
				DrawTaskDebugInfo(rect, rect2, num2, item3, "Immediate(Parallel)");
				num2 += (float)BUTTON_HEIGHT;
			}
			foreach (Task item4 in actor2.tasks.Immediate.Stack)
			{
				DrawTaskDebugInfo(rect, rect2, num2, item4, "Immediate");
				num2 += (float)BUTTON_HEIGHT;
			}
			foreach (Task item5 in actor2.tasks.Reactive.ParallelStack)
			{
				DrawTaskDebugInfo(rect, rect2, num2, item5, "Reactive(Parallel)");
				num2 += (float)BUTTON_HEIGHT;
			}
			foreach (Task item6 in actor2.tasks.Reactive.Stack)
			{
				DrawTaskDebugInfo(rect, rect2, num2, item6, "Reactive");
				num2 += (float)BUTTON_HEIGHT;
			}
			num2 -= (float)BUTTON_HEIGHT;
			rect2 = rect;
			rect2.x += 2f;
			rect2.y += (float)BUTTON_HEIGHT * 1.5f + num2;
			rect2.width -= BUTTON_INDENT_WIDTH * 2;
			rect2.height = BUTTON_HEIGHT;
			string text4 = "------------->";
			GUI.Label(rect2, text4);
			num2 += (float)BUTTON_HEIGHT;
		}
	}

	public void Hide()
	{
		mHidden = true;
	}

	public void Show()
	{
		mHidden = false;
	}

	public void Toggle()
	{
		mHidden = !mHidden;
	}

	public void GLDebugVisualise()
	{
		GameplayController gameplayController = GameplayController.Instance();
		if (!(gameplayController != null))
		{
			return;
		}
		foreach (Actor item in gameplayController.Selected)
		{
			GLDebugVisualiseTaskStack(item.tasks.Reactive.Stack);
			GLDebugVisualiseTaskStack(item.tasks.Immediate.Stack);
			GLDebugVisualiseTaskStack(item.tasks.LongTerm.Stack);
		}
	}

	private void GLDebugVisualiseTaskStack(List<Task> taskStack)
	{
	}

	private void DrawTaskDebugInfo(Rect visualiserRectangle, Rect labelRectangle, float yOffset, Task task, string taskLabel)
	{
		labelRectangle = visualiserRectangle;
		labelRectangle.x += BUTTON_INDENT_WIDTH;
		labelRectangle.y += (float)BUTTON_HEIGHT * 1.5f + yOffset;
		labelRectangle.width -= BUTTON_INDENT_WIDTH * 2;
		labelRectangle.height = BUTTON_HEIGHT;
		string text = string.Format("   {0}: {1} {2}", taskLabel, task.GetType().ToString(), GetTaskExtraInfo(task));
		GUI.Label(labelRectangle, text);
	}

	private bool DoButton(float x, float y, float width, float height, string label)
	{
		Rect position = new Rect(x, y, width, height);
		return GUI.Button(position, label);
	}

	private string GetTaskExtraInfo(Task task)
	{
		TaskRoutine taskRoutine = task as TaskRoutine;
		if (taskRoutine == null)
		{
			return string.Empty;
		}
		if (taskRoutine.NoneCombatAI)
		{
			return "[Combat AI Disabled]";
		}
		return "[Combat AI Enabled]";
	}
}
