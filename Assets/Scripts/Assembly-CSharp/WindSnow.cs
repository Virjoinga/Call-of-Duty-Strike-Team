using UnityEngine;

public class WindSnow : MonoBehaviour
{
	public enum Speed
	{
		Slow = 0,
		Medium = 1,
		Fast = 2
	}

	public static bool s_windSnowEnabled = true;

	private bool m_init;

	public float m_length = 10f;

	public float m_width = 10f;

	public float m_uSize = 10f;

	public float m_vSize = 10f;

	public float m_alpha = 1f;

	public bool m_side;

	public Speed m_speed = Speed.Medium;

	private void Start()
	{
		if (!s_windSnowEnabled)
		{
			return;
		}
		Vector3 vector = -Vector3.right;
		Vector3 forward = Vector3.forward;
		Color black = Color.black;
		black.a = 1f;
		switch (m_speed)
		{
		case Speed.Slow:
			black.r = 1f;
			break;
		case Speed.Medium:
			black.g = 1f;
			break;
		case Speed.Fast:
			black.b = 1f;
			break;
		}
		float value = Random.value;
		Mesh mesh = new Mesh();
		int num = 6;
		int num2 = 6;
		int num3 = (num + 1) * (num2 + 1);
		int num4 = num * num2 * 6;
		int num5 = 2;
		Vector3[] array = new Vector3[num3];
		Color[] array2 = new Color[num3];
		Vector2[] array3 = new Vector2[num3];
		float num6 = 0.17322835f;
		float num7 = (float)num5 / (float)num2;
		float num8 = 3.5f;
		float num9 = 3.5f;
		float num10 = 3.5f;
		float num11 = 1f / m_uSize;
		float num12 = 1f / m_vSize;
		int num13 = 0;
		for (int i = 0; i < num2 + 1; i++)
		{
			float num14 = (float)i / (float)num2;
			num14 = ((!(num14 < num7)) ? ((num14 - num7) / (1f - num7) * (1f - num6) + num6) : (num14 / num7 * num6));
			float num15;
			if (num14 > num6)
			{
				num15 = (num14 - num6) / (1f - num6);
				num15 = Mathf.Pow(2.718f, (0f - num10) * (num15 * num15));
			}
			else
			{
				num15 = num14 / num6;
				num15 = Mathf.Pow(2.718f, (0f - num9) * ((1f - num15) * (1f - num15)));
			}
			if (i == 0 || i == num2)
			{
				num15 = 0f;
			}
			for (int j = 0; j < num + 1; j++)
			{
				float num16 = (float)j / (float)num;
				float num17 = Mathf.Abs(num16 - 0.5f) * 2f;
				num17 = Mathf.Pow(2.718f, (0f - num8) * (num17 * num17));
				if (j == 0 || j == num)
				{
					num17 = 0f;
				}
				if (m_side)
				{
					num17 = 1f;
				}
				array[num13] = vector * (m_width * (num16 - 0.5f)) + forward * (m_length * num14);
				array2[num13] = black;
				array2[num13].a *= num17 * num15 * m_alpha;
				array3[num13].x = num14 * m_length * num11 + value;
				if (m_side)
				{
					array3[num13].y = num16;
				}
				else
				{
					array3[num13].y = num16 * m_width * num12;
				}
				num13++;
			}
		}
		mesh.vertices = array;
		mesh.colors = array2;
		mesh.uv = array3;
		int[] array4 = new int[num4];
		int num18 = 0;
		int num19 = 0;
		for (int k = 0; k < num2; k++)
		{
			for (int l = 0; l < num; l++)
			{
				bool flag = false;
				if (l >= num / 2)
				{
					flag = true;
				}
				if (k >= num5)
				{
					flag = !flag;
				}
				if (flag)
				{
					array4[num19++] = num18 + 1;
					array4[num19++] = num18 + 1 + (num + 1);
					array4[num19++] = num18;
					array4[num19++] = num18;
					array4[num19++] = num18 + 1 + (num + 1);
					array4[num19++] = num18 + (num + 1);
				}
				else
				{
					array4[num19++] = num18;
					array4[num19++] = num18 + 1;
					array4[num19++] = num18 + (num + 1);
					array4[num19++] = num18 + (num + 1);
					array4[num19++] = num18 + 1;
					array4[num19++] = num18 + 1 + (num + 1);
				}
				num18++;
			}
			num18++;
		}
		mesh.triangles = array4;
		MeshFilter meshFilter = base.gameObject.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;
		mesh.RecalculateBounds();
		m_init = true;
		bool flag2 = true;
		WindSnow[] array5 = Object.FindObjectsOfType(typeof(WindSnow)) as WindSnow[];
		WindSnow[] array6 = array5;
		foreach (WindSnow windSnow in array6)
		{
			if (!windSnow.m_init)
			{
				flag2 = false;
				break;
			}
		}
		if (flag2)
		{
			GameObject[] array7 = new GameObject[array5.Length];
			for (int n = 0; n < array5.Length; n++)
			{
				array7[n] = array5[n].gameObject;
			}
			GameObject staticBatchRoot = new GameObject("WindSnow Batch Object");
			StaticBatchingUtility.Combine(array7, staticBatchRoot);
		}
	}
}
