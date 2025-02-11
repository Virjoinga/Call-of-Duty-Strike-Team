using UnityEngine;

public class TransformUtils
{
	public static void ModifyTransformToAlignPoints(Transform t, Transform a, Transform b)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.SetRow(0, t.right);
		identity.SetRow(1, t.up);
		identity.SetRow(2, t.forward);
		Matrix4x4 identity2 = Matrix4x4.identity;
		identity2.SetRow(0, a.right);
		identity2.SetRow(1, a.up);
		identity2.SetRow(2, a.forward);
		Matrix4x4 identity3 = Matrix4x4.identity;
		identity3.SetColumn(0, b.right);
		identity3.SetColumn(1, b.up);
		identity3.SetColumn(2, b.forward);
		Matrix4x4 matrix4x = identity2 * identity3 * identity;
		t.right = matrix4x.GetRow(0);
		t.up = matrix4x.GetRow(1);
		t.forward = matrix4x.GetRow(2);
		t.Translate(a.position - b.position, Space.World);
	}
}
