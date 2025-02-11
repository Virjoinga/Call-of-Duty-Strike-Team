public class CMSecurityCamera : InterfaceableObject
{
	private Actor target;

	protected override void Start()
	{
		base.Start();
		target = base.gameObject.GetComponent<Actor>();
		quickType = QuickType.EnemySoldier;
	}

	protected override void PopulateMenuItems()
	{
		GameplayController gameplayController = GameplayController.Instance();
		if (gameplayController.AnySelectedAllowedCMOption(ContextMenuOptionType.Shoot) && target.awareness.ChDefCharacterType != CharacterType.SentryGun)
		{
			AddCallableMethod("S_CMShoot", ContextMenuIcons.Shoot);
		}
		if (ContextMenuVisualiser.ContextMenuDebugOptions)
		{
			AddCallableMethod("S_CMKill", ContextMenuIcons.Debug);
		}
	}

	public void S_CMShoot()
	{
		Actor component = base.gameObject.GetComponent<Actor>();
		if (component != null)
		{
			GameplayController gameplayController = GameplayController.Instance();
			OrdersHelper.OrderShootAtTarget(gameplayController, component, false);
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMThrowGrenade()
	{
		Actor component = base.gameObject.GetComponent<Actor>();
		if (component != null)
		{
			GameplayController gameplayController = GameplayController.Instance();
			OrdersHelper.OrderPrimeGrenade(gameplayController, component.GetPosition());
		}
		CommonHudController.Instance.ClearContextMenu();
	}

	public void S_CMKill()
	{
		Actor component = base.gameObject.GetComponent<Actor>();
		if (component != null)
		{
			component.realCharacter.Kill("ActOfGod");
		}
		CommonHudController.Instance.ClearContextMenu();
	}
}
