using System;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(GUITexture))]
public class Joystick : MonoBehaviour
{
	[NonSerialized]
	private static Joystick[] joysticks;

	[NonSerialized]
	private static bool enumeratedJoysticks;

	[NonSerialized]
	private static float tapTimeDelta = 0.3f;

	public bool touchPad;

	public Rect touchZone;

	public Vector2 deadZone;

	public bool normalize;

	public Vector2 position;

	public int tapCount;

	private int lastFingerId;

	private float tapTimeWindow;

	private Vector2 fingerDownPos;

	private float fingerDownTime;

	private float firstDeltaTime;

	private GUITexture gui;

	private Rect defaultRect;

	private Boundary guiBoundary;

	private Vector2 guiTouchOffset;

	private Vector2 guiCenter;

	public Joystick()
	{
		deadZone = Vector2.zero;
		lastFingerId = -1;
		firstDeltaTime = 0.5f;
		guiBoundary = new Boundary();
	}

	public virtual void Start()
	{
		gui = (GUITexture)GetComponent(typeof(GUITexture));
		defaultRect = gui.pixelInset;
		defaultRect.x += transform.position.x * (float)Screen.width;
		defaultRect.y += transform.position.y * (float)Screen.height;
		float x = 0f;
		Vector3 vector = transform.position;
		float num = (vector.x = x);
		Vector3 vector3 = (transform.position = vector);
		float y = 0f;
		Vector3 vector4 = transform.position;
		float num2 = (vector4.y = y);
		Vector3 vector6 = (transform.position = vector4);
		if (touchPad)
		{
			if ((bool)gui.texture)
			{
				touchZone = defaultRect;
			}
			return;
		}
		guiTouchOffset.x = defaultRect.width * 0.5f;
		guiTouchOffset.y = defaultRect.height * 0.5f;
		guiCenter.x = defaultRect.x + guiTouchOffset.x;
		guiCenter.y = defaultRect.y + guiTouchOffset.y;
		guiBoundary.min.x = defaultRect.x - guiTouchOffset.x;
		guiBoundary.max.x = defaultRect.x + guiTouchOffset.x;
		guiBoundary.min.y = defaultRect.y - guiTouchOffset.y;
		guiBoundary.max.y = defaultRect.y + guiTouchOffset.y;
	}

	public virtual void Disable()
	{
		gameObject.SetActive(false);
		enumeratedJoysticks = false;
	}

	public virtual void ResetJoystick()
	{
		gui.pixelInset = defaultRect;
		lastFingerId = -1;
		position = Vector2.zero;
		fingerDownPos = Vector2.zero;
		if (touchPad)
		{
			float a = 0.025f;
			Color color = gui.color;
			float num = (color.a = a);
			Color color3 = (gui.color = color);
		}
	}

	public virtual bool IsFingerDown()
	{
		return lastFingerId != -1;
	}

	public virtual void LatchedFinger(int fingerId)
	{
		if (lastFingerId == fingerId)
		{
			ResetJoystick();
		}
	}

	public virtual void Update()
	{
		if (!enumeratedJoysticks)
		{
			joysticks = ((Joystick[])UnityEngine.Object.FindObjectsOfType(typeof(Joystick))) as Joystick[];
			enumeratedJoysticks = true;
		}
		int touchCount = Input.touchCount;
		if (!(tapTimeWindow <= 0f))
		{
			tapTimeWindow -= Time.deltaTime;
		}
		else
		{
			tapCount = 0;
		}
		if (touchCount == 0)
		{
			ResetJoystick();
		}
		else
		{
			for (int i = 0; i < touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				Vector2 vector = touch.position - guiTouchOffset;
				bool flag = false;
				if (touchPad)
				{
					if (touchZone.Contains(touch.position))
					{
						flag = true;
					}
				}
				else if (gui.HitTest(touch.position))
				{
					flag = true;
				}
				if (flag && (lastFingerId == -1 || lastFingerId != touch.fingerId))
				{
					if (touchPad)
					{
						float a = 0.15f;
						Color color = gui.color;
						float num = (color.a = a);
						Color color3 = (gui.color = color);
						lastFingerId = touch.fingerId;
						fingerDownPos = touch.position;
						fingerDownTime = Time.time;
					}
					lastFingerId = touch.fingerId;
					if (!(tapTimeWindow <= 0f))
					{
						tapCount++;
					}
					else
					{
						tapCount = 1;
						tapTimeWindow = tapTimeDelta;
					}
					int j = 0;
					Joystick[] array = joysticks;
					for (int length = array.Length; j < length; j++)
					{
						if (array[j] != this)
						{
							array[j].LatchedFinger(touch.fingerId);
						}
					}
				}
				if (lastFingerId == touch.fingerId)
				{
					if (touch.tapCount > tapCount)
					{
						tapCount = touch.tapCount;
					}
					if (touchPad)
					{
						position.x = Mathf.Clamp((touch.position.x - fingerDownPos.x) / (touchZone.width / 2f), -1f, 1f);
						position.y = Mathf.Clamp((touch.position.y - fingerDownPos.y) / (touchZone.height / 2f), -1f, 1f);
					}
					else
					{
						float num2 = Mathf.Clamp(vector.x, guiBoundary.min.x, guiBoundary.max.x);
						Rect pixelInset = gui.pixelInset;
						float num4 = (pixelInset.x = num2);
						Rect rect2 = (gui.pixelInset = pixelInset);
						float num5 = Mathf.Clamp(vector.y, guiBoundary.min.y, guiBoundary.max.y);
						Rect pixelInset2 = gui.pixelInset;
						float num7 = (pixelInset2.y = num5);
						Rect rect4 = (gui.pixelInset = pixelInset2);
					}
					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						ResetJoystick();
					}
				}
			}
		}
		if (!touchPad)
		{
			position.x = (gui.pixelInset.x + guiTouchOffset.x - guiCenter.x) / guiTouchOffset.x;
			position.y = (gui.pixelInset.y + guiTouchOffset.y - guiCenter.y) / guiTouchOffset.y;
		}
		float num8 = Mathf.Abs(position.x);
		float num9 = Mathf.Abs(position.y);
		if (!(num8 >= deadZone.x))
		{
			position.x = 0f;
		}
		else if (normalize)
		{
			position.x = Mathf.Sign(position.x) * (num8 - deadZone.x) / (1f - deadZone.x);
		}
		if (!(num9 >= deadZone.y))
		{
			position.y = 0f;
		}
		else if (normalize)
		{
			position.y = Mathf.Sign(position.y) * (num9 - deadZone.y) / (1f - deadZone.y);
		}
	}

	public virtual void Main()
	{
	}
}
