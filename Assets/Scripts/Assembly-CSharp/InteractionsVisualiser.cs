using UnityEngine;

public class InteractionsVisualiser : MonoBehaviour
{
	public static InteractionsVisualiser instance;

	private bool mHidden = true;

	public static InteractionsVisualiser Instance()
	{
		return instance;
	}

	private void Awake()
	{
		instance = this;
	}

	private void OnGUI()
	{
		if (mHidden)
		{
			return;
		}
		string text = "Interactions Manager Window";
		int num = 600;
		int num2 = 800;
		Rect screenRect = new Rect(Screen.width / 2 - num2 / 2, Screen.height / 2 - num / 2, num2, num);
		GUILayout.BeginArea(screenRect);
		GUILayout.BeginVertical(text, GUI.skin.window);
		if (DoButton(screenRect.x + (float)(num2 - 260 - 5), screenRect.y + 5f, 130f, 25f, "Skip Current Action"))
		{
			InteractionsManager.Instance.SkipCurrentMustSeeAction();
		}
		InteractionsManager interactionsManager = InteractionsManager.Instance;
		GUILayout.Label("Interaction state: " + interactionsManager.StateString);
		GUILayout.Label("Current 'Must See' action");
		PrintInteraction(interactionsManager.CurrentMustSeeAction);
		GUILayout.Label("Current 'Prefer-See' action");
		PrintInteraction(interactionsManager.CurrentPreferSeeAction);
		GUILayout.Space(10f);
		GUILayout.Label("Current Actions");
		foreach (Interaction currentAction in interactionsManager.CurrentActions)
		{
			PrintInteraction(currentAction);
		}
		GUILayout.Space(10f);
		GUILayout.Label("Queued Actions");
		foreach (Interaction queuedAction in interactionsManager.QueuedActions)
		{
			GUILayout.Label(queuedAction.GameObj.name);
		}
		GUILayout.Space(10f);
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	public void Hide()
	{
		mHidden = true;
	}

	public void Show()
	{
		mHidden = false;
	}

	public void Toggle()
	{
		mHidden = !mHidden;
	}

	private void PrintInteraction(Interaction interAct)
	{
		if (interAct != null)
		{
			string text = interAct.GameObj.name;
			if (interAct.GameTask != null)
			{
				text = text + " (" + interAct.GameTask.ToString() + ")";
			}
			GUILayout.Label(text);
		}
	}

	private bool DoButton(float x, float y, float width, float height, string label)
	{
		Rect position = new Rect(x, y, width, height);
		return GUI.Button(position, label);
	}
}
