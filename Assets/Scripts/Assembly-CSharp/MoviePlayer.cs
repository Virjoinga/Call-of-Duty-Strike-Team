using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class MoviePlayer : MonoBehaviour
{
	public string MovieName;

	public string NextScene;

	public bool LoadIntoMission;

	public bool Wait;

	public string SubtitlesFile;

	public SpriteText SubtitleText;

	public bool forcePlugin;

	public bool hideAnimatedScreenBackground = true;

	private bool mLoadNextSection;

	private Camera mVideoCamera;

	private MediaPlayerCtrl mVideoTexture;

	private SrtLoader mSubtitles;

	private float mPlayTime;

	public VideoPlayer player;

	bool isPlaying;
	public void Start()
	{
		if (Wait)
		{
			StartCoroutine(WaitRoutine());
		}
		else
		{
			PlayMovie();
		}
	}

	private void PlayMovie()
	{
		//PlayHandheld();
		player.Play();

        if (!string.IsNullOrEmpty(SubtitlesFile))
        {
            mSubtitles = new SrtLoader(SubtitlesFile);
        }
    }

	private void Update()
	{
		if (!isPlaying)
		{
			if (player.isPlaying) isPlaying = true;
			return;
		}

		if (mLoadNextSection)
		{
			if (SoundManager.Instance != null)
			{
				SoundManager.Instance.MuteVolumeGroups(false);
			}
			if (LoadIntoMission)
			{
				SceneLoader.AsyncLoadSceneWithLoadingScreen(NextScene);
			}
			else
			{
				Application.LoadLevel(NextScene);
			}
			return;
		}
		mPlayTime = (float)player.time;
		if (SubtitleText != null && mSubtitles != null)
		{
			SubtitleText.Text = mSubtitles.GetTextForTime(mPlayTime);
		}

		if (!player.isPlaying) mLoadNextSection = true;

        for (int i = 0; i < Input.touchCount; i++)
		{
			if (mLoadNextSection)
			{
				break;
			}
			if (Input.GetTouch(i).tapCount > 0)
			{
				mLoadNextSection = true;
				Stop();
			}
		}
	}

	private void Stop()
	{
		player.Pause();
		/*if (mVideoTexture != null)
		{
			mVideoTexture.stop();
		}
		if (mVideoCamera != null)
		{
			if (mVideoCamera.GetComponent<Renderer>() != null)
			{
				mVideoCamera.GetComponent<Renderer>().enabled = false;
			}
			UnityEngine.Object.Destroy(mVideoCamera);
		}*/
	}

	private IEnumerator WaitRoutine()
	{
		yield return new WaitForSeconds(1f);
		PlayMovie();
	}

	private void PlayHandheld()
	{
		bool flag = false;
		if (!string.IsNullOrEmpty(SubtitlesFile) || forcePlugin)
		{
			Stop();
			if (SoundManager.Instance != null)
			{
				SoundManager.Instance.MuteVolumeGroups(true);
			}
			mSubtitles = null;
			if (!string.IsNullOrEmpty(SubtitlesFile))
			{
				mSubtitles = new SrtLoader(SubtitlesFile);
			}
			Camera camera = new GameObject("LiveTextureCamera").AddComponent<Camera>();
			camera.nearClipPlane = -1f;
			camera.farClipPlane = 1f;
			camera.orthographic = true;
			camera.orthographicSize = 1f;
			camera.cullingMask = -1;
			camera.clearFlags = CameraClearFlags.Color;
			camera.backgroundColor = Color.black;
			mVideoCamera = camera;
			float pixelWidth = camera.pixelWidth;
			float pixelHeight = camera.pixelHeight;
			float num = pixelWidth / pixelHeight;
			float num2 = 960f;
			float num3 = 540f;
			float num4 = num2 / num3;
			float num5 = ((!(num < num4)) ? 1f : (num / num4));
			GameObject gameObject = camera.gameObject;
			gameObject.transform.localPosition = Vector3.forward;
			gameObject.layer = LayerMask.NameToLayer("Default");
			Mesh mesh = new Mesh();
			mesh.name = "VideoPlayer";
			mesh.vertices = new Vector3[4]
			{
				num5 * new Vector3(0f - num4, 1f, 0f),
				num5 * new Vector3(num4, 1f, 0f),
				num5 * new Vector3(0f - num4, -1f, 0f),
				num5 * new Vector3(num4, -1f, 0f)
			};
			mesh.uv = new Vector2[4]
			{
				new Vector2(1f, 0f),
				new Vector2(0f, 0f),
				new Vector2(1f, 1f),
				new Vector2(0f, 1f)
			};
			mesh.triangles = new int[6] { 0, 1, 2, 2, 1, 3 };
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			float startAudioVolume = ((!(base.GetComponent<AudioSource>() != null)) ? 1f : 0f);
			mVideoTexture = gameObject.AddComponent<MediaPlayerCtrl>();
			mVideoTexture.Load(MovieName + ".mp4", false, true, startAudioVolume);
			Material material = new Material(Shader.Find("Unlit/Transparent"));
			material.mainTexture = mVideoTexture.texture;
			meshRenderer.material = material;
			MediaPlayerCtrl mediaPlayerCtrl = mVideoTexture;
			mediaPlayerCtrl.videoDidStartEvent = (Action)Delegate.Combine(mediaPlayerCtrl.videoDidStartEvent, (Action)delegate
			{
				if (base.GetComponent<AudioSource>() != null)
				{
					base.GetComponent<AudioSource>().Play();
				}
				mPlayTime = 0f;
				if (hideAnimatedScreenBackground && AnimatedScreenBackground.Instance != null)
				{
					AnimatedScreenBackground.Instance.Hide();
				}
			});
			MediaPlayerCtrl mediaPlayerCtrl2 = mVideoTexture;
			mediaPlayerCtrl2.videoDidFinishEvent = (Action)Delegate.Combine(mediaPlayerCtrl2.videoDidFinishEvent, (Action)delegate
			{
				Stop();
				mLoadNextSection = true;
			});
			flag = true;
		}
		if (!flag)
		{
			Handheld.PlayFullScreenMovie(MovieName + ".mp4", Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
			mLoadNextSection = true;
		}
	}

	public void WPVideoPlayed()
	{
	}
}
