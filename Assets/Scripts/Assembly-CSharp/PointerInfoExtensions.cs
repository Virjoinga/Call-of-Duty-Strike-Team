public static class PointerInfoExtensions
{
	public static InputUtils.PointerInfo ToPointerInfo(this POINTER_INFO ptr)
	{
		return new InputUtils.PointerInfo(ptr.inputDelta.xy(), ptr.devicePos.xy(), ptr.origPos.xy());
	}
}
