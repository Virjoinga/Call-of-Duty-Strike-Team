using UnityEngine;

public class LaserScanLineEffect : MonoBehaviour
{
	private enum AnimateState
	{
		AnimateOnLeft = 0,
		AnimateOnRight = 1,
		AnimateOff = 2
	}

	private const int WIDTH = 256;

	private const int NUM_TRIS = 5;

	private Mesh mMesh;

	private MeshRenderer mRend;

	private Material mMat;

	private MeshFilter mMeshFilter;

	public float mAnimScale = 0.5f;

	public float mAnimMaxAngle = 15f;

	private float mScroll;

	private float mTime;

	private bool mAllowMeshRender = true;

	private bool mAllowInFirstPerson;

	public Texture2D mDistMap;

	private float mAngle;

	private float mAngleInc = 10f;

	private float mCurrentAngle;

	private bool mAnimate;

	private AnimateState mAnimateState = AnimateState.AnimateOff;

	private Vector3 mOriginalForward;

	private Vector3 mOriginalRight;

	public void Awake()
	{
	}

	public void Setup(float x, float z, Vector3 Offset, bool animate)
	{
		base.gameObject.transform.localPosition += Offset;
		mAngle = Vector3.Angle(new Vector3(x, 0f, z), Vector3.forward);
		if (animate)
		{
			mAnimate = true;
			x /= 3f;
			mAnimateState = AnimateState.AnimateOnRight;
		}
		x = Mathf.Abs(x);
		z = Mathf.Abs(z);
		CreateFan(x, z, 5);
		CreateDistMap(z);
		mCurrentAngle = 0f;
		mOriginalForward = base.gameObject.transform.forward;
		mOriginalRight = base.gameObject.transform.right;
	}

	private void CreateFan(float x, float z, int numTris)
	{
		float num = 0.005f;
		float num2 = num * 2f / (float)numTris;
		float num3 = 0f - num;
		Vector3 vector = new Vector3(0f - x, 0f, z);
		Vector3 to = new Vector3(x, 0f, z);
		vector.Normalize();
		to.Normalize();
		float f = Vector3.Angle(vector, Vector3.forward);
		f = Mathf.Abs(f);
		f /= 360f;
		float num4 = f / (float)numTris * 2f;
		float num5 = 0f - f;
		mMesh = Triangles.GetMesh(2 * numTris, 0, 2, 2);
		Vector3[] array = new Vector3[6 * numTris];
		Vector2[] array2 = new Vector2[6 * numTris];
		Color[] array3 = new Color[6 * numTris];
		Vector3 vector2 = vector * z;
		int num6 = 0;
		for (int i = 0; i < numTris; i++)
		{
			float t = (float)(i + 1) / (float)numTris;
			Vector3 vector3 = Vector3.Slerp(vector, to, t);
			vector3 *= z;
			array[num6] = new Vector3(num3, 0f, 0f);
			array[num6 + 1] = new Vector3(vector2.x, 0f, vector2.z);
			array[num6 + 2] = new Vector3(vector3.x, 0f, vector3.z);
			array2[num6] = new Vector2(num3 / x * f, 0f);
			array2[num6 + 1] = new Vector2(num5, 1f);
			array2[num6 + 2] = new Vector2(num5 + num4, 1f);
			array3[num6] = new Color(1f, 0f, 0f, 0.5f);
			array3[num6 + 1] = new Color(1f, 0f, 0f, 0f);
			array3[num6 + 2] = new Color(1f, 0f, 0f, 0f);
			array[num6 + 3] = new Vector3(num3, 0f, 0f);
			array[num6 + 4] = new Vector3(vector3.x, 0f, vector3.z);
			array[num6 + 5] = new Vector3(num3 + num2, 0f, 0f);
			array2[num6 + 3] = new Vector2(num3 / x * f, 0f);
			array2[num6 + 4] = new Vector2(num5 + num4, 1f);
			array2[num6 + 5] = new Vector2((num3 + num2) / x * f, 0f);
			array3[num6 + 3] = new Color(1f, 0f, 0f, 0.5f);
			array3[num6 + 4] = new Color(1f, 0f, 0f, 0f);
			array3[num6 + 5] = new Color(1f, 0f, 0f, 0.5f);
			vector2 = vector3;
			num5 += num4;
			num3 += num2;
			num6 += 6;
		}
		mMesh.vertices = array;
		mMesh.uv = array2;
		mMesh.colors = array3;
		mMesh.Optimize();
		mMeshFilter = base.gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		mMeshFilter.mesh = mMesh;
		mRend = base.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		mRend.material = new Material(EffectsController.Instance.Effects.LaserScanLineShader);
		mRend.material.mainTexture = EffectsController.Instance.Effects.LaserScanLineTexture;
		mRend.material.SetFloat("_UVInc", 0f);
		mRend.material.SetTexture("_DistanceMap", mDistMap);
		mRend.material.SetFloat("_ScanScale", num / x);
		mMat = mRend.material;
		mMeshFilter.mesh.RecalculateBounds();
	}

