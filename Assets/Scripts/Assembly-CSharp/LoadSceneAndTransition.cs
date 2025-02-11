using System;
using System.Collections;
using UnityEngine;

public class LoadSceneAndTransition : MonoBehaviour
{
	private enum LoadTransState
	{
		PRE_LOAD_CLEANUP = 0,
		LOADING_ASYNC = 1,
		TRANSITIONING = 2,
		CLEANUP = 3
	}

	public delegate void TransitionCompleteEventHandler(object sender, EventArgs args);

	public delegate void PreLoadCleanupEventHandler(object sender, EventArgs args);

	private string mSceneName;

	private AsyncOperation mAsyncOpp;

	private bool mIsSwitching;

	private Camera mCurrentCamera;

	private LoadTransState mState;

	public event TransitionCompleteEventHandler OnTransitionComplete;

	public event PreLoadCleanupEventHandler OnPreLoadCleanup;

	public void SetSceneName(string sceneName)
	{
		mSceneName = sceneName;
	}

	public void SetTransitionFromCamera(Camera cam)
	{
		mCurrentCamera = cam;
	}

	private void Start()
	{
		mState = LoadTransState.PRE_LOAD_CLEANUP;
	}

	private void Update()
	{
		switch (mState)
		{
		case LoadTransState.PRE_LOAD_CLEANUP:
			mAsyncOpp = Application.LoadLevelAdditiveAsync(mSceneName);
			if (this.OnPreLoadCleanup != null)
			{
				this.OnPreLoadCleanup(this, new EventArgs());
			}
			mState = LoadTransState.LOADING_ASYNC;
			break;
		case LoadTransState.LOADING_ASYNC:
			if (mAsyncOpp.isDone)
			{
				StartCoroutine(DoTransitionCo());
				mState = LoadTransState.TRANSITIONING;
			}
			break;
		case LoadTransState.TRANSITIONING:
			if (!mIsSwitching)
			{
				mState = LoadTransState.CLEANUP;
			}
			break;
		case LoadTransState.CLEANUP:
			if (this.OnTransitionComplete != null)
			{
				this.OnTransitionComplete(this, new EventArgs());
			}
			UnityEngine.Object.Destroy(base.gameObject);
			break;
		}
	}

	private IEnumerator DoTransitionCo()
	{
		if (mIsSwitching)
		{
			yield return null;
		}
		mIsSwitching = true;
		if (CameraManager.Instance != null)
		{
			yield return StartCoroutine(ScreenWipe.use.CrossFadePro(mCurrentCamera, CameraManager.Instance.StrategyCamera, 2f));
		}
		mIsSwitching = false;
	}
}
