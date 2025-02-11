using System.Collections;
using System.Reflection;

public class PlaySoundFXCommand : Command
{
	public PlaySoundFx Target;

	public float Volume = 1f;

	public bool ShowSubs;

	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		Target.PlayVolume = Volume;
		Target.Play();
		if (!ShowSubs)
		{
			yield break;
		}
		SFXBank bank = Target.SoundBank;
		PropertyInfo mi = bank.GetType().GetProperty("Instance");
		SFXBank bnk = mi.GetValue(null, null) as SFXBank;
		SoundFXData sfxData = bnk.GetSFXDataFromName(Target.SoundFunction);
		if (sfxData.m_audioSourceData != null && sfxData.m_audioSourceData.Count > 0 && sfxData.m_audioSourceData[0] != null)
		{
			string stringkey2 = sfxData.m_audioSourceData[0].name;
			if (stringkey2.StartsWith("A_"))
			{
				stringkey2 = "S_" + stringkey2.Substring(2, stringkey2.Length - 2);
				BlackBarsController.Instance.DisplaySubtitle(stringkey2, sfxData.m_audioSourceData[0].length);
			}
		}
	}
}
