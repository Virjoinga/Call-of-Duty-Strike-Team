using UnityEngine;

public class RealCharacterVisualiser : MonoBehaviour
{
	public static RealCharacterVisualiser instance;

	private bool mHidden;

	public static RealCharacterVisualiser Instance()
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
		if (!mHidden)
		{
			string text = "RealCharacter Visualiser";
			Rect rect = new Rect(Screen.width - 230, Screen.height - 520, 200f, 500f);
			GUI.Box(rect, text);
			ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.AIMask);
			int num = 0;
			Actor a;
			while (actorIdentIterator.NextActor(out a))
			{
				Rect position = rect;
				position.x += 5f;
				position.y += num * 20 + 25;
				string text2 = string.Format("{0}:{1}", num, a.realCharacter.name);
				GUI.Label(position, text2);
				num++;
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
		}
	}
}
