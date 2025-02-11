using System;
using UnityEngine;

[Serializable]
public class ObjectiveSpriteText
{
	public PackedSprite TickBox;

	public SpriteText Name;

	public SpriteText Value;

	private string mNameText;

	private string mValueText;

	private MissionObjective.ObjectiveState mState;

	private bool mChangeColours;

	public void Store(string name, string val, MissionObjective.ObjectiveState state, bool colour)
	{
		mNameText = name;
		mValueText = val;
		mState = state;
		mChangeColours = colour;
	}

	public void ShowNow()
	{
		Color color = Color.white;
		if (mChangeColours)
		{
			if (mState == MissionObjective.ObjectiveState.Passed)
			{
				color = ColourChart.ObjectivePassed;
			}
			else if (mState == MissionObjective.ObjectiveState.Failed)
			{
				color = ColourChart.ObjectiveFailed;
			}
		}
		if (Name != null)
		{
			Name.Text = ((mNameText == null) ? string.Empty : mNameText.ToUpper());
			Name.SetColor(color);
		}
		if (Value != null)
		{
			Value.Text = ((mState != MissionObjective.ObjectiveState.Passed) ? string.Empty : mValueText);
			Value.SetColor(color);
		}
		if (TickBox != null)
		{
			TickBox.Hide(mState != MissionObjective.ObjectiveState.Passed);
			TickBox.SetColor(color);
		}
	}

	public void ShowNow(ObjectiveSpriteText other)
	{
		if (Name != null && other.Name != null)
		{
			Name.Text = other.Name.Text;
			Name.SetColor(other.Name.Color);
		}
		if (Value != null && other.Value != null)
		{
			Value.Text = other.Value.Text;
			Value.SetColor(other.Value.Color);
		}
		if (TickBox != null)
		{
			TickBox.Hide(mState != MissionObjective.ObjectiveState.Passed);
			TickBox.SetColor(other.Value.Color);
		}
	}

	public void Clear()
	{
		if (Name != null)
		{
			Name.Text = string.Empty;
		}
		if (Value != null)
		{
			Value.Text = string.Empty;
		}
		if (TickBox != null)
		{
			TickBox.Hide(true);
		}
	}
}
