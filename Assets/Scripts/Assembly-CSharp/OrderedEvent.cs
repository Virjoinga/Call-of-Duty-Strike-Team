public class OrderedEvent<T>
{
	public int mPriotiry;

	public T mFunc;

	public OrderedEvent(int priority, T func)
	{
		mPriotiry = priority;
		mFunc = func;
	}
}
