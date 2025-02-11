using UnityEngine;

public class AnimSfxHandler : MonoBehaviour
{
	public SFXList m_sfxList;

	private int GetSfxIndex(string SfxName)
	{
		int result = -1;
		if (m_sfxList != null)
		{
			int num = int.Parse(SfxName.Substring(SfxName.Length - 2)) - 1;
			if (num >= 0 && num < m_sfxList.m_sfx.Length)
			{
				result = num;
			}
		}
		return result;
	}

	private void KeyOn(string SfxName)
	{
		int sfxIndex = GetSfxIndex(SfxName);
		if (sfxIndex != -1)
		{
			SoundManager.Instance.Play(m_sfxList.m_sfx[sfxIndex], base.gameObject);
		}
	}

	private void KeyOff(string SfxName)
	{
		int sfxIndex = GetSfxIndex(SfxName);
		if (sfxIndex != -1)
		{
			SoundManager.Instance.Stop(m_sfxList.m_sfx[sfxIndex], base.gameObject);
		}
	}
}