	private void CreateDistMap(float z)
	{
		mDistMap = new Texture2D(256, 1);
		Transform transform = base.gameObject.transform;
		float angle = 1.40625f;
		GameObject gameObject = null;
		RaycastHit hitInfo;
		if (Physics.Linecast(transform.position, transform.position + transform.forward * z, out hitInfo, 1 << LayerMask.NameToLayer("Default")) && hitInfo.collider.gameObject.name == "Shield")
		{
			gameObject = hitInfo.collider.gameObject;
			gameObject.SetActive(false);
		}
		for (int i = 0; i < 256; i++)
		{
			Vector3 end = transform.position + transform.forward * z;
			if (Physics.Linecast(transform.position, end, out hitInfo, 1 << LayerMask.NameToLayer("Default")))
			{
				mDistMap.SetPixel(i, 0, new Color(1f, 0f, 0f, hitInfo.distance / z));
			}
			else
			{
				mDistMap.SetPixel(i, 0, new Color(0f, 1f, 0f, 1f));
			}
			transform.Rotate(Vector3.up, angle);
		}
		if (gameObject != null)
		{
			gameObject.SetActive(true);
		}
		mDistMap.Apply();
		mMat.SetTexture("_DistanceMap", mDistMap);
	}

	private void Update()
	{
		if (GameController.Instance.IsFirstPerson)
		{
			if (mAllowInFirstPerson)
			{
				mRend.enabled = true;
			}
			else
			{
				mRend.enabled = false;
			}
		}
		else if (mAllowMeshRender)
		{
			mRend.enabled = true;
		}
		else
		{
			mRend.enabled = false;
		}
		mTime += mAnimScale;
		if (mTime >= 1f || mTime < 0f)
		{
			mAnimScale *= -1f;
		}
		mScroll += Time.deltaTime * 0.1f;
		if (mScroll > 1f)
		{
			mScroll -= 1f;
		}
		if (mMat != null)
		{
			mMat.SetFloat("_Scroll", mScroll);
		}
		if (mAnimate)
		{
			if (mAnimateState == AnimateState.AnimateOnLeft || mAnimateState == AnimateState.AnimateOnRight)
			{
				if (mOriginalForward != base.gameObject.transform.parent.forward)
				{
					mAnimateState = AnimateState.AnimateOff;
					base.gameObject.transform.forward = mOriginalForward;
					mCurrentAngle = 0f;
				}
				else
				{
					base.gameObject.transform.Rotate(Vector3.up, mAngleInc * Time.deltaTime);
					mCurrentAngle += mAngleInc * Time.deltaTime;
					Mathf.Clamp(mCurrentAngle, 0f - mAngle, mAngle);
					if (mCurrentAngle <= 0f - mAngle && mAnimateState == AnimateState.AnimateOnLeft)
					{
						mAnimateState = AnimateState.AnimateOnRight;
						mAngleInc *= -1f;
					}
					else if (mCurrentAngle >= mAngle && mAnimateState == AnimateState.AnimateOnRight)
					{
						mAnimateState = AnimateState.AnimateOnLeft;
						mAngleInc *= -1f;
					}
				}
			}
			else if (mOriginalForward == base.gameObject.transform.parent.forward)
			{
				mAnimateState = AnimateState.AnimateOnLeft;
			}
		}
		float num = Vector3.Angle(mOriginalForward, base.gameObject.transform.parent.forward);
		float num2 = Vector3.Dot(mOriginalRight, base.gameObject.transform.forward);
		if ((double)num2 < 0.0)
		{
			num *= -1f;
		}
		mRend.material.SetFloat("_UVInc", (mCurrentAngle + num) / 360f);
	}

	public void SetLaserColour(Color col)
	{
		Color[] array = new Color[30];
		for (int i = 0; i < mMesh.colors.Length; i++)
		{
			array[i] = new Color(col.r, col.g, col.b, mMeshFilter.mesh.colors[i].a);
		}
		mMeshFilter.mesh.colors = array;
	}

	public void Enable(bool yesNo)
	{
		mAllowMeshRender = yesNo;
	}

	public void AllowInFirstPerson(bool yesNo)
	{
		mAllowInFirstPerson = yesNo;
	}
}
