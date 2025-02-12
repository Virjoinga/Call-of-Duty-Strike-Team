using UnityEngine;

public class NavigationZone : MonoBehaviour
{
	public enum NavigationType
	{
		MantelLow = 0,
		MantelHigh = 1,
		Mantel3M = 2,
		Vent = 3,
		VaultLowWall = 4,
		VaultWindow = 5,
		MantelLowDownOnly = 6,
		MantelLowUpOnly = 7,
		MantelHighDownOnly = 8,
		MantelHighUpOnly = 9,
		Mantel3MDownOnly = 10,
		Mantel3MUpOnly = 11,
		VaultLowWallOneWay = 12,
		VaultWindowOneWay = 13,
		TwoManVault = 14,
		VentOpen = 15,
		MantleLow1M = 16,
		MantleLow1MDownOnly = 17,
		MantleLow1MUpOnly = 18
	}

	public delegate float CalcDuration(Vector3 point1, Vector3 point2);

	private const float kNearLinkThresholdSqr = 0.040000003f;

	public NavigationType Type;

	private Transform[,] mLinkArray;

	private BookingForm[] mBookingForms;

	private void OnTriggerEnter(Collider other)
	{
		BaseCharacter component = other.gameObject.GetComponent<BaseCharacter>();
		if (!(component == null))
		{
		}
	}

	private void OnTriggerExit(Collider other)
	{
		BaseCharacter component = other.gameObject.GetComponent<BaseCharacter>();
		if (!(component == null))
		{
		}
	}

	public void GenerateOffLinks()
	{
		Transform parent = base.transform.parent.parent;
		if (parent == null)
		{
			return;
		}
		Transform transform = base.transform.Find("OffLinks");
		if (transform == null)
		{
			return;
		}
		mLinkArray = new Transform[transform.childCount, 2];
		mBookingForms = new BookingForm[transform.childCount];
		int num = 0;
		foreach (Transform item in transform)
		{
			mBookingForms[num] = new BookingForm(8);
			mLinkArray[num, 0] = item.GetChild(0);
			mLinkArray[num, 1] = item.GetChild(1);
			num++;
		}
	}

	public void EnableLinks()
	{
		Transform transform = base.transform.Find("OffLinks");
		if (transform == null)
		{
			transform = base.transform;
		}
		foreach (Transform item in transform)
		{
			UnityEngine.AI.OffMeshLink component = item.GetComponent<UnityEngine.AI.OffMeshLink>();
			if (component != null)
			{
				component.activated = true;
			}
		}
	}

	private void Awake()
	{
		GenerateOffLinks();
	}

	private void Start()
	{
		if ((bool)NavigationZoneManager.Instance)
		{
			NavigationZoneManager.Instance.OnClearBookings += OnClearBooking;
		}
	}

	private void OnEnable()
	{
		EnableLinks();
	}

	private void OnDestroy()
	{
		if ((bool)NavigationZoneManager.Instance)
		{
			NavigationZoneManager.Instance.OnClearBookings -= OnClearBooking;
		}
	}

	public bool MatchLink(Actor client, float baseTime, Vector3 comingFrom, float speed, CalcDuration dfunc, Vector3 pos1, Vector3 pos2, out Vector3 result, out float delay)
	{
		Vector3 b = new Vector3(5f, 1f, 5f);
		int length = mLinkArray.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			if (Vector3.Scale(mLinkArray[i, 0].position - pos1, b).sqrMagnitude < 1f && Vector3.Scale(mLinkArray[i, 1].position - pos2, b).sqrMagnitude < 1f)
			{
				float time = Vector3.Distance(comingFrom, pos1) / speed + baseTime;
				return FindLinkToBook(client, comingFrom, speed, baseTime, i, time, dfunc, 0, out result, out delay);
			}
			if (Vector3.Scale(mLinkArray[i, 1].position - pos1, b).sqrMagnitude < 1f && Vector3.Scale(mLinkArray[i, 0].position - pos2, b).sqrMagnitude < 1f)
			{
				float time2 = Vector3.Distance(comingFrom, pos1) / speed + baseTime;
				return FindLinkToBook(client, comingFrom, speed, baseTime, i, time2, dfunc, 1, out result, out delay);
			}
		}
		result = Vector3.zero;
		delay = 0f;
		return false;
	}

	private bool FindLinkToBook(Actor client, Vector3 comingFrom, float speed, float baseTime, int first, float time, CalcDuration dfunc, int end, out Vector3 result, out float delay)
	{
		NavigationZoneManager.ClearBookings(client);
		float duration = 0f;
		int length = mLinkArray.GetLength(0);
		int num = first;
		int num2 = 1;
		int num3 = -1;
		if (num < length - 1 && (mLinkArray[num, end].position - comingFrom).sqrMagnitude < (mLinkArray[num + 1, end].position - comingFrom).sqrMagnitude)
		{
			num2 = -num2;
			num3 = -num3;
		}
		float num4 = baseTime + 1000f;
		int num5 = -1;
		delay = 0f;
		for (int i = 0; i < length * 2; i++)
		{
			if (num >= 0 && num < length)
			{
				time = Vector3.Distance(comingFrom, mLinkArray[num, end].position) / speed + baseTime;
				duration = dfunc(mLinkArray[num, end].position, mLinkArray[num, 1 - end].position);
				int slot;
				float num6 = mBookingForms[num].Earliest(time, duration, out slot);
				if (num6 < num4)
				{
					num5 = num;
					num4 = num6;
					delay = num4 - time;
				}
			}
			num += num2;
			num2 = num3 - num2;
			num3 = -num3;
		}
		if (num5 >= 0)
		{
			mBookingForms[num5].Book(client, num4, duration, null);
			result = mLinkArray[num5, end].position;
			return true;
		}
		delay = 0f;
		result = Vector3.zero;
		return false;
	}

	public Transform GetReferenceTransform(Vector3 vDetailedPosition)
	{
		Transform transform = base.transform.Find("ReferenceMesh");
		if (transform != null)
		{
			return transform;
		}
		return base.transform;
	}

	public void GetReferencePositionRotation(Vector3 vDetailedPosition, out Vector3 pos, out Quaternion rot)
	{
		Transform transform = base.transform.Find("ReferenceMesh");
		if (transform == null)
		{
			transform = base.transform;
		}
		pos = transform.position;
		rot = transform.rotation;
		pos += transform.right * Vector3.Dot(vDetailedPosition - transform.position, transform.right);
	}

	private void OnClearBooking(Actor client)
	{
		if (mLinkArray != null)
		{
			for (int i = 0; i < mLinkArray.GetLength(0); i++)
			{
				mBookingForms[i].Cancel(client);
			}
		}
	}
}
