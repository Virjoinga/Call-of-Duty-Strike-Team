public class OptimisationInstanceHandle
{
	private bool registered;

	private OptType myOptType;

	private static int[] optInstances = new int[5];

	private int[] PerInstanceOffScreenMasks;

	private int[] PerInstanceOnScreenMasks;

	public static int[] StandardUpdateMasks = new int[42]
	{
		-1, -1, -1, 1431655765, 1431655765, 1431655765, 1431655765, 286331153, 286331153, 286331153,
		286331153, 286331153, 286331153, 269488144, 269488144, 269488144, 269488144, 269488144, 269488144, 269488144,
		269488144, 269488144, 269488144, 269488144, 269488144, 269488144, 269488144, 269488144, 269488144, 269488144,
		269488144, 269488144, 269488144, 269488144, 269488144, 269488144, 269488144, 269488144, 269488144, 269488144,
		269488144, 269488144
	};

	public OptimisationInstanceHandle()
	{
		registered = false;
	}

	~OptimisationInstanceHandle()
	{
		SafeUnregister();
	}

	public void SafeRegister(OptType ot)
	{
		SafeRegister(ot, StandardUpdateMasks, StandardUpdateMasks);
	}

	public void SafeRegister(OptType ot, int[] onScreenMasks, int[] offScreenMasks)
	{
		if (!registered)
		{
			PerInstanceOnScreenMasks = onScreenMasks;
			PerInstanceOffScreenMasks = offScreenMasks;
			myOptType = ot;
			optInstances[(int)ot]++;
			OptimisationManager.SetUpdateMask(myOptType, PerInstanceOnScreenMasks[optInstances[(int)ot]], PerInstanceOffScreenMasks[optInstances[(int)ot]]);
			registered = true;
		}
	}

	public void SafeUnregister()
	{
		if (registered)
		{
			optInstances[(int)myOptType]--;
			OptimisationManager.SetUpdateMask(myOptType, PerInstanceOnScreenMasks[optInstances[(int)myOptType]], PerInstanceOffScreenMasks[optInstances[(int)myOptType]]);
			registered = false;
		}
	}

	public bool Update(Actor a)
	{
		return OptimisationManager.Update(myOptType, a);
	}
}
