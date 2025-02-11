using System;
using UnityEngine;

[Serializable]
public class RainsplashManager : MonoBehaviour
{
	public int numberOfParticles;

	public float areaSize;

	public float areaHeight;

	public float fallingSpeed;

	public float flakeWidth;

	public float flakeHeight;

	public float flakeRandom;

	public Mesh[] preGennedMeshes;

	private int preGennedIndex;

	public bool generateNewAssetsOnStart;

	public RainsplashManager()
	{
		numberOfParticles = 700;
		areaSize = 40f;
		areaHeight = 15f;
		fallingSpeed = 23f;
		flakeWidth = 0.4f;
		flakeHeight = 0.4f;
		flakeRandom = 0.1f;
	}

	public virtual void Start()
	{
	}

	public virtual Mesh GetPreGennedMesh()
	{
		Mesh[] array = preGennedMeshes;
		int num;
		int num2 = (preGennedIndex = (num = preGennedIndex) + 1);
		return array[num % preGennedMeshes.Length];
	}

	public virtual Mesh CreateMesh()
	{
		Mesh mesh = new Mesh();
		Vector3 value = transform.right * UnityEngine.Random.Range(0.1f, 2f) + transform.forward * UnityEngine.Random.Range(0.1f, 2f);
		value = Vector3.Normalize(value);
		Vector3 value2 = Vector3.Cross(value, Vector3.up);
		value2 = Vector3.Normalize(value2);
		int num = numberOfParticles;
		Vector3[] array = new Vector3[4 * num];
		Vector2[] array2 = new Vector2[4 * num];
		Vector2[] array3 = new Vector2[4 * num];
		Vector3[] array4 = new Vector3[4 * num];
		int[] array5 = new int[6 * num];
		Vector3 vector = default(Vector3);
		for (int i = 0; i < num; i++)
		{
			int num2 = i * 4;
			int num3 = i * 6;
			vector.x = areaSize * (UnityEngine.Random.value - 0.5f);
			vector.y = 0f;
			vector.z = areaSize * (UnityEngine.Random.value - 0.5f);
			float value3 = UnityEngine.Random.value;
			float num4 = flakeWidth + value3 * flakeRandom;
			float num5 = num4;
			array[num2 + 0] = vector - value * num4;
			array[num2 + 1] = vector + value * num4;
			array[num2 + 2] = vector + value * num4 + value2 * 2f * num5;
			array[num2 + 3] = vector - value * num4 + value2 * 2f * num5;
			array4[num2 + 0] = -Camera.main.transform.forward;
			array4[num2 + 1] = -Camera.main.transform.forward;
			array4[num2 + 2] = -Camera.main.transform.forward;
			array4[num2 + 3] = -Camera.main.transform.forward;
			array2[num2 + 0] = new Vector2(0f, 0f);
			array2[num2 + 1] = new Vector2(1f, 0f);
			array2[num2 + 2] = new Vector2(1f, 1f);
			array2[num2 + 3] = new Vector2(0f, 1f);
			Vector2 vector2 = new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
			array3[num2 + 0] = new Vector2(vector2.x, vector2.y);
			array3[num2 + 1] = new Vector2(vector2.x, vector2.y);
			array3[num2 + 2] = new Vector2(vector2.x, vector2.y);
			array3[num2 + 3] = new Vector2(vector2.x, vector2.y);
			array5[num3 + 0] = num2 + 0;
			array5[num3 + 1] = num2 + 1;
			array5[num3 + 2] = num2 + 2;
			array5[num3 + 3] = num2 + 0;
			array5[num3 + 4] = num2 + 2;
			array5[num3 + 5] = num2 + 3;
		}
		mesh.vertices = array;
		mesh.triangles = array5;
		mesh.normals = array4;
		mesh.uv = array2;
		mesh.uv2 = array3;
		mesh.RecalculateBounds();
		return mesh;
	}
}
