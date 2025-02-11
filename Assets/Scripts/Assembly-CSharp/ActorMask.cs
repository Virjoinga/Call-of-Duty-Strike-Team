public class ActorMask
{
	private const int kMaxActorMasks = 8192;

	private const int kInUseBufferSize = 256;

	private const int kInUseBufferMask = 255;

	private static int[] debruijn_table = new int[32]
	{
		0, 1, 28, 2, 29, 14, 24, 3, 30, 22,
		20, 15, 25, 17, 4, 8, 31, 27, 13, 23,
		21, 19, 16, 7, 26, 12, 18, 6, 11, 5,
		10, 9
	};

	private static uint[] buffer = new uint[8192];

	private static int[] inUse = new int[256];

	private static int allocCheck = 0;

	private int index;

	private static int instanceCount = 0;

	public string DebugContents
	{
		get
		{
			char[] array = new char[39];
			for (int i = 1; i < 7; i++)
			{
				array[i * 5 - 1] = ' ';
			}
			for (int i = 0; i < 32; i++)
			{
				if ((buffer[index] & (uint)(1 << i)) != 0)
				{
					array[(i & 3) + i / 4 * 5] = '1';
				}
				else
				{
					array[(i & 3) + i / 4 * 5] = '0';
				}
			}
			return new string(array, 0, 39);
		}
	}

	public ActorMask(uint u, string name)
	{
		instanceCount++;
		while (inUse[allocCheck] == -1)
		{
			allocCheck = (allocCheck + 1) & 0xFF;
		}
		int num = ~inUse[allocCheck];
		index = debruijn_table[((num & -num) * 125613361 >> 27) & 0x1F];
		inUse[allocCheck] |= 1 << index;
		index += allocCheck << 5;
		buffer[index] = u;
	}

	public void SetName(string name)
	{
	}

	public void AppendName(string name)
	{
	}

	~ActorMask()
	{
		if (index >= 0)
		{
			Release();
		}
	}

	public void Set(uint u)
	{
		buffer[index] = u;
	}

	public static void Expunge(uint invident)
	{
		for (int i = 0; i < 256; i++)
		{
			if (inUse[i] != 0)
			{
				int num = i << 5;
				for (int j = 0; j < 32; j++)
				{
					buffer[num++] &= invident;
				}
			}
		}
	}

	public void MaskBy(uint invident)
	{
		TBFAssert.DoAssert(index >= 0, "Attempt made to MaskBy released Actor Mask!");
		buffer[index] &= invident;
	}

	public void Include(uint mask)
	{
		TBFAssert.DoAssert(index >= 0, "Attempt made to Include released Actor Mask!");
		buffer[index] |= mask;
	}

	public void Exclude(uint mask)
	{
		TBFAssert.DoAssert(index >= 0, "Attempt made to Exclude released Actor Mask!");
		buffer[index] &= ~mask;
	}

	public void Release()
	{
		instanceCount--;
		int num = index >> 5;
		inUse[num] &= ~(1 << (index & 0x1F));
		if (num < allocCheck)
		{
			allocCheck = num;
		}
		index = -1;
	}

	public static implicit operator uint(ActorMask am)
	{
		return buffer[am.index];
	}
}
