using System.Collections;
using UnityEngine;

public class TBFAutoProfileResultsPoster : MonoBehaviour
{
	public void postResults(string url, WWWForm formData)
	{
		StartCoroutine(coroutinePost(url, formData));
	}

	private IEnumerator coroutinePost(string url, WWWForm formData)
	{
		yield return new WWW(url, formData);
		Object.Destroy(base.gameObject);
	}
}
