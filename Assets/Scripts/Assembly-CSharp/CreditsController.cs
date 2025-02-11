using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
	private const float IMAGE_DURATION = 3.5f;

	private const float MINIMUM_TEXT_DURATION = 6f;

	private const float TEXT_DURATION_PER_LETTER = 0.08f;

	private const float WAIT_DELAY = 0.5f;

	private const float TIME_TO_MOVE_CAMERA = 0.5f;

	public MissionBriefingTextFocusItem FocusItemPrefab;

	public MissionBriefingImageItem ImageItemPrefab;

	public CreditsList CreditsListAsset;

	public GameObject PositionsRoot;

	public GameObject EntireLevelModel;

	public UIButton SkipMessage;

	private List<LookAtCamera> mCameraList;

	private CreditPosition[] mPositions;

	private MissionBriefingImageItem mImageItem;

	private Camera mCamera;

	private Camera mGlobeCamera;

	private CameraController mCameraController;

	private MissionBriefingTextFocusItem mFocusItem;

	private LookAtCamera mLastLookAtCamera;

	private int mLastCreditPositionIndex = -1;

	private bool mEnded = true;

	public bool SequenceIsRunning
	{
		get
		{
			return !mEnded;
		}
	}

	public void Awake()
	{
		mCamera = GetComponentInChildren<Camera>();
		mCameraController = GetComponentInChildren<CameraController>();
		mCameraList = new List<LookAtCamera>();
		if (mCamera != null)
		{
			mCamera.gameObject.SetActive(false);
		}
		GameObject gameObject = Object.FindObjectOfType(typeof(GlobeCamera)) as GameObject;
		if (gameObject != null)
		{
			mGlobeCamera = gameObject.GetComponent<Camera>();
		}
		if (PositionsRoot != null)
		{
			mPositions = PositionsRoot.GetComponentsInChildren<CreditPosition>();
		}
		if (EntireLevelModel != null)
		{
			EntireLevelModel.SetActive(false);
		}
	}

	public void Start()
	{
		HideButtons();
	}

	public void BeginSequence()
	{
		SwrveEventsUI.ViewedCredits();
		SoundManager.Instance.ActivateMissionBriefingSFX();
		MusicManager.Instance.PlayBriefingMusic();
		mEnded = false;
		if (mFocusItem == null)
		{
			mFocusItem = Object.Instantiate(FocusItemPrefab) as MissionBriefingTextFocusItem;
			mFocusItem.transform.parent = base.transform;
		}
		if (mImageItem == null)
		{
			mImageItem = Object.Instantiate(ImageItemPrefab) as MissionBriefingImageItem;
			mImageItem.transform.parent = base.transform;
		}
		StartCoroutine(RunSequence());
	}

	private IEnumerator RunSequence()
	{
		FrontEndController fe = FrontEndController.Instance;
		AnimatedScreenBackground bg = AnimatedScreenBackground.Instance;
		if (fe != null && bg != null)
		{
			fe.TransitionTo(ScreenID.None);
			bg.Activate();
			while (fe.IsBusy || !bg.IsActive)
			{
				yield return null;
			}
		}
		ShowButtons();
		if (EntireLevelModel != null)
		{
			EntireLevelModel.SetActive(true);
		}
		if (mGlobeCamera != null)
		{
			mGlobeCamera.gameObject.SetActive(false);
		}
		if (mCamera != null)
		{
			mCamera.gameObject.SetActive(true);
		}
		if (bg != null)
		{
			bg.Hide();
			while (!bg.IsHidden)
			{
				yield return null;
			}
		}
		for (int count = 0; count < CreditsListAsset.EntryList.Count; count++)
		{
			if (mEnded)
			{
				break;
			}
			while (!IsReadyToShowText())
			{
				yield return null;
			}
			string data2 = CreditsListAsset.EntryList[count];
			if (data2.StartsWith("IMAGE:"))
			{
				while (!IsReadyToShowImage())
				{
					mImageItem.ReleaseFromWait();
					yield return null;
				}
				string filename = data2.Replace("IMAGE:", string.Empty);
				Vector2 logoSize = new Vector2(256f, 128f);
				if (TBFUtils.IsRetinaHdDevice())
				{
					filename += "_@x2";
					logoSize *= 2f;
				}
				Vector2 logoPosition = new Vector2((float)Screen.width * 0.08f, (float)Screen.height * 0.91f - logoSize.y * 0.5f);
				Texture texture = Resources.Load(filename, typeof(Texture2D)) as Texture;
				ShowImage(texture, logoPosition, logoSize, 3.5f);
				continue;
			}
			int positionIndex = FindNextCreditPosition();
			float timeToShow = Mathf.Max(6f, (float)data2.Length * 0.08f);
			float time = Time.realtimeSinceStartup + timeToShow + 0.5f;
			data2 = data2.Replace("|", "\r\n");
			CreditPosition entry = mPositions[positionIndex];
			if (entry != null)
			{
				ShowTextAndFocusOnPoint(data2, entry.transform, timeToShow, entry.Direction);
			}
			if (count + 1 >= CreditsListAsset.EntryList.Count)
			{
				mImageItem.ReleaseFromWait();
			}
			while (!mEnded && Time.realtimeSinceStartup < time)
			{
				yield return null;
			}
		}
		SkipSequence();
	}

	public IEnumerator EndSequence()
	{
		SoundManager.Instance.DeactivateMissionBriefingSFX();
		if (mEnded)
		{
			yield break;
		}
		mEnded = true;
		SoundManager.Instance.ActivateGlobeSFX();
		FrontEndController fe = FrontEndController.Instance;
		AnimatedScreenBackground bg = AnimatedScreenBackground.Instance;
		if (bg != null)
		{
			bg.Activate();
			while (!bg.IsActive)
			{
				yield return null;
			}
		}
		HideButtons();
		if (mFocusItem != null)
		{
			Object.Destroy(mFocusItem.gameObject);
			mFocusItem = null;
		}
		if (mImageItem != null)
		{
			Object.Destroy(mImageItem.gameObject);
			mImageItem = null;
		}
		if (EntireLevelModel != null)
		{
			EntireLevelModel.SetActive(false);
		}
		if (mCamera != null)
		{
			mCamera.gameObject.SetActive(false);
		}
		if (mGlobeCamera != null)
		{
			mGlobeCamera.gameObject.SetActive(true);
		}
		if (bg != null && fe != null)
		{
			bg.Hide();
			fe.TransitionTo(ScreenID.MissionSelect);
			while (!bg.IsHidden || fe.IsBusy)
			{
				yield return null;
			}
		}
		BriefingSFX.Instance.BoxStatic.Stop2D();
	}

	private void SkipSequence()
	{
		if (!mEnded)
		{
			StartCoroutine(EndSequence());
		}
	}

	private void HideButtons()
	{
		if (SkipMessage != null)
		{
			SkipMessage.Hide(true);
		}
	}

	private void ShowButtons()
	{
		if (SkipMessage != null)
		{
			SkipMessage.gameObject.MoveFrom(SkipMessage.transform.position - new Vector3(0f, 3f, 0f), 0.4f, 0f, EaseType.easeOutSine);
			SkipMessage.Hide(false);
		}
	}

	private bool ShowTextAndFocusOnPoint(string text, Transform focus, float duration, MissionBriefingHelper.FocusDirection direction)
	{
		bool result = false;
		if (!mEnded)
		{
			if (mFocusItem != null)
			{
				float z = -0.2f;
				if (mCameraController != null)
				{
					GameObject gameObject = new GameObject("LookAt - " + focus.name + " - CameraTransition");
					gameObject.transform.parent = base.transform;
					Vector3 position = focus.position;
					position.y = mCamera.transform.position.y;
					mLastLookAtCamera = gameObject.AddComponent<LookAtCamera>();
					mLastLookAtCamera.LookAt = focus;
					mLastLookAtCamera.transform.position = position;
					mLastLookAtCamera.transform.up = mCamera.transform.up;
					mCameraList.Add(mLastLookAtCamera);
					CameraTransitionData ctd = new CameraTransitionData(mLastLookAtCamera, TweenFunctions.TweenType.easeInOutCubic, 0.5f);
					mCameraController.BlendTo(ctd);
				}
				mFocusItem.DisplayItem(text, duration - 0.5f, 0.5f, z, direction);
				result = true;
			}
			else
			{
				Debug.LogError("Called to focus on an item before BeginSequence was called");
			}
		}
		return result;
	}

	private bool IsReadyToShowText()
	{
		bool result = true;
		if (mFocusItem != null)
		{
			result = !mFocusItem.Displaying;
		}
		return result;
	}

	private bool IsReadyToShowImage()
	{
		bool result = true;
		if (mImageItem != null)
		{
			result = !mImageItem.IsBusy;
		}
		return result;
	}

	private bool ShowImage(Texture texture, Vector2 position, Vector2 size, float duration)
	{
		bool result = false;
		if (!mEnded && mImageItem != null)
		{
			float z = -0.2f;
			Vector3 position2 = new Vector3(position.x, position.y, z);
			mImageItem.DisplayAndHoldItem(texture, position2, size, duration, false);
			result = true;
		}
		return result;
	}

	private int FindNextCreditPosition()
	{
		int num = mLastCreditPositionIndex;
		bool flag = TBFUtils.IsSmallScreenDevice() || TBFUtils.IsSmallRetinaDevice();
		bool flag2 = TBFUtils.UseAlternativeLayout();
		while (num == mLastCreditPositionIndex)
		{
			num = Random.Range(0, mPositions.Length);
			CreditPosition creditPosition = mPositions[num];
			if ((flag && creditPosition != null && (creditPosition.Direction == MissionBriefingHelper.FocusDirection.Top || creditPosition.Direction == MissionBriefingHelper.FocusDirection.Bottom)) || (flag2 && creditPosition != null && creditPosition.Direction != 0))
			{
				num = mLastCreditPositionIndex;
			}
		}
		mLastCreditPositionIndex = num;
		return num;
	}
}
