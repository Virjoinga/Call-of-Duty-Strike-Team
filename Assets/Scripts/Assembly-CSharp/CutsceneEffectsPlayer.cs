using System;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneEffectsPlayer : MonoBehaviour
{
	private class TimedAction
	{
		public float Time { get; private set; }

		public Action Action { get; private set; }

		public TimedAction(float time, Action action)
		{
			Time = time;
			Action = action;
		}
	}

	public Animation AnimationPlayer;

	public CutsceneEffects Effects;

	private float mLastTime;

	private int mNext;

	private List<TimedAction> mActions;

	private AnimationState mCachedState;

	public void Start()
	{
		Dictionary<string, List<float>> dictionary = new Dictionary<string, List<float>>();
		mActions = new List<TimedAction>();
		if (Effects == null)
		{
			Debug.LogWarning("CutsceneEffectsPlayer has no effects file");
			return;
		}
		string[] objectsToDisableOnStart = Effects.ObjectsToDisableOnStart;
		foreach (string path in objectsToDisableOnStart)
		{
			Transform targetFromPath = GetTargetFromPath(path);
			if (targetFromPath != null)
			{
				targetFromPath.gameObject.SetActive(false);
			}
		}
		AnimationEventData.Event[] events = Effects.EventData.Events;
		foreach (AnimationEventData.Event @event in events)
		{
			List<float> value;
			if (!dictionary.TryGetValue(@event.Name, out value))
			{
				value = new List<float>();
				dictionary[@event.Name] = value;
			}
			value.Add(@event.Time);
		}
		CutsceneEffects.EffectData[] effects = Effects.Effects;
		foreach (CutsceneEffects.EffectData effectData in effects)
		{
			List<float> value2 = null;
			Transform target = GetTargetFromPath(effectData.Path);
			if (target == null)
			{
				continue;
			}
			if (effectData.EventName == null || effectData.EventName.Length == 0)
			{
				value2 = new List<float>();
				value2.Add(0f);
			}
			if (value2 == null && !dictionary.TryGetValue(effectData.EventName, out value2))
			{
				continue;
			}
			Action action = null;
			switch (effectData.Action)
			{
			case CutsceneEffects.EffectAction.Enable:
				action = delegate
				{
					if (target != null)
					{
						target.gameObject.SetActive(true);
					}
					else
					{
						Debug.LogWarning("Missing cutscene effects target");
					}
				};
				break;
			case CutsceneEffects.EffectAction.Disable:
				action = delegate
				{
					if (target != null)
					{
						target.gameObject.SetActive(false);
					}
					else
					{
						Debug.LogWarning("Missing cutscene effects target");
					}
				};
				break;
			}
			if (value2 == null)
			{
				continue;
			}
			foreach (float item in value2)
			{
				float num = item;
				float time = num + effectData.TimeOffset;
				mActions.Add(new TimedAction(time, action));
			}
		}
		mActions.Sort((TimedAction a, TimedAction b) => Comparer<float>.Default.Compare(a.Time, b.Time));
		mCachedState = GetAnimationState();
	}

	public void LateUpdate()
	{
		if (!(mCachedState != null) || !mCachedState.enabled)
		{
			return;
		}
		if (mActions != null)
		{
			while (mNext < mActions.Count)
			{
				TimedAction timedAction = mActions[mNext];
				if (mLastTime <= timedAction.Time && timedAction.Time <= mCachedState.time)
				{
					if (timedAction.Action != null)
					{
						timedAction.Action();
					}
					mNext++;
					continue;
				}
				break;
			}
		}
		mLastTime = mCachedState.time;
	}

	private Transform GetTargetFromPath(string path)
	{
		Transform result = null;
		if (path.StartsWith("/"))
		{
			GameObject gameObject = GameObject.Find(path);
			if (gameObject != null)
			{
				result = gameObject.transform;
			}
			else
			{
				GameObject gameObject2 = GameObject.Find("Section_Combined");
				if (gameObject2 != null)
				{
					result = gameObject2.transform.Find(path.Substring(1));
				}
			}
		}
		else
		{
			result = AnimationPlayer.transform.Find(path);
		}
		return result;
	}

	private AnimationState GetAnimationState()
	{
		if (AnimationPlayer != null)
		{
			return AnimationPlayer[AnimationPlayer.clip.name];
		}
		return null;
	}
}
