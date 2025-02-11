using System;
using System.Collections;
using UnityEngine;

public class ScreenWipe : MonoBehaviour
{
	public enum ZoomType
	{
		Grow = 0,
		Shrink = 1
	}

	public enum TransitionType
	{
		Left = 0,
		Right = 1,
		Up = 2,
		Down = 3
	}

	private Texture tex;

	private RenderTexture renderTex;

	private Texture2D tex2D;

	private float alpha;

	private bool reEnableListener;

	private Material shapeMaterial;

	private Transform shape;

	private AnimationCurve curve;

	private bool useCurve;

	public static ScreenWipe use;

	public int planeResolution = 90;

	private Vector3[] baseVertices;

	private Vector3[] newVertices;

	private Material planeMaterial;

	private GameObject plane;

	private RenderTexture renderTex2;

	private void Awake()
	{
		if ((bool)use)
		{
			Debug.LogWarning("Only one instance of ScreenCrossFadePro is allowed");
			return;
		}
		use = this;
		base.enabled = false;
	}

	private void OnGUI()
	{
		GUI.depth = -9999999;
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), tex);
	}

	private IEnumerator AlphaTimer(float time)
	{
		float rate = 1f / time;
		if (useCurve)
		{
			float t = 0f;
			while (t < 1f)
			{
				alpha = curve.Evaluate(t);
				t += TimeManager.DeltaTime * rate;
				yield return 0;
			}
		}
		else
		{
			for (alpha = 1f; alpha > 0f; alpha -= TimeManager.DeltaTime * rate)
			{
				yield return 0;
			}
		}
	}

	private void CameraSetup(Camera cam1, Camera cam2, bool cam1Active, bool enableThis)
	{
		if (enableThis)
		{
			base.enabled = true;
		}
		cam1.gameObject.SetActive(cam1Active);
		cam2.gameObject.SetActive(true);
		AudioListener component = cam2.GetComponent<AudioListener>();
		if ((bool)component)
		{
			reEnableListener = (component.enabled ? true : false);
			component.enabled = false;
		}
	}

	private void CameraCleanup(Camera cam1, Camera cam2)
	{
		AudioListener component = cam2.GetComponent<AudioListener>();
		if ((bool)component && reEnableListener)
		{
			component.enabled = true;
		}
		cam1.gameObject.SetActive(false);
		base.enabled = false;
	}

	public IEnumerator CrossFadePro(Camera cam1, Camera cam2, float time)
	{
		if (!renderTex)
		{
			renderTex = new RenderTexture(Screen.width, Screen.height, 24);
			renderTex.name = "CrossFadePro";
		}
		cam1.targetTexture = renderTex;
		tex = renderTex;
		CameraSetup(cam1, cam2, true, true);
		yield return StartCoroutine(AlphaTimer(time));
		cam1.targetTexture = null;
		renderTex.Release();
		CameraCleanup(cam1, cam2);
	}

	private IEnumerator CrossFade(Camera cam1, Camera cam2, float time)
	{
		yield return CrossFade(cam1, cam2, time, null);
	}

	public IEnumerator CrossFade(Camera cam1, Camera cam2, float time, AnimationCurve _curve)
	{
		curve = _curve;
		useCurve = curve != null;
		if (!tex2D)
		{
			tex2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		}
		tex2D.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0, false);
		tex2D.Apply();
		tex = tex2D;
		yield return 0;
		CameraSetup(cam1, cam2, false, true);
		yield return AlphaTimer(time);
		CameraCleanup(cam1, cam2);
	}

	private IEnumerator RectWipe(Camera cam1, Camera cam2, float time, ZoomType zoom)
	{
		yield return RectWipe(cam1, cam2, time, zoom, null);
	}

	public IEnumerator RectWipe(Camera cam1, Camera cam2, float time, ZoomType zoom, AnimationCurve _curve)
	{
		curve = _curve;
		useCurve = curve != null;
		CameraSetup(cam1, cam2, true, false);
		Camera useCam = ((zoom != ZoomType.Shrink) ? cam2 : cam1);
		Camera otherCam = ((zoom != ZoomType.Shrink) ? cam1 : cam2);
		Rect originalRect = useCam.rect;
		float originalDepth = useCam.depth;
		useCam.depth = otherCam.depth + 1f;
		if (useCurve)
		{
			float rate2 = 1f / time;
			if (zoom == ZoomType.Shrink)
			{
				for (float l = 0f; l < 1f; l += TimeManager.DeltaTime * rate2)
				{
					float t4 = curve.Evaluate(l) * 0.5f;
					cam1.rect = new Rect(t4, t4, 1f - t4 * 2f, 1f - t4 * 2f);
					yield return 0;
				}
			}
			else
			{
				for (float k = 0f; k < 1f; k += TimeManager.DeltaTime * rate2)
				{
					float t3 = curve.Evaluate(k) * 0.5f;
					cam2.rect = new Rect(0.5f - t3, 0.5f - t3, t3 * 2f, t3 * 2f);
					yield return 0;
				}
			}
		}
		else
		{
			float rate = 1f / (time * 2f);
			if (zoom == ZoomType.Shrink)
			{
				for (float j = 0f; (double)j < 0.5; j += TimeManager.DeltaTime * rate)
				{
					float t2 = Mathf.Lerp(0f, 0.5f, Mathf.Sin(j * (float)Math.PI));
					cam1.rect = new Rect(t2, t2, 1f - t2 * 2f, 1f - t2 * 2f);
					yield return 0;
				}
			}
			else
			{
				for (float i = 0f; i < 0.5f; i += TimeManager.DeltaTime * rate)
				{
					float t = Mathf.Lerp(0.5f, 0f, Mathf.Sin((0.5f - i) * (float)Math.PI));
					cam2.rect = new Rect(0.5f - t, 0.5f - t, t * 2f, t * 2f);
					yield return 0;
				}
			}
		}
		useCam.rect = originalRect;
		useCam.depth = originalDepth;
		CameraCleanup(cam1, cam2);
	}

	private IEnumerator ShapeWipe(Camera cam1, Camera cam2, float time, ZoomType zoom, Mesh mesh, float rotateAmount)
	{
		yield return ShapeWipe(cam1, cam2, time, zoom, mesh, rotateAmount, null);
	}

	public IEnumerator ShapeWipe(Camera cam1, Camera cam2, float time, ZoomType zoom, Mesh mesh, float rotateAmount, AnimationCurve _curve)
	{
		curve = _curve;
		useCurve = curve != null;
		if (!shapeMaterial)
		{
			shapeMaterial = new Material("Shader \"DepthMask\" {   SubShader {\t   Tags { \"Queue\" = \"Background\" }\t   Lighting Off ZTest LEqual ZWrite On Cull Off ColorMask 0\t   Pass {}   }}");
		}
		if (!shape)
		{
			GameObject gobjShape = new GameObject("Shape");
			gobjShape.AddComponent("MeshFilter");
			gobjShape.AddComponent("MeshRenderer");
			shape = gobjShape.transform;
			shape.renderer.material = shapeMaterial;
		}
		CameraSetup(cam1, cam2, true, false);
		Camera useCam = ((zoom != ZoomType.Shrink) ? cam2 : cam1);
		Camera otherCam = ((zoom != ZoomType.Shrink) ? cam1 : cam2);
		float originalDepth = otherCam.depth;
		CameraClearFlags originalClearFlags = otherCam.clearFlags;
		otherCam.depth = useCam.depth + 1f;
		otherCam.clearFlags = CameraClearFlags.Depth;
		shape.gameObject.SetActive(true);
		shape.GetComponent<MeshFilter>().mesh = mesh;
		shape.position = otherCam.transform.position + otherCam.transform.forward * (otherCam.nearClipPlane + 0.01f);
		shape.parent = otherCam.transform;
		shape.localRotation = Quaternion.identity;
		if (useCurve)
		{
			float rate2 = 1f / time;
			if (zoom == ZoomType.Shrink)
			{
				for (float l = 1f; l > 0f; l -= TimeManager.DeltaTime * rate2)
				{
					float t4 = curve.Evaluate(l);
					shape.localScale = new Vector3(t4, t4, t4);
					shape.localEulerAngles = new Vector3(0f, 0f, l * rotateAmount);
					yield return 0;
				}
			}
			else
			{
				for (float k = 0f; k < 1f; k += TimeManager.DeltaTime * rate2)
				{
					float t3 = curve.Evaluate(k);
					shape.localScale = new Vector3(t3, t3, t3);
					shape.localEulerAngles = new Vector3(0f, 0f, (0f - k) * rotateAmount);
					yield return 0;
				}
			}
		}
		else
		{
			float rate = 1f / time;
			if (zoom == ZoomType.Shrink)
			{
				for (float j = 1f; j > 0f; j -= TimeManager.DeltaTime * rate)
				{
					float t2 = Mathf.Lerp(1f, 0f, Mathf.Sin((1f - j) * (float)Math.PI * 0.5f));
					shape.localScale = new Vector3(t2, t2, t2);
					shape.localEulerAngles = new Vector3(0f, 0f, j * rotateAmount);
					yield return 0;
				}
			}
			else
			{
				for (float i = 0f; i < 1f; i += TimeManager.DeltaTime * rate)
				{
					float t = Mathf.Lerp(1f, 0f, Mathf.Sin((1f - i) * (float)Math.PI * 0.5f));
					shape.localScale = new Vector3(t, t, t);
					shape.localEulerAngles = new Vector3(0f, 0f, (0f - i) * rotateAmount);
					yield return 0;
				}
			}
		}
		otherCam.clearFlags = originalClearFlags;
		otherCam.depth = originalDepth;
		CameraCleanup(cam1, cam2);
		shape.parent = null;
		shape.gameObject.SetActive(false);
	}

	private IEnumerator SquishWipe(Camera cam1, Camera cam2, float time, TransitionType transitionType)
	{
		yield return SquishWipe(cam1, cam2, time, transitionType, null);
	}

	public IEnumerator SquishWipe(Camera cam1, Camera cam2, float time, TransitionType transitionType, AnimationCurve _curve)
	{
		curve = _curve;
		useCurve = curve != null;
		Rect originalCam1Rect = cam1.rect;
		Rect originalCam2Rect = cam2.rect;
		Matrix4x4 cam1Matrix = cam1.projectionMatrix;
		Matrix4x4 cam2Matrix = cam2.projectionMatrix;
		CameraSetup(cam1, cam2, true, false);
		float rate = 1f / time;
		float t = 0f;
		float i = 0f;
		while (i < 1f)
		{
			if (useCurve)
			{
				i = curve.Evaluate(t);
				t += TimeManager.DeltaTime * rate;
			}
			else
			{
				i += TimeManager.DeltaTime * rate;
			}
			switch (transitionType)
			{
			case TransitionType.Right:
				cam1.rect = new Rect(i, 0f, 1f, 1f);
				cam2.rect = new Rect(0f, 0f, i, 1f);
				break;
			case TransitionType.Left:
				cam1.rect = new Rect(0f, 0f, 1f - i, 1f);
				cam2.rect = new Rect(1f - i, 0f, 1f, 1f);
				break;
			case TransitionType.Up:
				cam1.rect = new Rect(0f, i, 1f, 1f);
				cam2.rect = new Rect(0f, 0f, 1f, i);
				break;
			case TransitionType.Down:
				cam1.rect = new Rect(0f, 0f, 1f, 1f - i);
				cam2.rect = new Rect(0f, 1f - i, 1f, 1f);
				break;
			}
			cam1.projectionMatrix = cam1Matrix;
			cam2.projectionMatrix = cam2Matrix;
			yield return 0;
		}
		cam1.rect = originalCam1Rect;
		cam2.rect = originalCam2Rect;
		CameraCleanup(cam1, cam2);
	}

	public void InitializeDreamWipe()
	{
		if ((bool)planeMaterial && (bool)plane)
		{
			return;
		}
		Debug.Log("InitializeDreamWipe here!");
		planeMaterial = new Material("Shader \"Unlit2Pass\" {Properties {\t_Color (\"Main Color\", Color) = (1,1,1,1)\t_Tex1 (\"Base\", Rect) = \"white\" {}\t_Tex2 (\"Base\", Rect) = \"white\" {}}Category {\tZWrite Off Alphatest Greater 0 ColorMask RGB Lighting Off\tTags {\"Queue\"=\"Transparent\" \"IgnoreProjector\"=\"True\" \"RenderType\"=\"Transparent\"}\tBlend SrcAlpha OneMinusSrcAlpha\tSubShader {\t\tPass {SetTexture [_Tex2]}\t\tPass {SetTexture [_Tex1] {constantColor [_Color] Combine texture * constant, texture * constant}}\t}}}");
		plane = new GameObject("Plane");
		plane.AddComponent("MeshFilter");
		plane.AddComponent("MeshRenderer");
		plane.renderer.material = planeMaterial;
		plane.renderer.castShadows = false;
		plane.renderer.receiveShadows = false;
		plane.renderer.enabled = false;
		Mesh mesh = new Mesh();
		plane.GetComponent<MeshFilter>().mesh = mesh;
		planeResolution = Mathf.Clamp(planeResolution, 1, 16380);
		baseVertices = new Vector3[4 * planeResolution + 4];
		newVertices = new Vector3[baseVertices.Length];
		Vector2[] array = new Vector2[baseVertices.Length];
		int[] array2 = new int[18 * planeResolution];
		int num = 0;
		for (int i = 0; i <= planeResolution; i++)
		{
			float num2 = 1f / (float)planeResolution * (float)i;
			array[num] = new Vector2(0f, 1f - num2);
			baseVertices[num++] = new Vector3(-1f, 0.5f - num2, 0f);
			array[num] = new Vector2(0f, 1f - num2);
			baseVertices[num++] = new Vector3(-0.5f, 0.5f - num2, 0f);
			array[num] = new Vector2(1f, 1f - num2);
			baseVertices[num++] = new Vector3(0.5f, 0.5f - num2, 0f);
			array[num] = new Vector2(1f, 1f - num2);
			baseVertices[num++] = new Vector3(1f, 0.5f - num2, 0f);
		}
		num = 0;
		for (int j = 0; j < planeResolution; j++)
		{
			for (int k = 0; k < 3; k++)
			{
				array2[num++] = j * 4 + k;
				array2[num++] = j * 4 + k + 1;
				array2[num++] = (j + 1) * 4 + k;
				array2[num++] = (j + 1) * 4 + k;
				array2[num++] = j * 4 + k + 1;
				array2[num++] = (j + 1) * 4 + k + 1;
			}
		}
		mesh.vertices = baseVertices;
		mesh.uv = array;
		mesh.triangles = array2;
		renderTex = new RenderTexture(Screen.width, Screen.height, 24);
		renderTex2 = new RenderTexture(Screen.width, Screen.height, 24);
		renderTex.name = "renderTex";
		renderTex2.name = "renderTex2";
		RenderTexture renderTexture = renderTex;
		FilterMode filterMode = FilterMode.Point;
		renderTex2.filterMode = filterMode;
		renderTexture.filterMode = filterMode;
		planeMaterial.SetTexture("_Tex1", renderTex);
		planeMaterial.SetTexture("_Tex2", renderTex2);
	}

	public IEnumerator DreamWipe(Camera cam1, Camera cam2, float time, float waveScale, float waveFrequency)
	{
		if (!plane)
		{
			InitializeDreamWipe();
		}
		waveScale = Mathf.Clamp(waveScale, -0.5f, 0.5f);
		waveFrequency = 0.25f / ((float)planeResolution / waveFrequency);
		CameraSetup(cam1, cam2, true, false);
		Debug.Log("DreamWipe() begun");
		Camera cam2Clone = UnityEngine.Object.Instantiate(cam2, cam2.transform.position, cam2.transform.rotation) as Camera;
		cam2Clone.depth = cam1.depth + 1f;
		if (cam2Clone.depth <= cam2.depth)
		{
			cam2Clone.depth = cam2.depth + 1f;
		}
		cam2Clone.nearClipPlane = 0.5f;
		cam2Clone.farClipPlane = 1f;
		Vector3 p = cam2Clone.transform.InverseTransformPoint(cam2.ScreenToWorldPoint(new Vector3(0f, 0f, cam2Clone.nearClipPlane)));
		plane.transform.localScale = new Vector3((0f - p.x) * 2f, (0f - p.y) * 2f, 1f);
		plane.transform.parent = cam2Clone.transform;
		Transform obj = plane.transform;
		Vector3 zero = Vector3.zero;
		plane.transform.localEulerAngles = zero;
		obj.localPosition = zero;
		plane.transform.Translate(Vector3.forward * (cam2Clone.nearClipPlane + 5E-05f));
		cam2Clone.transform.Translate(Vector3.forward * -1f);
		cam2Clone.transform.parent = cam2.transform;
		plane.renderer.enabled = true;
		float scale = 0f;
		Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;
		cam1.targetTexture = renderTex;
		cam2.targetTexture = renderTex2;
		float rate = 1f / time;
		for (float i = 0f; i < 1f; i += TimeManager.DeltaTime * rate)
		{
			planeMaterial.color = new Color(planeMaterial.color.r, planeMaterial.color.g, planeMaterial.color.b, Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.75f, 0.15f, i)));
			for (int j = 0; j < newVertices.Length; j++)
			{
				newVertices[j] = baseVertices[j];
				newVertices[j].x += Mathf.Sin((float)j * waveFrequency + i * time) * scale;
			}
			planeMesh.vertices = newVertices;
			scale = Mathf.Sin((float)Math.PI * Mathf.SmoothStep(0f, 1f, i)) * waveScale;
			yield return 0;
		}
		Debug.Log("DreamWipe() finished");
		CameraCleanup(cam1, cam2);
		plane.renderer.enabled = false;
		plane.transform.parent = null;
		UnityEngine.Object.Destroy(cam2Clone.gameObject);
		RenderTexture targetTexture = (cam2.targetTexture = null);
		cam1.targetTexture = targetTexture;
		renderTex.Release();
		renderTex2.Release();
	}
}
