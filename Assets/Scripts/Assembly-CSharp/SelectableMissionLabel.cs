using UnityEngine;

[RequireComponent(typeof(SpriteText))]
public class SelectableMissionLabel : HudBlipIcon
{
	private SpriteText mSpriteText;

	private void Awake()
	{
		mSpriteText = base.gameObject.GetComponent<SpriteText>();
	}

	public override void Start()
	{
		CameraOverride = GlobeSelect.Instance.GlobeCamera;
		base.Start();
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
		SelectableMissionMarker component = Target.gameObject.GetComponent<SelectableMissionMarker>();
		if (component != null)
		{
			SwitchOn();
		}
	}

	public void SetText(string text)
	{
		mSpriteText.Text = text;
		if (text.Contains("Empty"))
		{
			mSpriteText.SetColor(ColourChart.MissionEmpty);
		}
		else if (text.Contains("Test"))
		{
			mSpriteText.SetColor(ColourChart.MissionTest);
		}
		else if (text.Contains("Demo"))
		{
			mSpriteText.SetColor(ColourChart.MissionDemo);
		}
		else
		{
			mSpriteText.SetColor(ColourChart.MissionStory);
		}
	}
}
