public class FriendData
{
	public string UserName { get; private set; }

	public ulong UserId { get; private set; }

	public FriendData(ulong userId, string userName)
	{
		UserId = userId;
		UserName = userName;
	}

	public override string ToString()
	{
		return string.Format("[FriendData: UserName={0}, UserId={1}]", UserName, UserId);
	}
}
