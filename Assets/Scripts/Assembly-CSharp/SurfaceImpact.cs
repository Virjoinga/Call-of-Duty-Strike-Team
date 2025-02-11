using System;
using UnityEngine;

[Serializable]
public class SurfaceImpact
{
	public GameObject gameobject;

	public SurfaceMaterial material;

	public Vector3 position;

	public Vector3 direction;

	public Vector3 normal;

	public bool noDecal;

	public SurfaceImpact(GameObject gameobject, SurfaceMaterial material, Vector3 position, Vector3 direction, Vector3 normal, bool noDecal)
	{
		this.gameobject = gameobject;
		this.material = material;
		this.position = position;
		this.direction = direction;
		this.normal = normal;
		this.noDecal = noDecal;
	}
}
