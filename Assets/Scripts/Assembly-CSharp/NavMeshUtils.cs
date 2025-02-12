using System;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshUtils
{
	private static List<string> mPossibleLayers;

	private static void Initialise()
	{
		if (mPossibleLayers == null)
		{
			mPossibleLayers = new List<string>();
			mPossibleLayers.Add("Default");
			mPossibleLayers.Add("Not Walkable");
			mPossibleLayers.Add("Jump");
			mPossibleLayers.Add("Camera");
			mPossibleLayers.Add("EnemyOnly");
			for (int i = 0; i < 32; i++)
			{
				string item = string.Format("Portal{0}", i);
				mPossibleLayers.Add(item);
			}
		}
	}

	public static UnityEngine.AI.NavMeshHit SampleNavMesh(Vector3 position)
	{
		return SampleNavMesh(position, -1);
	}

	public static UnityEngine.AI.NavMeshHit SampleNavMesh(Vector3 position, int layerMask)
	{
		float num = 0.1f;
		UnityEngine.AI.NavMeshHit hit = default(UnityEngine.AI.NavMeshHit);
		while (!hit.hit && num < 128f)
		{
			UnityEngine.AI.NavMesh.SamplePosition(position, out hit, num, layerMask);
			num *= 2f;
		}
		return hit;
	}

	public static string GetNavMeshLayerName(int mask)
	{
		Initialise();
		List<string> list = new List<string>();
		foreach (string mPossibleLayer in mPossibleLayers)
		{
			int navMeshLayerFromName = UnityEngine.AI.NavMesh.GetNavMeshLayerFromName(mPossibleLayer);
			if (navMeshLayerFromName != -1 && (mask & (1 << navMeshLayerFromName)) != 0)
			{
				list.Add(mPossibleLayer);
			}
		}
		return string.Join(Environment.NewLine, list.ToArray());
	}

	public static string FindClosestPortal(Vector3 testPosition, float maxDistance)
	{
		string result = null;
		float num = float.MaxValue;
		for (int i = 0; i < 32; i++)
		{
			string text = string.Format("Portal{0}", i);
			int navMeshLayerFromName = UnityEngine.AI.NavMesh.GetNavMeshLayerFromName(text);
			UnityEngine.AI.NavMeshHit hit;
			if (navMeshLayerFromName != -1 && UnityEngine.AI.NavMesh.SamplePosition(testPosition, out hit, maxDistance, 1 << navMeshLayerFromName))
			{
				float num2 = Vector3.Distance(hit.position, testPosition);
				if (num2 < num)
				{
					result = text;
					num = num2;
				}
			}
		}
		return result;
	}
}
