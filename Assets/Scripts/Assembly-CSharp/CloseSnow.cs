using System.Collections;
using UnityEngine;

public class CloseSnow : MonoBehaviour
{
	private float m_alpha;

	private float m_snowAlpha = 1f;

	private Vector3 m_windDir;

	private Vector3 m_cameraPos;

	private float m_windSpeed = 1f;

	private float m_windDirTime;

	private static MaterialPropertyBlock s_properties;

	private static int g_cameraForward;

	private static int g_cameraRight;

	private static int g_cameraUp;

	private static int g_cameraPos;

	private static int g_cameraVel;

	private static int g_closeSnowData;

	private static int g_closeSnowData2;

	private static int g_closeSnowOffset;

	private static int g_closeSnowFade;

	private static int g_closeSnowWindData;

	public int m_numParticles = 4000;

	public float m_wrapSize = 4f;

	public float m_wrapSizeY = -1f;

	public float m_windDirY;

	public float m_windVariation = 1f;

	public float m_speed = 1f;

	public float m_forwardRayDist = 10f;

	public float m_fowardRayCutoff = 1f;

	public float m_fadeRate = 2f;

	public float m_fadeScale = 0.5f;

	public bool m_rayCastForWindDir;

	public float m_windScale = 1f;

	public float m_randomScale = 0.2f;

	public float m_sizeOverride = -1f;

	public float m_alphaOverride = -1f;

	public float m_cameraMotionBlurOverride = -1f;

	public float m_posScaleOverride = -1f;

	private bool m_Enabled = true;

	public bool m_disableWrapOffset;

	public bool m_forwardCameraVelocityOnly;

	private static bool g_CutSceneSwitch = true;

	protected virtual Camera CameraToUse
	{
		get
		{
			if (CameraManager.Instance != null)
			{
				return CameraManager.Instance.PlayCamera;
			}
			Transform transform = null;
			if (Camera.main != null)
			{
				transform = Camera.main.transform;
			}
			if (transform == null)
			{
				Camera[] allCameras = Camera.allCameras;
				if (allCameras.Length > 0)
				{
					transform = allCameras[0].transform;
				}
			}
			return transform.camera;
		}
	}

	private void Awake()
	{
		if (!OptimisationManager.CanUseOptmisation(OptimisationManager.OptimisationType.Weather))
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (m_wrapSizeY <= 0f)
		{
			m_wrapSizeY = m_wrapSize;
		}
		CreatePropertyBlock();
		base.gameObject.AddComponent<MeshFilter>();
		BuildMesh();
		MeshRenderer component = GetComponent<MeshRenderer>();
		if (m_sizeOverride > 0f || m_alphaOverride > 0f || m_cameraMotionBlurOverride > 0f || m_posScaleOverride > 0f)
		{
			Material material = new Material(component.material);
			if (m_sizeOverride > 0f)
			{
				material.SetFloat("_Size", m_sizeOverride);
			}
			if (m_alphaOverride > 0f)
			{
				material.SetFloat("_Alpha", m_alphaOverride);
			}
			if (m_cameraMotionBlurOverride > 0f)
			{
				material.SetFloat("_CameraMotionBlur", m_cameraMotionBlurOverride);
			}
			if (m_posScaleOverride > 0f)
			{
				material.SetFloat("_PosScale", m_posScaleOverride);
			}
			component.material = material;
		}
	}

	private void OnDestroy()
	{
		Object.DestroyImmediate(base.renderer.material);
	}

	private void CreatePropertyBlock()
	{
		if (s_properties == null)
		{
			s_properties = new MaterialPropertyBlock();
			g_cameraForward = Shader.PropertyToID("g_cameraForward");
			g_cameraRight = Shader.PropertyToID("g_cameraRight");
			g_cameraUp = Shader.PropertyToID("g_cameraUp");
			g_cameraPos = Shader.PropertyToID("g_cameraPos");
			g_cameraVel = Shader.PropertyToID("g_cameraVel");
			g_closeSnowData = Shader.PropertyToID("g_closeSnowData");
			g_closeSnowData2 = Shader.PropertyToID("g_closeSnowData2");
			g_closeSnowOffset = Shader.PropertyToID("g_closeSnowOffset");
			g_closeSnowFade = Shader.PropertyToID("g_closeSnowFade");
			g_closeSnowWindData = Shader.PropertyToID("g_closeSnowWindData");
		}
	}

