using UnityEngine;

public class ViewConeEffect : MonoBehaviour
{
	private Mesh mMesh;

	private MeshRenderer mRend;

	private Material mMat;

	private float mScroll;

	private Color mCurrentColour = Color.white;

	public Color CurrentColour
	{
		set
		{
			if (mCurrentColour != value)
			{
				mCurrentColour = value;
				Color[] colors = new Color[3]
				{
					mCurrentColour.Alpha(0.8f),
					mCurrentColour.Alpha(0f),
					mCurrentColour.Alpha(0f)
				};
				MeshFilter meshFilter = base.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
				meshFilter.mesh.colors = colors;
			}
		}
	}

	public void Awake()
	{
	}

	public void Setup(float x, float y)
	{
		mMesh = Triangles.GetMesh(1, 0, 2, 2);
		Vector3[] vertices = new Vector3[3]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(x, y, 0f),
			new Vector3(0f - x, y, 0f)
		};
		mMesh.vertices = vertices;
		mMesh.Optimize();
		MeshFilter meshFilter = base.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		meshFilter.mesh = mMesh;
		mRend = base.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		mRend.material = new Material(EffectsController.Instance.Effects.LaserScanLineShader);
		mRend.material.mainTexture = EffectsController.Instance.Effects.LaserScanLineTexture;
		mMat = mRend.material;
		meshFilter.mesh.RecalculateBounds();
		CurrentColour = ColourChart.ViewConeEnemy;
	}

	private void Update()
	{
		mScroll += Time.deltaTime * 0.1f;
		if (mScroll > 1f)
		{
			mScroll -= 1f;
		}
		if (mMat != null)
		{
			mMat.SetFloat("_Scroll", mScroll);
		}
	}
}
