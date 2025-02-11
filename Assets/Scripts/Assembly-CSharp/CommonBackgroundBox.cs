using UnityEngine;

public class CommonBackgroundBox : MonoBehaviour
{
	public const int UNITSIZE = 32;

	private const int BGBORDER = 2;

	private const int FGBORDER = 5;

	private const int FGOFFSET = 10;

	private const int FOOTERHEIGHT = 16;

	private const int FOOTERGAP = 2;

	public PackedSprite HeaderDetailPrefab;

	public float ForegroundHeightInUnits = 3f;

	public float UnitsWide = 4f;

	public float SmallScreenForegroundHeightInUnits = 3f;

	public float SmallScreenUnitsWide = 4f;

	public bool UseSmallScreenSizes;

	public Material BackgroundMaterial;

	public Material ForegroundMaterial;

	public Material FooterMaterial;

	public Material TitleMaterial;

	public Material FooterDetailMaterial;

	public Scale9Grid Background;

	public Scale9Grid Foreground;

	public BackgroundBoxFooter Footer;

	public BackgroundBoxHeader Title;

	public BackgroundBoxFooterDetail FooterDetail;

	private CommonBackgroundBoxPlacement[] mObjectsToPlace;

	private static string mBackgroundName = "Background";

	private static string mForegroundName = "Foreground";

	private static string mTitleName = "Title";

	private static string mFooterName = "Footer";

	private static string mFooterDetailName = "FooterDetail";

	private Vector3 mForegroundCentre;

	private Vector2 mForegroundSize;

	private float mPixelSize;

	private int mUnitSize;

	private int mBgBorder;

	private int mFgBorder;

	private int mFgOffset;

	private int mFooterHeight;

	private int mFooterGap;

	private bool mRefreshPlacements;

	private bool mRefreshSizes = true;

	private bool HasTitle;

	private bool HasFooter;

	private bool mAdjustForRetina = true;

	public Rect BoundingRect;

	public Vector3 ForegroundCentre
	{
		get
		{
			return mForegroundCentre;
		}
	}

	public Vector2 ForegroundSize
	{
		get
		{
			return mForegroundSize;
		}
	}

	public bool RefreshPlacements
	{
		set
		{
			mRefreshPlacements = value;
		}
	}

	public bool AdjustForRetina
	{
		get
		{
			return mAdjustForRetina;
		}
		set
		{
			mAdjustForRetina = value;
		}
	}

	private void AdjustSizes()
	{
		if (mRefreshSizes)
		{
			mPixelSize = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
			float num = 1f;
			bool flag = TBFUtils.IsRetinaHdDevice();
			if (TBFUtils.UseAlternativeLayout() && UseSmallScreenSizes)
			{
				ForegroundHeightInUnits = SmallScreenForegroundHeightInUnits;
				UnitsWide = SmallScreenUnitsWide;
			}
			if (flag && mAdjustForRetina)
			{
				num = 2f;
			}
			mUnitSize = (int)(32f * num);
			mBgBorder = (int)(2f * num);
			mFgBorder = (int)(5f * num);
			mFgOffset = (int)(10f * num);
			mFooterHeight = (int)(16f * num);
			mFooterGap = (int)(2f * num);
			CalculateForegroundSize();
			mRefreshSizes = false;
		}
	}

	private void CalculateForegroundSize()
	{
		float num = UnitsWide * (float)mUnitSize + (float)mBgBorder * 2f;
		mForegroundSize.x = num - (float)mFgOffset * 2f;
		mForegroundSize.y = ForegroundHeightInUnits * (float)mUnitSize + (float)mFgBorder * 2f;
	}

	public void PreAnimateBoxSizeChange()
	{
		AdjustSizes();
	}

	public float GetWidth()
	{
		AdjustSizes();
		return UnitsWide * (float)mUnitSize + (float)mBgBorder * 2f;
	}

	public float GetHeight()
	{
		AdjustSizes();
		float num = ForegroundHeightInUnits * (float)mUnitSize + (float)mFgBorder * 2f;
		return num + ((float)mFgOffset * 2f + (float)(mFooterHeight + mFooterGap));
	}

	private void Start()
	{
		Resize();
	}

	public void SetupRect()
	{
		Vector2 vector = Camera.main.WorldToScreenPoint(base.transform.position);
		float width = Background.size.x / mPixelSize;
		float num = Background.size.y / mPixelSize;
		BoundingRect = new Rect(vector.x, vector.y - num, width, num);
	}