	private IEnumerator Fade(bool _enable, float _fadeTime)
	{
		base.renderer.enabled = true;
		float start = m_snowAlpha;
		float end = ((!_enable) ? 0f : 1f);
		float time = 0f;
		while (time < _fadeTime)
		{
			time += Time.deltaTime;
			m_snowAlpha = Mathf.Lerp(start, end, Mathf.Min(time / _fadeTime, 1f));
			yield return null;
		}
		base.renderer.enabled = _enable;
	}

	public void Toggle(bool _enable, float _fadeTime)
	{
		if (_fadeTime == 0f)
		{
			if (m_numParticles > 0)
			{
				base.renderer.enabled = _enable;
			}
		}
		else
		{
			StopAllCoroutines();
			StartCoroutine(Fade(_enable, _fadeTime));
		}
	}

	public void BuildMesh()
	{
		m_numParticles /= 4;
		if (m_numParticles <= 0)
		{
			base.renderer.enabled = false;
			return;
		}
		Mesh mesh = new Mesh();
		int numParticles = m_numParticles;
		int num = numParticles * 4;
		int num2 = numParticles * 6;
		Vector3[] array = new Vector3[num];
		Color[] array2 = new Color[num];
		Vector2[] array3 = new Vector2[num];
		Vector3[] array4 = new Vector3[num];
		m_windDir = Wind.direction;
		m_windDir.y = m_windDirY;
		m_windDir.Normalize();
		int num3 = 0;
		for (int i = 0; i < numParticles; i++)
		{
			Vector3 vector = new Vector3(Random.Range(0f, m_wrapSize), Random.Range(0f, m_wrapSizeY), Random.Range(0f, m_wrapSize));
			Vector3 vector2 = (m_windDir * m_windScale + Random.insideUnitSphere * m_randomScale) * m_speed;
			Color color = new Color(Random.value, Random.value, Random.value, Random.value);
			array[num3] = vector;
			array3[num3].x = 0f;
			array3[num3].y = 0f;
			array4[num3] = vector2;
			array2[num3] = color;
			num3++;
			array[num3] = vector;
			array3[num3].x = 1f;
			array3[num3].y = 0f;
			array4[num3] = vector2;
			array2[num3] = color;
			num3++;
			array[num3] = vector;
			array3[num3].x = 0f;
			array3[num3].y = 1f;
			array4[num3] = vector2;
			array2[num3] = color;
			num3++;
			array[num3] = vector;
			array3[num3].x = 1f;
			array3[num3].y = 1f;
			array4[num3] = vector2;
			array2[num3] = color;
			num3++;
		}
		mesh.vertices = array;
		mesh.colors = array2;
		mesh.uv = array3;
		mesh.normals = array4;
		int[] array5 = new int[num2];
		int num4 = 0;
		int num5 = 0;
		for (int j = 0; j < numParticles; j++)
		{
			array5[num5++] = num4;
			array5[num5++] = num4 + 1;
			array5[num5++] = num4 + 2;
			array5[num5++] = num4 + 2;
			array5[num5++] = num4 + 1;
			array5[num5++] = num4 + 3;
			num4 += 4;
		}
		mesh.triangles = array5;
		MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
		component.mesh = mesh;
		mesh.bounds = new Bounds(Vector3.zero, new Vector3(100000000f, 100000000f, 100000000f));
		base.renderer.enabled = true;
	}

