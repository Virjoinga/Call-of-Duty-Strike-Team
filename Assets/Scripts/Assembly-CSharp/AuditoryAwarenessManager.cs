using System;
using System.Collections.Generic;
using UnityEngine;

public class AuditoryAwarenessManager : MonoBehaviour
{
	public class HeardEventArgs : EventArgs
	{
		public AuditoryAwarenessComponent Listener;

		public AuditoryAwarenessComponent Target;

		public AuditoryAwarenessEvent TargetEvent;

		public AAWorldEvent TargetWorldEvent;

		public HeardEventArgs(AuditoryAwarenessComponent listener, AuditoryAwarenessComponent target)
		{
			Listener = listener;
			Target = target;
		}

		public HeardEventArgs(AuditoryAwarenessComponent listener, AuditoryAwarenessEvent target)
		{
			Listener = listener;
			TargetEvent = target;
		}

		public HeardEventArgs(AuditoryAwarenessComponent listener, AAWorldEvent target)
		{
			Listener = listener;
			TargetWorldEvent = target;
		}
	}

	public class AAWorldEvent
	{
		private Actor mOwner;

		private Vector3 mOrigin;

		private float mRadius;

		public Actor Owner
		{
			get
			{
				return mOwner;
			}
		}

		public Vector3 Origin
		{
			get
			{
				return mOrigin;
			}
		}

		public float Radius
		{
			get
			{
				return mRadius;
			}
		}

		public AAWorldEvent(Vector3 origin, float radius, Actor owner)
		{
			mOrigin = origin;
			mRadius = radius;
			mOwner = owner;
		}
	}

	public delegate void OnHeardEventHandler(object sender, EventArgs args);

	private static AuditoryAwarenessManager smInstance;

	private int mUpdateHearingSpread;

	private List<AuditoryAwarenessComponent> mAANodes;

	private List<AuditoryAwarenessEvent> mAAEvents;

	private List<AAWorldEvent> mAAWorldEvents;

	public static AuditoryAwarenessManager Instance
	{
		get
		{
			return smInstance;
		}
	}

	public event OnHeardEventHandler OnHeard;

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple AuditoryAwarenessManager");
		}
		smInstance = this;
		mAANodes = new List<AuditoryAwarenessComponent>();
		mAAEvents = new List<AuditoryAwarenessEvent>();
		mAAWorldEvents = new List<AAWorldEvent>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (mUpdateHearingSpread >= mAANodes.Count)
		{
			mUpdateHearingSpread = 0;
		}
		for (int i = 0; i < mAANodes.Count; i++)
		{
			UpdateEvents(mAANodes[i]);
		}
		mUpdateHearingSpread++;
		mAAEvents.Clear();
		mAAWorldEvents.Clear();
	}

	public void AddAuditoryAwarenessComponent(AuditoryAwarenessComponent aac)
	{
		mAANodes.Add(aac);
	}

	public void RemoveAuditoryAwarenessComponent(AuditoryAwarenessComponent aac)
	{
		mAANodes.Remove(aac);
		foreach (AuditoryAwarenessComponent mAANode in mAANodes)
		{
			mAANode.ICanHearList.Remove(aac);
		}
	}

	public void RegisterEvent(AuditoryAwarenessEvent aaEvent)
	{
		mAAEvents.Add(aaEvent);
	}

	public void RegisterEventInWorld(Vector3 origin, float radius, Actor owner)
	{
		AAWorldEvent item = new AAWorldEvent(origin, radius, owner);
		mAAWorldEvents.Add(item);
	}

	public void GlDebugVisualise()
	{
		GL.PushMatrix();
		DebugDraw.LineMaterial.SetPass(0);
		GL.Color(Color.blue);
		GL.Begin(1);
		foreach (AuditoryAwarenessComponent mAANode in mAANodes)
		{
			if (mAANode.CanBeHeard)
			{
				mAANode.GlDebugVisualise();
			}
		}
		GL.End();
		GL.PopMatrix();
	}

	private void UpdateAAC(AuditoryAwarenessComponent source, bool bUpdateCanHear)
	{
		List<AuditoryAwarenessComponent> couldHear = new List<AuditoryAwarenessComponent>(source.ICanHearList);
		source.ClearCanHear();
		if (source.CanHear && bUpdateCanHear)
		{
			source.UpdateCanHear(mAANodes);
		}
		UpdateHeardEvents(source, couldHear);
	}

	private void UpdateEvents(AuditoryAwarenessComponent source)
	{
		if (!source.CanHear)
		{
			return;
		}
		for (int i = 0; i < mAAEvents.Count; i++)
		{
			if (source.CanHearEvent(mAAEvents[i]))
			{
				this.OnHeard(this, new HeardEventArgs(source, mAAEvents[i]));
			}
		}
		for (int j = 0; j < mAAWorldEvents.Count; j++)
		{
			if (source.CanHearEvent(mAAWorldEvents[j]))
			{
				this.OnHeard(this, new HeardEventArgs(source, mAAWorldEvents[j]));
			}
		}
	}

	private void UpdateHeardEvents(AuditoryAwarenessComponent source, List<AuditoryAwarenessComponent> couldHear)
	{
		foreach (AuditoryAwarenessComponent iCanHear in source.ICanHearList)
		{
			if (!couldHear.Contains(iCanHear))
			{
				this.OnHeard(this, new HeardEventArgs(source, iCanHear));
			}
		}
	}
}
