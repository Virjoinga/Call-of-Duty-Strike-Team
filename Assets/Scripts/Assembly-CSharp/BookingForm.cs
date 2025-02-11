using UnityEngine;

public class BookingForm
{
	private const float never = 1000000f;

	public int slots = 8;

	public float[,] mTimes;

	public string[] mTags;

	private Actor[] mClients;

	public BookingForm(int s)
	{
		slots = s;
		mTimes = new float[slots, 2];
		mClients = new Actor[slots];
		for (int i = 0; i < slots; i++)
		{
			mTimes[i, 0] = 1000000f;
			mTimes[i, 1] = 1000000f;
		}
		mTags = new string[slots];
	}

	public float Earliest(float time, float duration, out int slot)
	{
		int i;
		for (i = 0; i < slots; i++)
		{
			if (mTimes[i, 1] > time && mTimes[i, 0] < time + duration)
			{
				if (!(mTimes[i, 1] - time < time + duration - mTimes[i, 0]))
				{
					TaskSetPiece taskSetPiece = (TaskSetPiece)mClients[i].tasks.GetRunningTask(typeof(TaskSetPiece));
					if (taskSetPiece != null && !taskSetPiece.CanReschedule())
					{
					}
					break;
				}
				time = mTimes[i, 1] + 0.01f;
			}
			else if (mTimes[i, 0] > time)
			{
				break;
			}
		}
		slot = i;
		return time;
	}

	public void Book(Actor client, float time, float duration, string tag)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < slots; i++)
		{
			TBFAssert.DoAssert(mClients[i] != client);
		}
		for (num2 = 0; num2 < slots && !(mTimes[num2, 1] > Time.time); num2++)
		{
		}
		if (num2 > num)
		{
			while (mTimes[num2, 1] < time)
			{
				mTimes[num, 0] = mTimes[num2, 0];
				mTimes[num, 1] = mTimes[num2, 1];
				mClients[num] = mClients[num2];
				mTags[num] = mTags[num2];
				num2++;
				num++;
			}
		}
		else
		{
			while (mTimes[num2, 1] < time)
			{
				num2++;
				num++;
				if (num2 >= slots)
				{
					return;
				}
			}
		}
		for (; mTimes[num2, 0] < time + duration; num2++)
		{
			TaskSetPiece taskSetPiece = (TaskSetPiece)mClients[num2].tasks.GetRunningTask(typeof(TaskSetPiece));
			if (taskSetPiece != null)
			{
				taskSetPiece.Reschedule();
			}
			mClients[num2] = null;
		}
		if (num2 > num)
		{
			mTimes[num, 0] = time;
			mTimes[num, 1] = time + duration;
			mClients[num] = client;
			mTags[num] = tag;
			num++;
			if (num2 > num)
			{
				while (num2 < slots)
				{
					mTimes[num, 0] = mTimes[num2, 0];
					mTimes[num, 1] = mTimes[num2, 1];
					mClients[num] = mClients[num2];
					mTags[num] = mTags[num2];
					num2++;
					num++;
				}
				for (; num < slots; num++)
				{
					mTimes[num, 0] = 1000000f;
					mTimes[num, 1] = 1000000f;
					mClients[num] = null;
					mTags[num] = string.Empty;
				}
			}
		}
		else
		{
			for (int i = slots - 1; i > num2; i--)
			{
				mTimes[i, 0] = mTimes[i - 1, 0];
				mTimes[i, 1] = mTimes[i - 1, 1];
				mClients[i] = mClients[i - 1];
				mTags[i] = mTags[i - 1];
			}
			mTimes[num2, 0] = time;
			mTimes[num2, 1] = time + duration;
			mClients[num2] = client;
			mTags[num2] = tag;
		}
	}

	public float FirstBookingTime()
	{
		for (int i = 0; i < slots; i++)
		{
			if (mTimes[i, 0] > WorldHelper.ThisFrameTime)
			{
				return mTimes[i, 0];
			}
		}
		return 0f;
	}

	public void Cancel(Actor client)
	{
		int i = 0;
		for (int j = 0; j < slots; j++)
		{
			if (mClients[j] != client)
			{
				if (j > i)
				{
					mClients[i] = mClients[j];
					mTimes[i, 0] = mTimes[j, 0];
					mTimes[i, 1] = mTimes[j, 1];
				}
				i++;
			}
		}
		for (; i < slots; i++)
		{
			mClients[i] = null;
			mTimes[i, 0] = 1000000f;
			mTimes[i, 1] = 1000000f;
		}
	}

	public void DrawGizmos(Vector3 pos)
	{
		Gizmos.color = Color.black;
		Gizmos.DrawCube(pos, new Vector3(0.5f, 0.05f, 0.5f));
		Color color = new Color(0.5f, 0f, 0f, 1f);
		for (int i = 0; i < slots; i++)
		{
			Gizmos.color = color;
			if (mTimes[i, 0] < 1000000f && mTimes[i, 1] > Time.time)
			{
				float num = mTimes[i, 0];
				if (num < Time.time)
				{
					Gizmos.color = Color.red;
					num = Time.time;
				}
				Vector3 vector = pos + new Vector3(0f, (num + mTimes[i, 1]) * 0.5f - Time.time, 0f);
				Vector3 size = new Vector3(0.2f, mTimes[i, 1] - num, 0.2f);
				Gizmos.DrawCube(vector, size);
				Gizmos.DrawLine(vector, mClients[i].GetPosition());
			}
		}
		Gizmos.color = Color.white;
	}
}
