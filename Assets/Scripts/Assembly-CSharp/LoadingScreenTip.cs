using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenTip : MonoBehaviour
{
	public SpriteText DisplayText;

	public CommonBackgroundBox Background;

	public List<TipMessage> ToolTipMessages;

	public AnimateCommonBackgroundBox AnimationControl;

	private bool m_bShowMe;

	public void Start()
	{
		if (TBFUtils.IsRetinaHdDevice())
		{
			DisplayText.maxWidth *= 2f;
		}
	}

	public void DoShow()
	{
		m_bShowMe = true;
		StartCoroutine("ShowRoutine");
	}

	private IEnumerator ShowRoutine()
	{
		yield return null;
		if (DisplayText != null)
		{
			bool bValidForContext = false;
			string strMsg = "INVALID STRING";
			do
			{
				int iRandString = Random.Range(0, ToolTipMessages.Count - 1);
				if (ToolTipMessages[iRandString].Availability == TipAvailability.All || (ToolTipMessages[iRandString].Availability == TipAvailability.Campaign && !ActStructure.Instance.CurrentMissionIsSpecOps()) || (ToolTipMessages[iRandString].Availability == TipAvailability.GMG && ActStructure.Instance.CurrentMissionIsSpecOps()))
				{
					string msg = ToolTipMessages[iRandString].Message;
					strMsg = string.Format(Language.GetWithFallback(msg + "_DROID", msg), CommonHelper.HardCurrencySymbol());
					DisplayText.Text = strMsg;
					bValidForContext = true;
				}
			}
			while (!bValidForContext);
			float fLength = DisplayText.GetScreenWidth(strMsg);
			Background.UnitsWide = 1.5f + Mathf.Min(fLength, DisplayText.maxWidth) / 32f;
			Background.ForegroundHeightInUnits = 1f + DisplayText.TotalScreenHeight / 32f;
			DisplayText.transform.position = DisplayText.transform.position + new Vector3(0f, DisplayText.BaseHeight / 4f, 0f);
			if (TBFUtils.IsRetinaHdDevice())
			{
				Background.UnitsWide *= 0.5f;
				Background.ForegroundHeightInUnits *= 0.5f;
			}
			Background.Resize();
		}
		AnimationControl.RecacheVariables();
		AnimationControl.AnimateOpen();
		while (m_bShowMe || !AnimationControl.IsOpen)
		{
			yield return new WaitForEndOfFrame();
		}
		AnimationControl.AnimateClosed();
	}

	public void DoHide()
	{
		m_bShowMe = false;
	}
}
