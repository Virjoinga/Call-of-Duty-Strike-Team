using UnityEngine;

public class BodyDropSfxPlayer
{
	private SurfaceMaterial CurrentMaterial(Vector3 position)
	{
		position += Vector3.up;
		SurfaceImpact surfaceImpact = ProjectileManager.Trace(position, position + Vector3.down, ProjectileManager.DefaultLayerProjectileMask);
		return surfaceImpact.material;
	}

	public void Play(Vector3 position, GameObject gameObject)
	{
		if (!OverwatchController.Instance || !OverwatchController.Instance.Active)
		{
			switch (CurrentMaterial(position))
			{
			case SurfaceMaterial.Default:
				BodyFallSFX.Instance.BodyFallGeneric.Play(gameObject);
				break;
			case SurfaceMaterial.Masonry:
				BodyFallSFX.Instance.BodyFallGeneric.Play(gameObject);
				break;
			case SurfaceMaterial.Metal:
				BodyFallSFX.Instance.BodyFallMetalSolid.Play(gameObject);
				break;
			case SurfaceMaterial.Wood:
				BodyFallSFX.Instance.BodyFallWood.Play(gameObject);
				break;
			case SurfaceMaterial.Snow:
				BodyFallSFX.Instance.BodyFallSnow.Play(gameObject);
				break;
			case SurfaceMaterial.Ice:
				BodyFallSFX.Instance.BodyFallGeneric.Play(gameObject);
				break;
			case SurfaceMaterial.Water:
				BodyFallSFX.Instance.BodyFallWater.Play(gameObject);
				break;
			case SurfaceMaterial.Carpet:
				BodyFallSFX.Instance.BodyFallGeneric.Play(gameObject);
				break;
			case SurfaceMaterial.Dirt:
				BodyFallSFX.Instance.BodyFallDirt.Play(gameObject);
				break;
			case SurfaceMaterial.Duct:
				break;
			case SurfaceMaterial.Glass:
				BodyFallSFX.Instance.BodyFallGeneric.Play(gameObject);
				break;
			case SurfaceMaterial.Grass:
				BodyFallSFX.Instance.BodyFallGeneric.Play(gameObject);
				break;
			case SurfaceMaterial.Leaves:
				BodyFallSFX.Instance.BodyFallGeneric.Play(gameObject);
				break;
			case SurfaceMaterial.Mud:
				BodyFallSFX.Instance.BodyFallGeneric.Play(gameObject);
				break;
			case SurfaceMaterial.Cement:
				BodyFallSFX.Instance.BodyFallStone.Play(gameObject);
				break;
			case SurfaceMaterial.Sand:
				BodyFallSFX.Instance.BodyFallGeneric.Play(gameObject);
				break;
			case SurfaceMaterial.None:
				BodyFallSFX.Instance.BodyFallGeneric.Play(gameObject);
				break;
			case SurfaceMaterial.Flesh:
				break;
			}
		}
	}
}
