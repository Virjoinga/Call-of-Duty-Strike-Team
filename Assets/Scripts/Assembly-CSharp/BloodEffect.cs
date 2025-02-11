using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BloodEffect : MonoBehaviour
{
	private Mesh m_mesh;

	private float m_severity;

	private float m_alpha;

	private bool m_isMaterialCloned;

	private Material m_materialBloodEffect;

	private Material m_materialInvulnerabilityEffect;

	private Material m_activeMaterial;

	[SerializeField]
	private bool m_enableDebug;

	public float Severity
	{
		get
		{
			return m_severity;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (num != m_severity)
			{
				m_severity = num;
				OnParametersUpdated();
			}
		}
	}

	public float Alpha
	{
		get
		{
			return m_alpha;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (num != m_alpha)
			{
				m_alpha = num;
				OnParametersUpdated();
			}
		}
	}

	private void Awake()
	{
		m_materialBloodEffect = EffectsController.Instance.Effects.BloodEffect;
		m_materialInvulnerabilityEffect = EffectsController.Instance.Effects.InvulnerabilityEffect;
		CreateMesh();
		CloneMaterial();
		OnParametersUpdated();
		if (m_enableDebug)
		{
			Alpha = 1f;
			Severity = 1f;
		}
		SetAsBloodEffect();
	}

	private void OnDestroy()
	{
		if (m_isMaterialCloned)
		{
			if (m_materialBloodEffect != null)
			{
				Object.Destroy(m_materialBloodEffect);
				m_materialBloodEffect = null;
			}
			if (m_materialInvulnerabilityEffect != null)
			{
				Object.Destroy(m_materialInvulnerabilityEffect);
				m_materialInvulnerabilityEffect = null;
			}
		}
	}

	public void SetAsBloodEffect()
	{
		m_activeMaterial = m_materialBloodEffect;
	}

	public void SetAsInvulnerabilityEffect()
	{
		m_activeMaterial = m_materialInvulnerabilityEffect;
	}

	private void OnPostRender()
	{
		if (m_activeMaterial != null && m_activeMaterial.SetPass(0))
		{
			Graphics.DrawMeshNow(m_mesh, Matrix4x4.identity);
		}
	}

	private void CreateMesh()
	{
		m_mesh = new Mesh();
		m_mesh.name = "Blood";
		int num = 8;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		Vector2[] array3 = new Vector2[num];
		array[0] = new Vector3(-1f, -1f, 0f);
		array2[0] = new Vector2(0f, 0f);
		array3[0] = Vector2.zero;
		array[1] = new Vector3(-1f, -1f, 0f);
		array2[1] = new Vector2(0f, 0f);
		array3[1] = new Vector2(1f, 1f);
		array[2] = new Vector3(-1f, 1f, 0f);
		array2[2] = new Vector2(0f, 1f);
		array3[2] = Vector2.zero;
		array[3] = new Vector3(-1f, 1f, 0f);
		array2[3] = new Vector2(0f, 1f);
		array3[3] = new Vector2(1f, -1f);
		array[4] = new Vector3(1f, 1f, 0f);
		array2[4] = new Vector2(1f, 1f);
		array3[4] = Vector2.zero;
		array[5] = new Vector3(1f, 1f, 0f);
		array2[5] = new Vector2(1f, 1f);
		array3[5] = new Vector2(-1f, -1f);
		array[6] = new Vector3(1f, -1f, 0f);
		array2[6] = new Vector2(1f, 0f);
		array3[6] = Vector2.zero;
		array[7] = new Vector3(1f, -1f, 0f);
		array2[7] = new Vector2(1f, 0f);
		array3[7] = new Vector2(-1f, 1f);
		m_mesh.vertices = array;
		m_mesh.uv = array2;
		m_mesh.uv1 = array3;
		int num2 = 24;
		int[] array4 = new int[num2];
		array4[0] = 0;
		array4[1] = 1;
		array4[2] = 2;
		array4[3] = 2;
		array4[4] = 1;
		array4[5] = 3;
		array4[6] = 2;
		array4[7] = 3;
		array4[8] = 4;
		array4[9] = 4;
		array4[10] = 3;
		array4[11] = 5;
		array4[12] = 4;
		array4[13] = 5;
		array4[14] = 6;
		array4[15] = 6;
		array4[16] = 5;
		array4[17] = 7;
		array4[18] = 6;
		array4[19] = 7;
		array4[20] = 0;
		array4[21] = 0;
		array4[22] = 7;
		array4[23] = 1;
		m_mesh.triangles = array4;
	}

	private void CloneMaterial()
	{
		if (m_materialBloodEffect != null)
		{
			m_materialBloodEffect = new Material(m_materialBloodEffect);
			m_materialBloodEffect.name += " (Clone)";
		}
		if (m_materialInvulnerabilityEffect != null)
		{
			m_materialInvulnerabilityEffect = new Material(m_materialInvulnerabilityEffect);
			m_materialInvulnerabilityEffect.name += " (Clone)";
		}
		m_isMaterialCloned = true;
	}

	private void OnParametersUpdated()
	{
		if (!m_enableDebug)
		{
			base.enabled = m_severity > float.Epsilon && m_alpha >= float.Epsilon;
		}
		if (m_activeMaterial != null)
		{
			Vector4 zero = Vector4.zero;
			Vector4 vector;
			Vector4 vector2;
			if (m_severity > 0.75f)
			{
				zero.z = 1f - (zero.w = Mathf.InverseLerp(0.75f, 1f, m_severity));
				vector = new Vector4(0.4f, 0.4f, 0f, 0f);
				vector2 = new Vector4(0.2f, 0.2f, 0f, 0f);
			}
			else if (m_severity > 0.5f)
			{
				zero.y = 1f - (zero.z = Mathf.InverseLerp(0.5f, 0.75f, m_severity));
				vector = new Vector4(0.3f, 0.3f, 0f, 0f);
				vector2 = new Vector4(0.15f, 0.15f, 0f, 0f);
			}
			else if (m_severity > 0.25f)
			{
				zero.x = 1f - (zero.y = Mathf.InverseLerp(0.25f, 0.5f, m_severity));
				vector = new Vector4(0.2f, 0.2f, 0f, 0f);
				vector2 = new Vector4(0.1f, 0.1f, 0f, 0f);
			}
			else
			{
				float x = Mathf.InverseLerp(0f, 0.25f, m_severity);
				zero.x = x;
				vector = new Vector4(0.1f, 0.1f, 0f, 0f);
				vector2 = new Vector4(0.05f, 0.05f, 0f, 0f);
			}
			m_activeMaterial.SetColor("_Severity", zero);
			m_activeMaterial.SetColor("_PosOffset", vector);
			m_activeMaterial.SetColor("_TexOffset", vector2);
			m_activeMaterial.SetFloat("_Alpha", m_alpha);
		}
	}
}
