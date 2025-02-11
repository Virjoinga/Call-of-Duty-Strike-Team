using System;
using UnityEngine;

public class SetPieceVisualiser : MonoBehaviour
{
	private const int kVisualWidth = 500;

	private const int kVisualHeight = 150;

	private const int kMargin = 10;

	private const int kGap = 5;

	private const int kButtonWidth = 150;

	private const int kControlButtonWidth = 70;

	private const int kButtonHeight = 20;

	private const float kCamDist = 7f;

	private Vector2 mScrollPos = Vector2.zero;

	private static SetPieceVisualiser mInstance;

	public float SlowMoSpeed = 0.25f;

	private bool mHidden;

	private SetPieceModule currentSetPiece;

	private bool mUpdateCamera;

	private bool mIsSlowMo;

	private bool mPaused;

	public static SetPieceVisualiser Instance()
	{
		return mInstance;
	}

	private void Awake()
	{
		mInstance = this;
		SoundManager.Instance.SetVolumeGroup(SoundFXData.VolumeGroup.Cutscene, 1f);
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Start()
	{
		mUpdateCamera = false;
		mPaused = false;
	}

	private void LateUpdate()
	{
		if (mUpdateCamera && (bool)currentSetPiece)
		{
			CameraBase theCamera = currentSetPiece.TheCamera;
			Camera.main.transform.position = theCamera.transform.position;
			Camera.main.transform.rotation = theCamera.transform.rotation;
		}
	}

	private void OnGUI()
	{
		if (mHidden)
		{
			return;
		}
		int num = Screen.width - 500 - 10;
		int num2 = Screen.height - 150 - 10;
		int num3 = num + 10;
		int num4 = num2 + 30;
		string text = "SetPiece Visualiser" + Environment.NewLine;
		GUIStyle gUIStyle = new GUIStyle(GUI.skin.box);
		gUIStyle.alignment = TextAnchor.UpperLeft;
		GUI.Box(Rect.MinMaxRect(num, num2, num + 500, num2 + 150), text, gUIStyle);
		SetPieceModule[] array = UnityEngine.Object.FindObjectsOfType(typeof(SetPieceModule)) as SetPieceModule[];
		mScrollPos = GUI.BeginScrollView(new Rect(num, num2 + 20, 168f, 130f), mScrollPos, new Rect(0f, 0f, 150f, array.Length * 25));
		num3 = 0;
		num4 = 0;
		SetPieceModule[] array2 = array;
		foreach (SetPieceModule setPieceModule in array2)
		{
			if (setPieceModule.name.Contains("(Clone)"))
			{
				continue;
			}
			if (DoButton(num3, num4, 150f, 20f, setPieceModule.name) && currentSetPiece != setPieceModule)
			{
				if ((bool)currentSetPiece)
				{
					currentSetPiece.Stop();
				}
				currentSetPiece = setPieceModule;
				ResetStates();
				Camera camera = null;
				if (setPieceModule.TheCamera != null)
				{
					mUpdateCamera = true;
				}
				else
				{
					camera = Camera.allCameras[0];
					if ((bool)camera)
					{
						Vector3 vector = new Vector3(-4.5499997f, 7f, -5.25f);
						Camera.main.transform.position = setPieceModule.transform.position + vector;
						Camera.main.transform.LookAt(setPieceModule.transform.position);
						mUpdateCamera = false;
					}
				}
			}
			num4 += 25;
			text += setPieceModule.name;
		}
		GUI.EndScrollView();
		num3 = num + 150 + 20;
		num4 = num2 + 10;
		int num5 = num3 + 320;
		int num6 = num4 + 80;
		if (!currentSetPiece)
		{
			return;
		}
		text = currentSetPiece.name + Environment.NewLine + Environment.NewLine;
		string text2 = text;
		text = text2 + "Current Timer: " + currentSetPiece.GetCurrentSequenceTime() + Environment.NewLine;
		int currentStatement = currentSetPiece.GetCurrentStatement();
		if (currentStatement == -1)
		{
			text = text + "Current Statement: Not Running" + Environment.NewLine;
		}
		else
		{
			text2 = text;
			text = text2 + "Current Statement: " + currentSetPiece.GetCurrentStatement() + Environment.NewLine;
		}
		float num7 = num3 + 10;
		float num8 = num6 + 10;
		if (currentSetPiece.IsPlaying())
		{
			if (mPaused)
			{
				if (DoButton(num7, num8, 70f, 20f, "Play"))
				{
					if (mIsSlowMo)
					{
						Time.timeScale = SlowMoSpeed;
					}
					else
					{
						Time.timeScale = 1f;
					}
					mPaused = false;
				}
			}
			else if (DoButton(num7, num8, 70f, 20f, "Pause"))
			{
				Time.timeScale = 0f;
				mPaused = true;
			}
		}
		else if (DoButton(num7, num8, 70f, 20f, "Play"))
		{
			currentSetPiece.Stop();
			currentSetPiece.Play();
		}
		num7 += 75f;
		if (DoButton(num7, num8, 70f, 20f, "Restart"))
		{
			currentSetPiece.Stop();
			currentSetPiece.Play();
		}
		num7 += 75f;
		if (mIsSlowMo)
		{
			if (DoButton(num7, num8, 70f, 20f, "Normal"))
			{
				Time.timeScale = 1f;
				mIsSlowMo = false;
			}
		}
		else if (DoButton(num7, num8, 70f, 20f, "SlowMo"))
		{
			Time.timeScale = SlowMoSpeed;
			mIsSlowMo = true;
		}
		num7 = num3;
		num8 += 25f;
		GUI.Box(Rect.MinMaxRect(num3, num4, num5, num6), text, gUIStyle);
	}

	private void SelectSetPiece(SetPieceModule setPiece)
	{
	}

	private bool DoButton(float x, float y, float width, float height, string label)
	{
		Rect position = new Rect(x, y, width, height);
		return GUI.Button(position, label);
	}

	private void ResetStates()
	{
		mIsSlowMo = false;
		mPaused = false;
		Time.timeScale = 1f;
	}
}
