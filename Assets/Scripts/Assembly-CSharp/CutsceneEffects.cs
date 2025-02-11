using System;
using UnityEngine;

public class CutsceneEffects : ScriptableObject
{
	public enum EffectAction
	{
		Enable = 0,
		Disable = 1
	}

	[Serializable]
	public class EffectData
	{
		public string EffectName;

		public string EventName;

		public float TimeOffset;

		public string Path;

		public EffectAction Action;
	}

	public AnimationEventData EventData;

	public string[] ObjectsToDisableOnStart;

	public EffectData[] Effects;
}
