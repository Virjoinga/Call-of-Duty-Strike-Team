using System.Collections.Generic;
using UnityEngine;

public class LevelLoaderList : MonoBehaviour
{
	public GameObject PrefabButton;

	public UIScrollList ScrollList;

	private string mLevelToLoad;

	private UIButton mLevelSelectButton;

	private void Start()
	{
		if (ScrollList == null)
		{
			ScrollList = base.gameObject.GetComponent<UIScrollList>();
		}
		GameObject gameObject = Object.Instantiate(PrefabButton) as GameObject;
		UIButton component = gameObject.GetComponent<UIButton>();
		component.Text = "Clear Cache";
		component.SetColor(Color.blue);
		component.AddValueChangedDelegate(ClearBundleCacheDelegate);
		ScrollList.AddItem(gameObject);
		GameObject gameObject2 = Object.Instantiate(PrefabButton) as GameObject;
		mLevelSelectButton = gameObject2.GetComponent<UIButton>();
		mLevelSelectButton.Text = "PLAY!";
		mLevelSelectButton.SetColor(Color.red);
		mLevelSelectButton.AddValueChangedDelegate(LoadLevelSelect);
		ScrollList.AddItem(gameObject2);
		List<string> currentlyLoadedScenes = BundleManager.Instance.GetCurrentlyLoadedScenes();
		foreach (string item in currentlyLoadedScenes)
		{
			NewSceneStartedDownload(base.gameObject, new BundleManager.NewSceneAddedEventArgs(item));
			NewSceneAdded(base.gameObject, new BundleManager.NewSceneAddedEventArgs(item));
		}
		BundleManager.Instance.OnNewSceneAdded += NewSceneAdded;
		BundleManager.Instance.OnNewSceneStartDownload += NewSceneStartedDownload;
	}

	private void ClearBundleCacheDelegate(IUIObject obj)
	{
		BundleManager.Instance.ClearCache();
		BundleManager.Instance.DeleteAllBundles();
		Object.Destroy(BundleManager.Instance.gameObject);
		Application.LoadLevel("dlc_loader");
	}

	private void NewSceneStartedDownload(object obj, BundleManager.NewSceneAddedEventArgs args)
	{
		if (!args.SceneName.StartsWith("hud"))
		{
			GameObject gameObject = Object.Instantiate(PrefabButton) as GameObject;
			UIButton component = gameObject.GetComponent<UIButton>();
			component.Text = args.SceneName;
			component.SetColor(Color.red);
			ScrollList.AddItem(gameObject);
		}
	}

	private void NewSceneAdded(object obj, BundleManager.NewSceneAddedEventArgs args)
	{
		for (int i = 0; i < ScrollList.Count; i++)
		{
			IUIListObject item = ScrollList.GetItem(i);
			GameObject gameObject = item.gameObject;
			UIButton component = gameObject.GetComponent<UIButton>();
			if (component.Text == args.SceneName)
			{
				component.SetColor(Color.green);
				component.AddValueChangedDelegate(LoadLevelButtonSelectedDelegate);
			}
		}
	}

	private void LoadLevelButtonSelectedDelegate(IUIObject obj)
	{
		UIButton uIButton = obj as UIButton;
		if ((bool)uIButton)
		{
			mLevelToLoad = uIButton.Text;
			obj.gameObject.PunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.2f, 0f, "LoadLevel", base.gameObject);
		}
	}

	private void LoadLevelSelect(IUIObject obj)
	{
		if (BundleManager.Instance.IsReady())
		{
			UIButton uIButton = obj as UIButton;
			if ((bool)uIButton)
			{
				mLevelToLoad = "MainMenu";
				obj.gameObject.PunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.2f, 0f, "LoadLevel", base.gameObject);
			}
		}
	}

	private void LoadLevel()
	{
		SceneLoader.AsyncLoadSceneWithLoadingScreen(mLevelToLoad);
	}

	private void Update()
	{
		if (BundleManager.Instance.IsReady())
		{
			mLevelSelectButton.SetColor(Color.magenta);
			mLevelSelectButton.gameObject.ColorTo(Color.green, 0.3f, 0f, LoopType.pingPong);
			mLevelSelectButton.gameObject.ScaleTo(new Vector3(1.1f, 1.1f, 1.1f), 0.3f, 0f, EaseType.easeInOutSine, LoopType.pingPong);
			if (Application.CanStreamedLevelBeLoaded(15))
			{
				LoadLevelSelect(mLevelSelectButton);
			}
		}
	}
}
