using System;
using UnityEngine;

public class MediaPlayerCtrl : MonoBehaviour
{
	public enum MediaPlayerError
	{
		MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK = 200,
		MEDIA_ERROR_IO = -1004,
		MEDIA_ERROR_MALFORMED = -1007,
		MEDIA_ERROR_TIMED_OUT = -110,
		MEDIA_ERROR_UNSUPPORTED = -1010,
		MEDIA_ERROR_SERVER_DIED = 100,
		MEDIA_ERROR_UNKNOWN = 1
	}

	public enum MediaPlayerState
	{
		NOT_READY = 0,
		READY = 1,
		END = 2,
		PLAYING = 3,
		PAUSED = 4,
		STOPPED = 5,
		ERROR = 6
	}

	public Texture2D texture;

	public Action videoDidStartEvent;

	public Action videoDidFinishEvent;

	private string mFilename = string.Empty;

	private bool mLoop;

	private bool mAutoPlay = true;

	private MediaPlayerState mCurrentState;

	private int mCurrentSeekPosition;

	private bool mStop;

	private bool mCheckFBO;

	private AndroidJavaObject javaObj;

	private void Awake()
	{
        Application.LoadLevel("01_Arc_Mission_S1_Baked");
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
		gameObject.transform.position = base.transform.position + new Vector3(0f, 0f, 0.5f);
		gameObject.GetComponent<Renderer>().material = new Material(Shader.Find("Diffuse"));
		gameObject.GetComponent<Renderer>().material.color = Color.black;
	}

	private void Update()
	{
		if (mCurrentState == MediaPlayerState.PLAYING)
		{
			if (!mCheckFBO)
			{
				if (Call_GetVideoWidth() <= 0 || Call_GetVideoHeight() <= 0)
				{
					return;
				}
				Call_SetWindowSize();
				mCheckFBO = true;
			}
			Call_UpdateVideoTexture();
			mCurrentSeekPosition = Call_GetSeekPosition();
		}
		/*if (mCurrentState == Call_GetStatus())
		{
			return;
		}
		mCurrentState = Call_GetStatus();*/
		if (mCurrentState == MediaPlayerState.READY)
		{
			if (mAutoPlay)
			{
				Call_Play(0);
			}
		}
		else if (mCurrentState == MediaPlayerState.PLAYING)
		{
			if (videoDidStartEvent != null)
			{
				videoDidStartEvent();
			}
		}
		else if (mCurrentState == MediaPlayerState.END)
		{
			if (videoDidFinishEvent != null)
			{
				videoDidFinishEvent();
			}
			if (mLoop)
			{
				Call_Play(0);
			}
		}
		else if (mCurrentState == MediaPlayerState.ERROR)
		{
			OnError((MediaPlayerError)Call_GetError(), (MediaPlayerError)Call_GetErrorExtra());
		}
	}

	private void OnError(MediaPlayerError iCode, MediaPlayerError iCodeExtra)
	{
		string empty = string.Empty;
		switch (iCode)
		{
		case MediaPlayerError.MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK:
			empty = "MEDIA_ERROR_NOT_VALID_FOR_PROGRESSIVE_PLAYBACK";
			break;
		case MediaPlayerError.MEDIA_ERROR_SERVER_DIED:
			empty = "MEDIA_ERROR_SERVER_DIED";
			break;
		case MediaPlayerError.MEDIA_ERROR_UNKNOWN:
			empty = "MEDIA_ERROR_UNKNOWN";
			break;
		default:
			empty = "Unknown error " + iCode;
			break;
		}
		empty += " ";
		switch (iCodeExtra)
		{
		case MediaPlayerError.MEDIA_ERROR_IO:
			empty += "MEDIA_ERROR_IO";
			break;
		case MediaPlayerError.MEDIA_ERROR_MALFORMED:
			empty += "MEDIA_ERROR_MALFORMED";
			break;
		case MediaPlayerError.MEDIA_ERROR_TIMED_OUT:
			empty += "MEDIA_ERROR_TIMED_OUT";
			break;
		case MediaPlayerError.MEDIA_ERROR_UNSUPPORTED:
			empty += "MEDIA_ERROR_UNSUPPORTED";
			break;
		default:
			empty = "Unknown error " + iCode;
			break;
		}
		Debug.LogError(empty);
	}

	private void OnDestroy()
	{
		Call_UnLoad();
		if (texture != null)
		{
			UnityEngine.Object.Destroy(texture);
		}
		Call_Destroy();
	}

	private void OnApplicationPause(bool bPause)
	{
		if (bPause)
		{
			Call_Pause();
		}
		else
		{
			Call_RePlay();
		}
	}

	public void Play()
	{
		if (mStop)
		{
			Call_Play(0);
			mStop = false;
		}
		if (mCurrentState == MediaPlayerState.PAUSED)
		{
			Call_RePlay();
		}
		else if (mCurrentState == MediaPlayerState.READY || mCurrentState == MediaPlayerState.STOPPED)
		{
			Call_Play(0);
		}
	}

