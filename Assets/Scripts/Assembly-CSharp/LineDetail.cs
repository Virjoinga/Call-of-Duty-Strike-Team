using System;
using UnityEngine;

[Serializable]
public class LineDetail
{
	public LineFlag flag;

	public Transform trans;

	public Color color;

	public LineDetail()
	{
	}

	public LineDetail(LineFlag inFlag, Transform inTrans, Color inColor)
	{
		flag = inFlag;
		trans = inTrans;
		color = inColor;
	}
}
