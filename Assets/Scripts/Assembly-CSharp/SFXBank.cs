using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SFXBank : SingletonMonoBehaviour
{
	private Dictionary<string, SoundFXData> mSfxDictionary;

	protected override void Awake()
	{
		BuildDictionary();
		ScanForAdditionalVariations();
		base.Awake();
	}

	protected override void OnDestroy()
	{
		StopAllSounds(true);
		FieldInfo[] fields = GetType().GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.FieldType != typeof(SoundFXData))
			{
				continue;
			}
			SoundFXData soundFXData = fieldInfo.GetValue(this) as SoundFXData;
			if (soundFXData == null)
			{
				continue;
			}
			foreach (AudioClip audioSourceDatum in soundFXData.m_audioSourceData)
			{
				Resources.UnloadAsset(audioSourceDatum);
			}
			soundFXData = null;
		}
		mSfxDictionary.Clear();
		base.OnDestroy();
	}

	public bool CheckSFXBankName(string sfxName)
	{
		if (base.name == sfxName || base.name == sfxName + "(Clone)")
		{
			return true;
		}
		return false;
	}

	private void ScanForAdditionalVariations()
	{
		if (!OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.GByteDevice))
		{
			return;
		}
		string text = base.name;
		if (!text.Contains("HighSFX"))
		{
			text = text.Replace("SFX", "HighSFX");
			if (text.Contains("(Clone)"))
			{
				text = text.Replace("(Clone)", string.Empty);
			}
			SFXBank sFXBank = Resources.Load("Game Prefabs/" + text, typeof(SFXBank)) as SFXBank;
			if (sFXBank != null)
			{
				AppendExtraVariationsToDictionary(sFXBank);
			}
		}
	}

	public void CreateBuildDictionary()
	{
		BuildDictionary();
	}

	public void AppendExtraVariationsToDictionary(SFXBank extraHighSFX)
	{
		if (mSfxDictionary == null)
		{
			return;
		}
		FieldInfo[] fields = GetType().GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.FieldType != typeof(SoundFXData))
			{
				continue;
			}
			FieldInfo[] fields2 = extraHighSFX.GetType().GetFields();
			FieldInfo[] array2 = fields2;
			foreach (FieldInfo fieldInfo2 in array2)
			{
				if (!(fieldInfo.Name == fieldInfo2.Name))
				{
					continue;
				}
				SoundFXData soundFXData = fieldInfo.GetValue(this) as SoundFXData;
				SoundFXData soundFXData2 = fieldInfo2.GetValue(extraHighSFX) as SoundFXData;
				if (soundFXData.m_audioSourceData.Count > 0 && soundFXData2.m_audioSourceData.Count > 0)
				{
					int num = soundFXData.m_audioSourceData.Count;
					for (int k = 0; k < soundFXData2.m_audioSourceData.Count; k++)
					{
						AudioClip item = new AudioClip();
						soundFXData.m_audioSourceData.Add(item);
						soundFXData.m_audioSourceData[num] = soundFXData2.m_audioSourceData[k];
						num++;
					}
					mSfxDictionary.Remove(fieldInfo.Name);
					mSfxDictionary.Add(fieldInfo.Name, soundFXData);
				}
				else
				{
					if (soundFXData.m_audioSourceData.Count == 0)
					{
						Debug.LogWarning(string.Format("AppendExtraVariationsToDictionary: SFXBank = {0}:{1} : Item = {2} : Base bank has no Clips Attached", base.name, extraHighSFX.name, fieldInfo.Name));
					}
					if (soundFXData2.m_audioSourceData.Count == 0)
					{
						Debug.LogWarning(string.Format("AppendExtraVariationsToDictionary: SFXBank = {0}:{1} : Item = {2} : Extra Bank has No Clips Attached", base.name, extraHighSFX.name, fieldInfo.Name));
					}
				}
				break;
			}
		}
	}

	private void BuildDictionary()
	{
		if (mSfxDictionary != null)
		{
			return;
		}
		mSfxDictionary = new Dictionary<string, SoundFXData>();
		FieldInfo[] fields = GetType().GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.FieldType == typeof(SoundFXData))
			{
				SoundFXData value = fieldInfo.GetValue(this) as SoundFXData;
				mSfxDictionary.Add(fieldInfo.Name, value);
			}
		}
	}

	private void StopAllSounds(bool onlyStopLooping)
	{
		FieldInfo[] fields = GetType().GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.FieldType == typeof(SoundFXData))
			{
				SoundFXData soundFXData = fieldInfo.GetValue(this) as SoundFXData;
				if (soundFXData != null && (soundFXData.m_loop || !onlyStopLooping))
				{
					soundFXData.StopAllAndCleanup();
				}
			}
		}
	}

	public SoundFXData GetSFXDataFromName(string name)
	{
		SoundFXData soundFXData = mSfxDictionary[name];
		if (soundFXData == null)
		{
			return null;
		}
		return soundFXData;
	}
}