	public void stop()
	{
		if (mCurrentState == MediaPlayerState.PLAYING)
		{
			Call_Pause();
		}
		mStop = true;
		mCurrentSeekPosition = 0;
	}

	public void Pause()
	{
		if (mCurrentState == MediaPlayerState.PLAYING)
		{
			Call_Pause();
		}
		mCurrentState = MediaPlayerState.PAUSED;
	}

	public void Load(string fileName, bool loop, bool autoPlay, float startAudioVolume)
	{
		Debug.Log("MediaPlayerCtrl Load: filename=" + fileName);
		texture = new Texture2D(0, 0, TextureFormat.RGB565, false);
		texture.filterMode = FilterMode.Bilinear;
		texture.wrapMode = TextureWrapMode.Clamp;
		base.GetComponent<Renderer>().material.mainTexture = texture;
		Call_SetUnityActivity();
		Call_SetUnityTexture(texture.GetNativeTextureID());
		mCheckFBO = false;
		mLoop = loop;
		mAutoPlay = autoPlay;
		mFilename = fileName;
		Call_Load(mFilename, 0);
		Call_SetVolume(startAudioVolume);
		mCurrentState = MediaPlayerState.NOT_READY;
	}

	public void SetVolume(float fVolume)
	{
		Call_SetVolume(fVolume);
	}

	public int GetSeekPosition()
	{
		return mCurrentSeekPosition;
	}

	public void SeekTo(int iSeek)
	{
		Call_SetSeekPosition(iSeek);
	}

	public int GetDuration()
	{
		return Call_GetDuration();
	}

	public int GetCurrentSeekPercent()
	{
		return Call_GetCurrentSeekPercent();
	}

	public int GetVideoWidth()
	{
		return Call_GetVideoWidth();
	}

	public int GetVideoHeight()
	{
		return Call_GetVideoHeight();
	}

	public void UnLoad()
	{
		mCheckFBO = false;
		Call_UnLoad();
	}

	private AndroidJavaObject GetJavaObject()
	{
		/*if (javaObj == null)
		{
			javaObj = new AndroidJavaObject("com.EasyMovieTexture.EasyMovieTexture");
		}*/
		return javaObj;
	}

	private void Call_Destroy()
	{
		GetJavaObject().Call("Destroy");
	}

	private void Call_UnLoad()
	{
		GetJavaObject().Call("UnLoad");
	}

	private bool Call_Load(string strFileName, int iSeek)
	{
		//return GetJavaObject().Call<bool>("Load", new object[2] { strFileName, iSeek });
		return true;
	}

	private void Call_UpdateVideoTexture()
	{
		GetJavaObject().Call("UpdateVideoTexture");
	}

	private void Call_SetVolume(float fVolume)
	{
		//GetJavaObject().Call("SetVolume", fVolume);
	}

	private void Call_SetSeekPosition(int iSeek)
	{
		GetJavaObject().Call("SetSeekPosition", iSeek);
	}

	private int Call_GetSeekPosition()
	{
		return GetJavaObject().Call<int>("GetSeekPosition", new object[0]);
	}

	private void Call_Play(int iSeek)
	{
		GetJavaObject().Call("Play", iSeek);
	}

	private void Call_Reset()
	{
		GetJavaObject().Call("Reset");
	}

	private void Call_Stop()
	{
		GetJavaObject().Call("Stop");
	}

	private void Call_RePlay()
	{
		GetJavaObject().Call("RePlay");
	}

	private void Call_Pause()
	{
		//GetJavaObject().Call("Pause");
	}

	private int Call_GetVideoWidth()
	{
		return GetJavaObject().Call<int>("GetVideoWidth", new object[0]);
	}

	private int Call_GetVideoHeight()
	{
		return GetJavaObject().Call<int>("GetVideoHeight", new object[0]);
	}

	private void Call_SetUnityTexture(int iTextureID)
	{
		//GetJavaObject().Call("SetUnityTexture", iTextureID);
	}

	private void Call_SetWindowSize()
	{
		GetJavaObject().Call("SetWindowSize");
	}

	private int Call_GetDuration()
	{
		return GetJavaObject().Call<int>("GetDuration", new object[0]);
	}

	private int Call_GetCurrentSeekPercent()
	{
		return GetJavaObject().Call<int>("GetCurrentSeekPercent", new object[0]);
	}

	private int Call_GetError()
	{
		return GetJavaObject().Call<int>("GetError", new object[0]);
	}

	private int Call_GetErrorExtra()
	{
		return GetJavaObject().Call<int>("GetErrorExtra", new object[0]);
	}

	private void Call_SetUnityActivity()
	{
		//AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		//AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		//GetJavaObject().Call("SetUnityActivity", @static);
	}

	/*private MediaPlayerState Call_GetStatus()
	{
		return (MediaPlayerState)GetJavaObject().Call<int>("GetStatus", new object[0]);
	}*/
}
