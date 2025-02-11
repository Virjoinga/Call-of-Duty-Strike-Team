using UnityEngine;

public class NanniedBehaviour : MonoBehaviour
{
	protected void NannyMe()
	{
		SceneNanny.NannyMe(Tug);
	}

	private void Tug(SceneNanny.TugType t)
	{
		switch (t)
		{
		case SceneNanny.TugType.kDestroy:
			Object.DestroyObject(this);
			break;
		case SceneNanny.TugType.kDisable:
			base.enabled = false;
			break;
		case SceneNanny.TugType.kEnable:
			base.enabled = true;
			break;
		}
	}
}
