using System.Collections;
using UnityEngine;

public class ImageListMessageBox : MessageBox
{
	public RankIconController RankIcon;

	public PerkIconController PerkIcon;

	public WeaponIconController WeaponIcon;

	public EquipmentIconController EquipmentIcon;

	private AnimatedIconListHelper mUnlockAnim;

	public void SetupWithBundles(BundleDescriptor[] bundles)
	{
		mUnlockAnim = new AnimatedIconListHelper();
		mUnlockAnim.Setup(PerkIcon, WeaponIcon, EquipmentIcon, null, null, m_bodyText);
		int num = 4;
		EquipmentDescriptor[] array = new EquipmentDescriptor[num];
		int[] array2 = new int[num];
		foreach (BundleDescriptor bundleDescriptor in bundles)
		{
			if (!(bundleDescriptor != null))
			{
				continue;
			}
			mUnlockAnim.AddWeaponIcon(bundleDescriptor.Weapon);
			mUnlockAnim.AddPerkIcon(bundleDescriptor.Perk);
			for (int j = 0; j < bundleDescriptor.Equipment.Length; j++)
			{
				if (bundleDescriptor.Equipment[j] != null && (int)bundleDescriptor.Equipment[j].Type < num)
				{
					array[(int)bundleDescriptor.Equipment[j].Type] = bundleDescriptor.Equipment[j];
					array2[(int)bundleDescriptor.Equipment[j].Type] += bundleDescriptor.NumItemsPerEquipment;
				}
			}
		}
		for (int k = 0; k < num; k++)
		{
			if (array[k] != null && array2[k] > 0)
			{
				mUnlockAnim.AddEquipmentIcon(array[k], array2[k]);
			}
		}
	}

	public void SetupWithProPerk(Perk[] perks)
	{
		mUnlockAnim = new AnimatedIconListHelper();
		mUnlockAnim.Setup(PerkIcon, WeaponIcon, EquipmentIcon, null, null, m_bodyText);
		for (int i = 0; i < perks.Length; i++)
		{
			mUnlockAnim.AddProPerkIcon(perks[i].Identifier);
		}
	}

	public override IEnumerator Display(string Title, string Message, bool messageIsTranslated)
	{
		SetText(Title, string.Empty, true);
		InterfaceSFX.Instance.MessageBoxOn.Play2D();
		mAnimator.AnimateOpen();
		while (mAnimator.IsOpening)
		{
			yield return new WaitForEndOfFrame();
		}
		CreateAndPositionButtons();
		RepositionButtons();
		mUnlockAnim.Begin();
		while (mInternalResult == MessageBoxResults.Result.Unknown)
		{
			mUnlockAnim.Update();
			yield return new WaitForEndOfFrame();
		}
		mUnlockAnim.Finish();
		mAnimator.AnimateClosed();
		MenuSFX.Instance.MenuBoxClose.Play2D();
		while (mAnimator.IsClosing)
		{
			yield return new WaitForEndOfFrame();
		}
		if (Results != null)
		{
			Results.InvokeMethodForResult(mInternalResult);
		}
		Object.Destroy(base.gameObject);
	}
}
