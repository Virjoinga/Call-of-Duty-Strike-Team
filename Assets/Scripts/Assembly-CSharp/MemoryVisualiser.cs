using System.Collections.Generic;
using UnityEngine;

public class MemoryVisualiser : MonoBehaviour
{
	private struct MemoryReportEntry
	{
		public string name;

		public string type;

		public int size;

		public override string ToString()
		{
			return string.Format("{0} [{1}]:\t{2}", name, type, size);
		}
	}

	public static MemoryVisualiser instance;

	private bool mShowMesh;

	private bool mShowTexture;

	private bool mShowAnimation;

	private bool mShowAudio;

	private bool mShowMaterial;

	private bool mShowRenderer;

	private int mMeshTotal;

	private int mTextureTotal;

	private int mAnimationTotal;

	private int mAudioTotal;

	private int mMaterialTotal;

	private int mRendererTotal;

	private List<MemoryReportEntry> report = new List<MemoryReportEntry>();

	private bool mHidden;

	private Vector2 scrollPosition;

	private string savePath;

	public static MemoryVisualiser Instance()
	{
		return instance;
	}

	private void Awake()
	{
		Object.Destroy(this);
	}

	private void Start()
	{
		mHidden = true;
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (mHidden)
		{
			return;
		}
		string text = "Assets Runtime Memory Visualiser";
		int num = 600;
		int num2 = 800;
		Rect screenRect = new Rect(Screen.width / 2 - num2 / 2, Screen.height / 2 - num / 2, num2, num);
		bool flag = false;
		GUILayout.BeginArea(screenRect);
		GUILayout.BeginVertical(text, GUI.skin.window);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Options");
		flag = GUILayout.Toggle(mShowMesh, "Mesh");
		if (flag != mShowMesh)
		{
			mShowMesh = flag;
			CoronaAssetMemoryReport();
		}
		flag = GUILayout.Toggle(mShowTexture, "Texture");
		if (flag != mShowTexture)
		{
			mShowTexture = flag;
			CoronaAssetMemoryReport();
		}
		flag = GUILayout.Toggle(mShowMaterial, "Material");
		if (flag != mShowMaterial)
		{
			mShowMaterial = flag;
			CoronaAssetMemoryReport();
		}
		flag = GUILayout.Toggle(mShowAnimation, "Animation");
		if (flag != mShowAnimation)
		{
			mShowAnimation = flag;
			CoronaAssetMemoryReport();
		}
		flag = GUILayout.Toggle(mShowAudio, "Audio");
		if (flag != mShowAudio)
		{
			mShowAudio = flag;
			CoronaAssetMemoryReport();
		}
		flag = GUILayout.Toggle(mShowRenderer, "Renderer");
		if (flag != mShowRenderer)
		{
			mShowRenderer = flag;
			CoronaAssetMemoryReport();
		}
		GUILayout.EndHorizontal();
		if (savePath != null)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Save Path = " + savePath);
			GUILayout.EndHorizontal();
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("Mesh = " + mMeshTotal / 1024 + "kb");
		GUILayout.Label("Texture = " + mTextureTotal / 1024 + "kb");
		GUILayout.Label("Material = " + mMaterialTotal / 1024 + "kb");
		GUILayout.Label("Animation = " + mAnimationTotal / 1024 + "kb");
		GUILayout.Label("Audio = " + mAudioTotal / 1024 + "kb");
		GUILayout.Label("Renderers = " + mRendererTotal);
		GUILayout.Label("Asset Total = " + (mMeshTotal + mTextureTotal + mMaterialTotal + mAnimationTotal + mAudioTotal) / 1048576 + "mb");
		GUILayout.Label("Heap Size = " + UnityEngine.Profiling.Profiler.usedHeapSize / 1048576 + "mb");
		GUILayout.EndHorizontal();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		int num3 = 0;
		foreach (MemoryReportEntry item in report)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(item.ToString());
			GUILayout.EndHorizontal();
			num3++;
			if (num3 == 200)
			{
				break;
			}
		}
		GUILayout.EndScrollView();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	public void Toggle()
	{
		mHidden = !mHidden;
		if (!mHidden)
		{
			CoronaAssetMemoryReport();
		}
	}

	public void Hide()
	{
		mHidden = true;
	}

	public void Show()
	{
		mHidden = false;
		CoronaAssetMemoryReport();
	}

	private void CoronaAssetMemoryReport()
	{
		report.Clear();
		if (mShowMesh)
		{
			mMeshTotal = 0;
			Object[] array = Resources.FindObjectsOfTypeAll(typeof(Mesh));
			foreach (Object @object in array)
			{
				MemoryReportEntry item = default(MemoryReportEntry);
				item.name = @object.name;
				item.type = @object.GetType().ToString();
				item.size = UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(@object);
				mMeshTotal += item.size;
				report.Add(item);
			}
		}
		if (mShowAnimation)
		{
			mAnimationTotal = 0;
			Object[] array2 = Resources.FindObjectsOfTypeAll(typeof(AnimationClip));
			foreach (Object object2 in array2)
			{
				MemoryReportEntry item2 = default(MemoryReportEntry);
				item2.name = object2.name;
				item2.type = object2.GetType().ToString();
				item2.size = UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(object2);
				mAnimationTotal += item2.size;
				report.Add(item2);
			}
		}
		if (mShowTexture)
		{
			mTextureTotal = 0;
			Object[] array3 = Resources.FindObjectsOfTypeAll(typeof(Texture));
			foreach (Object object3 in array3)
			{
				MemoryReportEntry item3 = default(MemoryReportEntry);
				item3.name = object3.name;
				item3.type = object3.GetType().ToString();
				item3.size = UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(object3);
				mTextureTotal += item3.size;
				report.Add(item3);
			}
		}
		if (mShowMaterial)
		{
			mMaterialTotal = 0;
			Object[] array4 = Resources.FindObjectsOfTypeAll(typeof(Material));
			foreach (Object object4 in array4)
			{
				MemoryReportEntry item4 = default(MemoryReportEntry);
				item4.name = object4.name;
				item4.type = object4.GetType().ToString();
				item4.size = UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(object4);
				mMaterialTotal += item4.size;
				report.Add(item4);
			}
		}
		if (mShowAudio)
		{
			mAudioTotal = 0;
			Object[] array5 = Resources.FindObjectsOfTypeAll(typeof(AudioClip));
			foreach (Object object5 in array5)
			{
				MemoryReportEntry item5 = default(MemoryReportEntry);
				item5.name = object5.name;
				item5.type = object5.GetType().ToString();
				item5.size = UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(object5);
				mAudioTotal += item5.size;
				report.Add(item5);
			}
		}
		if (mShowRenderer)
		{
			mRendererTotal = 0;
			Object[] array6 = Resources.FindObjectsOfTypeAll(typeof(Renderer));
			foreach (Object object6 in array6)
			{
				MemoryReportEntry item6 = default(MemoryReportEntry);
				item6.name = object6.name;
				item6.type = object6.GetType().ToString();
				item6.size = 0;
				mRendererTotal++;
				report.Add(item6);
			}
		}
		report.Sort((MemoryReportEntry a, MemoryReportEntry b) => Comparer<int>.Default.Compare(b.size, a.size));
	}
}
