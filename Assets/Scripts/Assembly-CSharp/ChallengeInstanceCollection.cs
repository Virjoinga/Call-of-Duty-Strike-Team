using System;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeInstanceCollection : MonoBehaviour
{
	private Dictionary<uint, Challenge> _challengeInstances = new Dictionary<uint, Challenge>();

	public static event EventHandler<ValueEventArgs<uint>> JoinedChanged;

	public void SetChallengeInstance(uint challengeId, Challenge instance)
	{
		_challengeInstances[challengeId] = instance;
		OnJoinedChanged(challengeId);
		EventHub.Instance.Report(new Events.ChallengeJoined());
	}

	public void RemoveChallengeInstance(uint challengeId)
	{
		_challengeInstances.Remove(challengeId);
		OnJoinedChanged(challengeId);
	}

	public Challenge GetChallengeInstance(uint challengeId)
	{
		Challenge value;
		if (!_challengeInstances.TryGetValue(challengeId, out value))
		{
			return null;
		}
		return value;
	}

	private void OnJoinedChanged(uint challengeId)
	{
		if (ChallengeInstanceCollection.JoinedChanged != null)
		{
			ChallengeInstanceCollection.JoinedChanged(this, new ValueEventArgs<uint>(challengeId));
		}
	}
}
