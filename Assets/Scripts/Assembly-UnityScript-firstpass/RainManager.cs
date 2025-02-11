using System;
using UnityEngine;

[Serializable]
public class RainManager : MonoBehaviour
{
	public float minYPosition;

	public int numberOfParticles;

	public float areaSize;

	public float areaHeight;

	public float fallingSpeed;

	public float particleSize;

	public float flakeRandom;

	public Mesh[] preGennedMeshes;

	private int preGennedIndex;

	public bool generateNewAssetsOnStart;

	public RainManager()
	{
		numberOfParticles = 400;
		areaSize = 40f;
		areaHeight = 15f;
		fallingSpeed = 23f;
		particleSize = 0.2f;
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
		Vector3 right = Camera.main.transform.right;
		Vector3 up = Vector3.up;
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
			vector.y = areaHeight * UnityEngine.Random.value;
			vector.z = areaSize * (UnityEngine.Random.value - 0.5f);
			float value = UnityEngine.Random.value;
			float num4 = particleSize * 0.215f;
			float num5 = particleSize + value * flakeRandom;
			array[num2 + 0] = vector - right * num4 - up * num5;
			array[num2 + 1] = vector + right * num4 - up * num5;
			array[num2 + 2] = vector + right * num4 + up * num5;
			array[num2 + 3] = vector - right * num4 + up * num5;
			array4[num2 + 0] = -Camera.main.transform.forward;
			array4[num2 + 1] = -Camera.main.transform.forward;
			array4[num2 + 2] = -Camera.main.transform.forward;
			array4[num2 + 3] = -Camera.main.transform.forward;
			array2[num2 + 0] = new Vector2(0f, 0f);
			array2[num2 + 1] = new Vector2(1f, 0f);
			array2[num2 + 2] = new Vector2(1f, 1f);
			array2[num2 + 3] = new Vector2(0f, 1f);
			array3[num2 + 0] = new Vector2((float)UnityEngine.Random.Range(-2, 2) * 4f, (float)UnityEngine.Random.Range(-1, 1) * 1f);
			array3[num2 + 1] = new Vector2(array3[num2 + 0].x, array3[num2 + 0].y);
			array3[num2 + 2] = new Vector2(array3[num2 + 0].x, array3[num2 + 0].y);
			array3[num2 + 3] = new Vector2(array3[num2 + 0].x, array3[num2 + 0].y);
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
