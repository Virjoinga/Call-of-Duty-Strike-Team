using UnityEngine;

[DefaultExecutionOrder(-10000)]
public sealed class GameCursor : MonoBehaviour
{
    private static GameCursor sInstance;
    private bool mFocused;
    private bool mApplicationPaused;
    private bool mReleasedManually;
    private bool mWasGameplay;
    private int mCaptureFrame;
    private FrontEndController mFrontend;
    private MessageBoxController mMessages;

    public static bool CanUseMouse
    {
        get { return sInstance != null && sInstance.WantsCapture() &&
            Cursor.lockState == CursorLockMode.Locked && Time.frameCount > sInstance.mCaptureFrame; }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        if (sInstance == null) new GameObject("Desktop cursor").AddComponent<GameCursor>();
#endif
    }

    private void Awake()
    {
        if (sInstance != null && sInstance != this) { Destroy(gameObject); return; }
        sInstance = this;
        mFocused = Application.isFocused;
        DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update += EditorTick;
#endif
        Apply();
    }

    private bool IsGameplay()
    {
        GameController game = GameController.Instance;
        CameraManager cameras = CameraManager.Instance;
        if (game == null || !game.IsReady || !game.GameplayStarted || !game.IsFirstPerson ||
            game.IsPaused || CinematicHelper.IsInCinematic ||
            (TimeManager.instance != null && TimeManager.instance.GlobalTimeState == TimeManager.State.IngamePaused) ||
            (OverwatchController.Instance != null && OverwatchController.Instance.Active) ||
            cameras == null || cameras.IsSwitching ||
            cameras.ActiveCamera != CameraManager.ActiveCameraType.PlayCamera) return false;
        if (mFrontend == null) mFrontend = FindObjectOfType<FrontEndController>();
        if (mMessages == null) mMessages = FindObjectOfType<MessageBoxController>();
        return (mFrontend == null || mFrontend.ActiveScreen == ScreenID.None) &&
            (mMessages == null || !mMessages.IsAnyMessageActive);
    }

    private bool WantsCapture()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPaused) return false;
#endif
        return mFocused && !mApplicationPaused && !mReleasedManually && IsGameplay();
    }

    private void Update()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        bool gameplay = IsGameplay();
        if (gameplay != mWasGameplay) { mReleasedManually = false; mWasGameplay = gameplay; }
        GameController game = GameController.Instance;
        if (mFocused && Input.GetKeyDown(KeyCode.Escape) && game != null && game.IsReady &&
            game.GameplayStarted && !CinematicHelper.IsInCinematic &&
            (mFrontend == null || mFrontend.ActiveScreen == ScreenID.None || mFrontend.ActiveScreen == ScreenID.Pause) &&
            (mMessages == null || !mMessages.IsAnyMessageActive)) game.TogglePause();
        if (mFocused && gameplay && Input.GetKeyDown(KeyCode.Tab)) mReleasedManually = !mReleasedManually;
        Apply();
#endif
    }

    private void LateUpdate() { Apply(); }
    private void Apply() { SetCursor(WantsCapture()); }

    private void SetCursor(bool capture)
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        CursorLockMode mode = capture ? CursorLockMode.Locked : CursorLockMode.None;
        if (Cursor.lockState != mode)
        {
#if UNITY_EDITOR
            if (capture)
            {
                UnityEditor.Unsupported.SetAllowCursorLock(true);
                UnityEditor.Unsupported.SetAllowCursorHide(true);
            }
#endif
            Cursor.lockState = mode;
            if (capture) mCaptureFrame = Time.frameCount;
        }
        if (Cursor.visible != !capture) Cursor.visible = !capture;
#endif
    }

#if UNITY_EDITOR
    private void EditorTick()
    {
        if (!UnityEditor.EditorApplication.isPlaying || UnityEditor.EditorApplication.isPaused) SetCursor(false);
    }
#endif
    private void OnApplicationFocus(bool focused) { mFocused = focused; Apply(); }
    private void OnApplicationPause(bool paused) { mApplicationPaused = paused; Apply(); }
    private void OnDisable() { if (sInstance == this) SetCursor(false); }
    private void OnDestroy()
    {
        if (sInstance != this) return;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update -= EditorTick;
#endif
        sInstance = null;
        SetCursor(false);
    }
}
