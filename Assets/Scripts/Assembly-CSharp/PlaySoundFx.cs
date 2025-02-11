using System.Reflection;
using UnityEngine;

public class PlaySoundFx : MonoBehaviour
{
	public SFXBank SoundBank;

	public string SoundFunction;

	public bool PlayAs3D = true;

	public bool PlayOnStart = true;

	public float PlayVolume = 1f;

	public bool DestroyAfterPlay = true;

	private void Start()
	{
		if (PlayOnStart)
		{
			Play();
		}
	}

	public static SoundManager.SoundInstance PlaySfxHelper(GameObject go, SFXBank bank, string function, bool playAs3d, float playVolume)
	{
		SoundFXData sfxData;
		return PlaySfxHelper(go, bank, function, playAs3d, playVolume, out sfxData);
	}

	public static SoundManager.SoundInstance PlaySfxHelper(GameObject go, SFXBank bank, string function, bool playAs3d, float playVolume, out SoundFXData sfxData)
	{
		sfxData = null;
		SoundManager.SoundInstance soundInstance = SoundManager.SoundInstance.Null;
		if (bank == null || function == null || function == string.Empty)
		{
			Debug.LogWarning("playing an empty sfx? - " + go.name);
		}
		else
		{
			PropertyInfo property = bank.GetType().GetProperty("Instance");
			if (property != null)
			{
				SFXBank sFXBank = property.GetValue(null, null) as SFXBank;
				sfxData = sFXBank.GetSFXDataFromName(function);
				soundInstance = ((!playAs3d) ? sfxData.Play2D() : sfxData.Play(go));
				if (soundInstance != null)
				{
					soundInstance.Volume = playVolume;
				}
			}
		}
		return soundInstance;
	}

	public void Play()
	{
		PlaySfxHelper(base.gameObject, SoundBank, SoundFunction, PlayAs3D, PlayVolume);
	}

	public static void StopSfxHelper(GameObject go, SFXBank bank, string function, bool playAs3d)
	{
		if (bank == null || function == null || function == string.Empty)
		{
			Debug.LogWarning("playing an empty sfx? - " + go.name);
			return;
		}
		PropertyInfo property = bank.GetType().GetProperty("Instance");
		if (property != null)
		{
			SFXBank sFXBank = property.GetValue(null, null) as SFXBank;
			SoundFXData sFXDataFromName = sFXBank.GetSFXDataFromName(function);
			if (playAs3d)
			{
				sFXDataFromName.Stop(go);
			}
			else
			{
				sFXDataFromName.Stop2D();
			}
		}
	}

	public void Stop()
	{
		StopSfxHelper(base.gameObject, SoundBank, SoundFunction, PlayAs3D);
		if (DestroyAfterPlay)
		{
			Object.Destroy(this);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon(base.transform.position, "Speaker");
	}

	private void OnDrawGizmosSelected()
	{
		if (SoundBank != null && SoundFunction != null)
		{
			FieldInfo field = SoundBank.GetType().GetField(SoundFunction);
			SoundFXData soundFXData = field.GetValue(SoundBank) as SoundFXData;
			if (soundFXData != null)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(base.transform.position, soundFXData.m_minDistance);
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(base.transform.position, soundFXData.m_maxDistance);
				Gizmos.color = Color.white;
			}
		}
	}
}
