using UnityEngine;

public class FrontEndScreen : MonoBehaviour
{
	private const float BladesAccumulativeDelay = 0.1f;

	public ScreenID ID;

	private MenuScreenBlade[] mBlades;

	private bool mDisableAfterPlacement;

	public bool IsActive { get; private set; }

	public bool IsTransitioning { get; private set; }

	public bool IsAnyBladeTransitioningOff
	{
		get
		{
			bool result = false;
			for (int i = 0; i < mBlades.Length; i++)
			{
				if (mBlades[i].IsTransitioningOff)
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}

	protected MenuScreenBlade[] Blades
	{
		get
		{
			return mBlades;
		}
		set
		{
			mBlades = value;
		}
	}

	protected virtual void Awake()
	{
		FrontEndController.Instance.RegisterScreen(this);
		MenuScreenBlade component = GetComponent<MenuScreenBlade>();
		MenuScreenBlade[] componentsInChildren = GetComponentsInChildren<MenuScreenBlade>();
		int num = ((component != null) ? 1 : 0);
		mBlades = new MenuScreenBlade[componentsInChildren.Length + num];
		if (component != null)
		{
			mBlades[0] = component;
		}
		componentsInChildren.CopyTo(mBlades, num);
		mDisableAfterPlacement = false;
	}

	protected virtual void Start()
	{
		mDisableAfterPlacement = true;
	}

	protected virtual void Update()
	{
		if (mDisableAfterPlacement)
		{
			bool flag = true;
			if (mBlades != null)
			{
				MenuScreenBlade[] array = mBlades;
				foreach (MenuScreenBlade menuScreenBlade in array)
				{
					if (!menuScreenBlade.IsPlaced)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
		}
		else
		{
			if (!IsTransitioning)
			{
				return;
			}
			bool flag2 = true;
			bool flag3 = false;
			MenuScreenBlade[] array2 = mBlades;
			foreach (MenuScreenBlade menuScreenBlade2 in array2)
			{
				if (menuScreenBlade2.IsActive)
				{
					flag3 = true;
				}
				if (menuScreenBlade2.IsTransitioning)
				{
					flag2 = false;
					break;
				}
			}
			IsTransitioning = !flag2;
			IsActive = (flag3 && flag2) || mBlades.Length == 0;
		}
	}

	protected void ClearScreen(bool active)
	{
		IsTransitioning = false;
		IsActive = active;
	}

	public virtual void EnterScreen()
	{
		if (IsActive || IsTransitioning)
		{
			return;
		}
		mDisableAfterPlacement = false;
		IsTransitioning = true;
		float num = 0.1f;
		MenuScreenBlade[] array = mBlades;
		foreach (MenuScreenBlade menuScreenBlade in array)
		{
			if (menuScreenBlade != null)
			{
				menuScreenBlade.DelayedActivate(num);
				num += 0.1f;
			}
		}
	}

	public virtual void ExitScreen()
	{
		if (!IsActive || IsTransitioning)
		{
			return;
		}
		IsActive = false;
		IsTransitioning = true;
		float num = 0.1f;
		MenuScreenBlade[] array = mBlades;
		foreach (MenuScreenBlade menuScreenBlade in array)
		{
			if (menuScreenBlade != null)
			{
				menuScreenBlade.DelayedDeactivate(num);
				num += 0.1f;
			}
		}
	}

	public virtual void OnScreen()
	{
	}

	public virtual void OffScreen()
	{
	}
}
