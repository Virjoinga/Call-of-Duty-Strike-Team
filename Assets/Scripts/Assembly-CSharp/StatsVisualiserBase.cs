using UnityEngine;

public class StatsVisualiserBase : MonoBehaviour
{
	protected int titleHeight = 20;

	protected int gap = 4;

	protected int buttonWidth = 30;

	protected int buttonHeight = 20;

	protected int xPos = 60;

	protected int yPos = 20;

	protected int width = 200;

	protected int height = 160;

	protected Rect titleRect;

	protected Rect statsRect;

	protected Rect prevRect;

	protected Rect nextRect;

	public virtual void Start()
	{
		titleRect = new Rect(xPos, yPos, width, titleHeight);
		statsRect = new Rect(xPos, yPos + titleHeight + gap, width, height);
		prevRect = new Rect(xPos, statsRect.y + (float)height + (float)gap, buttonWidth, buttonHeight);
		nextRect = new Rect(xPos + buttonWidth + gap, statsRect.y + (float)height + (float)gap, buttonWidth, buttonHeight);
	}
}
