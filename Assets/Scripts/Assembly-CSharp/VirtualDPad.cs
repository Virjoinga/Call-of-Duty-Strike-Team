using UnityEngine;

public class VirtualDPad : MonoBehaviour
{
	private Rect m_touchRegion;

	private Vector3 m_prevMousePosition;

	private Vector3 m_firstDownPos;

	[SerializeField]
	private Rect m_screenProps;

	[SerializeField]
	private Rect m_altScreenProps;

	[SerializeField]
	private float m_scaleX = 15f;

	[SerializeField]
	private float m_scaleY = 15f;

	public static bool HasKeyboard
	{
		get
		{
			return false;
		}
	}

	public float XAxis { get; private set; }

	public float YAxis { get; private set; }

	public float XPos { get; private set; }

	public float YPos { get; private set; }

	private Rect ScreenProps
	{
		get
		{
			return (!HasKeyboard) ? m_screenProps : m_altScreenProps;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		m_touchRegion = new Rect((float)Screen.width * ScreenProps.x, (float)Screen.height * ScreenProps.y, (float)Screen.width * ScreenProps.width, (float)Screen.height * ScreenProps.height);
		bool flag = false;
		Vector3 vector;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			vector = m_prevMousePosition;
			for (int i = 0; i < Input.touchCount; i++)
			{
				Debug.Log("Touch detected");
				Touch touch = Input.GetTouch(i);
				if (m_touchRegion.Contains(touch.position))
				{
					if (touch.phase == TouchPhase.Began)
					{
						m_prevMousePosition = touch.position;
						m_firstDownPos = touch.position;
					}
					flag = true;
					vector = touch.position;
					break;
				}
			}
		}
		else
		{
			vector = Input.mousePosition;
			if (m_touchRegion.Contains(Input.mousePosition))
			{
				flag = Input.GetMouseButton(1);
				if (Input.GetMouseButtonDown(1))
				{
					m_firstDownPos = Input.mousePosition;
				}
			}
		}
		XAxis = 0f;
		YAxis = 0f;
		XPos = 0f;
		YPos = 0f;
		if (flag)
		{
			Vector3 vector2 = CalcDelta(vector, m_prevMousePosition);
			XAxis = vector2.x;
			YAxis = vector2.y;
			Vector3 vector3 = CalcDelta(vector, m_firstDownPos);
			XPos = vector3.x;
			YPos = vector3.y;
		}
		m_prevMousePosition = vector;
	}

	private Vector3 CalcDelta(Vector3 position, Vector3 anchor)
	{
		Vector3 result = position - anchor;
		result.x = result.x / m_touchRegion.width * m_scaleX;
		result.y = result.y / m_touchRegion.height * m_scaleY;
		return result;
	}
}
