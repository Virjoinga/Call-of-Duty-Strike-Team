using UnityEngine;

public class DefaultSurfaceMarkup : MonoBehaviour
{
	public SurfaceMaterial Material;

	public ExtraSurfaceMarkup Markup;

	public static void CreateComponent(GameObject owner, SurfaceMaterial material, ExtraSurfaceMarkup extraSurfaceMarkup)
	{
		DefaultSurfaceMarkup defaultSurfaceMarkup = owner.AddComponent<DefaultSurfaceMarkup>();
		defaultSurfaceMarkup.Material = material;
		if (extraSurfaceMarkup != null)
		{
			defaultSurfaceMarkup.Markup = new ExtraSurfaceMarkup();
			defaultSurfaceMarkup.Markup.NoDecals = extraSurfaceMarkup.NoDecals;
		}
	}
}
