using System;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxDescriptor : ScriptableObject
{
	public enum HitBoxType
	{
		Box = 0,
		Capsule = 1,
		Sphere = 2
	}

	public enum Axis
	{
		X = 0,
		Y = 1,
		Z = 2
	}

	[Serializable]
	public class HitBox
	{
		public string Location;

		public string Bone;

		public HitBoxType Type;

		public Axis Direction;

		public Vector3 Center;

		public float Radius;

		public float Height;

		public Vector3 Size;

		public float Mass;

		public float DamageMultiplier = 1f;

		public SurfaceMaterial DefaultSurfaceMaterial;

		public ExtraSurfaceMarkup SurfaceMarkup;

		public float[] SiblingForceFraction;
	}

	[Serializable]
	public class Constraint
	{
		public string Body;

		public string Connected;

		public Vector3 PrimaryAxis = Vector3.forward;

		public Vector3 SecondaryAxis = Vector3.up;

		public SoftJointLimit LowerAngularXLimit;

		public SoftJointLimit UpperAngularXLimit;

		public SoftJointLimit AngularYLimit;

		public SoftJointLimit AngularZLimit;
	}

	[Serializable]
	public class SoftJointLimit
	{
		public float Limit;

		public float Bounciness;

		public float Spring;

		public float Damper;
	}

	public List<HitBox> HitBoxes;

	public List<Constraint> Constraints;
}