	private void LateUpdate()
	{
		m_windSpeed = 0.8f + Mathf.Sin(Time.time * 0.5f) * 0.5f * m_windVariation;
		m_windSpeed += Mathf.Sin(Time.time * 2.43f) * 0.15f * m_windVariation;
		m_windDirTime += Time.deltaTime * m_windSpeed;
		Camera cameraToUse = CameraToUse;
		if (!(cameraToUse != null) || !cameraToUse.enabled)
		{
			return;
		}
		Transform transform = cameraToUse.transform;
		s_properties.Clear();
		Vector3 forward = transform.forward;
		s_properties.AddVector(g_cameraForward, new Vector4(forward.x, forward.y, forward.z, 0f));
		Vector3 right = transform.right;
		s_properties.AddVector(g_cameraRight, new Vector4(right.x, right.y, right.z, 0f));
		Vector3 up = transform.up;
		s_properties.AddVector(g_cameraUp, new Vector4(up.x, up.y, up.z, 0f));
		Vector3 position = transform.position;
		s_properties.AddVector(g_cameraPos, new Vector4(position.x, position.y, position.z, 1f));
		Vector3 vector = position - m_cameraPos;
		m_cameraPos = position;
		vector -= (vector - Vector3.Dot(vector, forward) * forward) * 0.5f;
		vector = Vector3.ClampMagnitude(vector, 0.5f);
		if (m_forwardCameraVelocityOnly)
		{
			float num = Mathf.Max(0f, Vector3.Dot(vector, forward));
			Vector3 vector2 = num * forward;
			s_properties.AddVector(g_cameraVel, new Vector4(vector2.x, vector2.y, vector2.z, 1f));
		}
		else
		{
			s_properties.AddVector(g_cameraVel, new Vector4(vector.x, vector.y, vector.z, 1f));
		}
		float num2 = m_wrapSize * 0.5f;
		float y = m_wrapSizeY * 0.5f;
		Vector3 vector3 = position - new Vector3(num2, y, num2);
		float num3 = 1f / m_wrapSize;
		float num4 = 1f / m_wrapSizeY;
		Vector3 vector4 = new Vector3(vector3.x * num3, vector3.y * num4, vector3.z * num3);
		Vector3 vector5 = new Vector3(Mathf.Floor(vector4.x), Mathf.Floor(vector4.y), Mathf.Floor(vector4.z));
		Vector3 vector6 = ((!m_disableWrapOffset) ? (vector5 - vector4) : Vector3.zero);
		s_properties.AddVector(g_closeSnowData, new Vector4(vector6.x, vector6.y, vector6.z, num3));
		s_properties.AddVector(g_closeSnowData2, new Vector4(num4, m_wrapSizeY, 0f, 0f));
		s_properties.AddVector(g_closeSnowOffset, new Vector4(vector3.x, vector3.y, vector3.z, m_wrapSize));
		bool flag = false;
		if (GameController.Instance != null && GameController.Instance.mFirstPersonActor != null && GameController.Instance.mFirstPersonActor.realCharacter.Location != null)
		{
			flag = true;
		}
		InteractionsManager instance = InteractionsManager.Instance;
		bool flag2 = false;
		if (instance != null && instance.CurrentMustSeeAction != null && instance.CurrentMustSeeAction.GameTask != null && instance.CurrentMustSeeAction.GameTask.GetType() == typeof(TaskPeekaboo))
		{
			flag2 = true;
			flag = true;
		}
		if (!g_CutSceneSwitch)
		{
			flag = true;
		}
		if (flag && m_Enabled)
		{
			if (flag2)
			{
				Toggle(false, 1f);
			}
			else
			{
				Toggle(false, 0.3f);
			}
			m_Enabled = false;
		}
		else if (!flag && !m_Enabled)
		{
			Toggle(true, 1f);
			m_Enabled = true;
		}
		float num5 = 1f;
		if (!flag)
		{
			RaycastHit hitInfo;
			if (m_rayCastForWindDir && Physics.Raycast(position, -m_windDir, out hitInfo, m_forwardRayDist))
			{
				num5 = Mathf.Min(num5, Mathf.Max(0f, (hitInfo.distance - m_fowardRayCutoff) / (m_forwardRayDist - m_fowardRayCutoff)));
			}
			if (Physics.Raycast(position, transform.forward, out hitInfo, m_forwardRayDist, 1 << LayerMask.NameToLayer("Default")) && hitInfo.collider as MeshCollider != null)
			{
				num5 = Mathf.Min(num5, Mathf.Max(0f, (hitInfo.distance - m_fowardRayCutoff) / (m_forwardRayDist - m_fowardRayCutoff)));
			}
		}
		m_alpha = Mathf.Lerp(m_alpha, num5, m_fadeRate * Time.deltaTime);
		if (m_alpha < 0.001f)
		{
			m_alpha = 0f;
		}
		else if (m_alpha > 0.999f)
		{
			m_alpha = 1f;
		}
		float num6 = Mathf.Min(m_alpha, m_snowAlpha);
		Vector4 value = new Vector4(m_snowAlpha - num6, num6, m_fowardRayCutoff * (0f - m_fadeScale), m_fadeScale);
		s_properties.AddVector(g_closeSnowFade, value);
		s_properties.AddVector(g_closeSnowWindData, new Vector4(m_windDirTime, m_windSpeed, m_windDirTime, m_windDirTime));
		base.renderer.SetPropertyBlock(s_properties);
	}

	public static void CutSceneWeatherSwitch(bool bOnOff)
	{
		g_CutSceneSwitch = bOnOff;
	}
}