	public void Resize()
	{
		mRefreshSizes = true;
		FetchComponents();
		AdjustSizes();
		if (Background != null)
		{
			Background.size.x = UnitsWide * (float)mUnitSize + (float)mBgBorder * 2f;
			float num = ForegroundHeightInUnits * (float)mUnitSize + (float)mFgBorder * 2f;
			Background.size.y = num + (float)mFgOffset * 2f + ((!HasFooter) ? 0f : ((float)(mFooterHeight + mFooterGap)));
			Vector3 vector = Camera.main.WorldToScreenPoint(base.transform.position);
			Vector3 position = vector;
			position.y += ((!HasFooter) ? 0f : ((float)mFooterHeight * 0.5f + (float)mFooterGap * 0.5f));
			Vector3 vector2 = Camera.main.ScreenToWorldPoint(position);
			mForegroundCentre = vector2;
			Background.Resize(mPixelSize);
			SetupRect();
			if (HasFooter)
			{
				if (Footer != null)
				{
					position = vector;
					position.y -= num * 0.5f + (float)mFooterGap * 0.5f;
					vector2 = Camera.main.ScreenToWorldPoint(position);
					vector2.z = Footer.transform.position.z;
					Footer.size.x = mForegroundSize.x;
					Footer.size.y = mFooterHeight;
					Footer.transform.position = vector2;
					if (FooterDetail != null)
					{
						position.x += mForegroundSize.x * 0.48f;
						position.z -= 0.25f;
						vector2 = Camera.main.ScreenToWorldPoint(position);
						FooterDetail.transform.position = vector2;
					}
					Footer.Resize(mPixelSize);
				}
				else
				{
					Debug.LogWarning("Might need to build again due to missing footer");
				}
			}
			if (Title != null)
			{
				Title.Hide();
			}
			if (mObjectsToPlace != null && !mRefreshPlacements)
			{
				return;
			}
			mObjectsToPlace = base.transform.parent.GetComponentsInChildren<CommonBackgroundBoxPlacement>();
			mRefreshPlacements = false;
			CommonBackgroundBoxPlacement[] array = mObjectsToPlace;
			foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
			{
				if (commonBackgroundBoxPlacement != null && commonBackgroundBoxPlacement.enabled)
				{
					commonBackgroundBoxPlacement.Position(mForegroundCentre, mForegroundSize);
				}
			}
		}
		else
		{
			Debug.LogWarning("Build before repositioning");
		}
	}

	public void BuildGrid()
	{
		BuildGrid(false);
	}

	private void FetchComponents()
	{
		if (Background == null)
		{
			Transform transform = base.transform.FindChild(mBackgroundName);
			Background = ((!(transform != null)) ? null : transform.GetComponent<Scale9Grid>());
		}
		if (Foreground == null)
		{
			Transform transform2 = base.transform.FindChild(mForegroundName);
			Foreground = ((!(transform2 != null)) ? null : transform2.GetComponent<Scale9Grid>());
			if (Foreground != null)
			{
				Object.DestroyImmediate(Foreground.gameObject);
			}
		}
		if (Title == null)
		{
			Transform transform3 = base.transform.FindChild(mTitleName);
			Title = ((!(transform3 != null)) ? null : transform3.GetComponent<BackgroundBoxHeader>());
		}
		if (Footer == null)
		{
			Transform transform4 = base.transform.FindChild(mFooterName);
			Footer = ((!(transform4 != null)) ? null : transform4.GetComponent<BackgroundBoxFooter>());
		}
		if (FooterDetail == null)
		{
			Transform transform5 = base.transform.FindChild(mFooterDetailName);
			FooterDetail = ((!(transform5 != null)) ? null : transform5.GetComponent<BackgroundBoxFooterDetail>());
		}
	}

	public void BuildGrid(bool force)
	{
		FetchComponents();
		if (force)
		{
			if (Background != null)
			{
				Object.DestroyImmediate(Background.gameObject);
			}
			if (Foreground != null)
			{
				Object.DestroyImmediate(Foreground.gameObject);
			}
			if (Footer != null)
			{
				Object.DestroyImmediate(Footer.gameObject);
			}
			if (FooterDetail != null)
			{
				Object.DestroyImmediate(FooterDetail.gameObject);
			}
			if (Title != null)
			{
				Object.DestroyImmediate(Title.gameObject);
			}
		}
		if (BackgroundMaterial != null && ForegroundMaterial != null)
		{
			if (Background == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = mBackgroundName;
				Background = gameObject.AddComponent<Scale9Grid>();
				gameObject.transform.parent = base.transform;
				Background.mat = BackgroundMaterial;
			}
			Background.BuildGrid(force);
			if (HasFooter && FooterMaterial != null && FooterDetailMaterial != null)
			{
				if (Footer == null)
				{
					GameObject gameObject2 = new GameObject();
					gameObject2.name = mFooterName;
					Footer = gameObject2.AddComponent<BackgroundBoxFooter>();
					gameObject2.transform.parent = base.transform;
					Footer.Mat = FooterMaterial;
				}
				if (FooterDetail == null)
				{
					GameObject gameObject3 = new GameObject();
					gameObject3.name = mFooterDetailName;
					FooterDetail = gameObject3.AddComponent<BackgroundBoxFooterDetail>();
					gameObject3.transform.parent = base.transform;
					FooterDetail.Mat = FooterDetailMaterial;
				}
				Footer.BuildGrid(force);
				FooterDetail.Build(force);
			}
			Resize();
		}
		else
		{
			Debug.LogWarning("Assign materials before building");
		}
		if (HasTitle)
		{
			if (TitleMaterial != null)
			{
				GameObject gameObject4 = new GameObject();
				gameObject4.name = mTitleName;
				Title = gameObject4.AddComponent<BackgroundBoxHeader>();
				gameObject4.transform.parent = base.transform;
				Title.Mat = TitleMaterial;
				Title.DetailPrefab = HeaderDetailPrefab;
				Title.BuildGrid(force);
			}
			else
			{
				Debug.LogWarning("Assign material to title before building");
			}
		}
	}
}
