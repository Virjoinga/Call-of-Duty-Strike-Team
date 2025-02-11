using System.Reflection;
using UnityEngine;

public class FadeSoundFx : MonoBehaviour
{
	public SFXBank SoundBank;

	public string FadeFunction;

	public bool PlayAs3D = true;

	public float Duration;

	public float DesiredVolume = 1f;

	public bool PlayOnStart = true;

	public bool DestroyAfterFade;

	private void Start()
	{
	}

	public static void FadeSfxHelper(GameObject go, SFXBank bank, string function, bool playAs3d, bool createOnStart, float duration, float desiredVolume, bool destroyAfterFade)
	{
		if (bank == null || function == null || function == string.Empty)
		{
			Debug.LogWarning("playing an empty sfx? - " + go.name);
			return;
		}
		PropertyInfo property = bank.GetType().GetProperty("Instance");
		if (property == null)
		{
			return;
		}
		SFXBank sFXBank = property.GetValue(null, null) as SFXBank;
		SoundFXData sFXDataFromName = sFXBank.GetSFXDataFromName(function);
		if (createOnStart)
		{
			SoundManager.SoundInstance @null = SoundManager.SoundInstance.Null;
			@null = ((!playAs3d) ? sFXDataFromName.Play2D() : sFXDataFromName.Play(go));
			if (@null != null)
			{
				@null.Volume = 0f;
			}
		}
		sFXDataFromName.Fade(go, duration, desiredVolume, destroyAfterFade);
	}

	public static void FadeSfxHelper(GameObject go, SoundFXData sfxData, bool playAs3d, bool createOnStart, float duration, float desiredVolume, bool destroyAfterFade)
	{
		if (sfxData == null)
		{
			Debug.LogWarning("playing an empty sfx? - " + go.name);
			return;
		}
		if (createOnStart)
		{
			SoundManager.SoundInstance @null = SoundManager.SoundInstance.Null;
			@null = ((!playAs3d) ? sfxData.Play2D() : sfxData.Play(go));
			if (@null != null)
			{
				@null.Volume = 0f;
			}
		}
		sfxData.Fade(go, duration, desiredVolume, destroyAfterFade);
	}

	public void FadeIn()
	{
		FadeSfxHelper(base.gameObject, SoundBank, FadeFunction, PlayAs3D, PlayOnStart, Duration, DesiredVolume, DestroyAfterFade);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon(base.transform.position, "Speaker");
	}

	private void OnDrawGizmosSelected()
	{
		if (SoundBank != null && FadeFunction != null)
		{
			FieldInfo field = SoundBank.GetType().GetField(FadeFunction);
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
