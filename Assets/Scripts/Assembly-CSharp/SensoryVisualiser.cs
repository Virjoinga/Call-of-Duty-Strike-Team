using UnityEngine;

public class SensoryVisualiser : MonoBehaviour
{
	public static SensoryVisualiser instance;

	private bool mHidden;

	public static SensoryVisualiser Instance()
	{
		return instance;
	}

	private void Awake()
	{
		Object.Destroy(this);
	}

	private void Start()
	{
		mHidden = true;
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (mHidden)
		{
			return;
		}
		string text = "Sensory Visualiser";
		int num = 250;
		Rect position = new Rect(Screen.width - 500, Screen.height - (num + 20), 450f, num);
		if (mHidden)
		{
			num = 50;
			position.x = (float)(Screen.width - 10) - position.width;
		}
		GUI.Box(position, text);
		if (mHidden)
		{
			return;
		}
		float num2 = position.y + 30f;
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.ActorsInPlay);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (!a.awareness.visible)
			{
				Rect position2 = new Rect(position.x + 10f, num2, position.width - 20f, 20f);
				GUI.Label(position2, string.Format("INVISIBLE: {0}", a.realCharacter.name));
				num2 += 20f;
			}
		}
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

	public void GLDebugVisualise()
	{
		if (!mHidden)
		{
			if (GlobalKnowledgeManager.Instance() != null)
			{
				GlobalKnowledgeManager.Instance().GlDebugVisualise(true);
			}
			if (AuditoryAwarenessManager.Instance != null)
			{
				AuditoryAwarenessManager.Instance.GlDebugVisualise();
			}
		}
	}
}
