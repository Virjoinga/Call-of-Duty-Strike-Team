using UnityEngine;

public class RedirectBone
{
	public Transform srcTrans;

	public Transform dstTrans;

	public RedirectBone(Transform src, Transform dst)
	{
		srcTrans = src;
		dstTrans = dst;
	}
}
