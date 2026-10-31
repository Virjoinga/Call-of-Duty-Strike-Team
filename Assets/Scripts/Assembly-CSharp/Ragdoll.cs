using System;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
	private struct BoneTransform
	{
		public Vector3 Position;

		public Quaternion Rotation;
	}

	private const int SLEEP_INC_CHECK = 25;

	public HitLocation[] Bones;

	public HitLocation Pelvis;

	public HitBoxDescriptor HitBoxDesc;

	private Dictionary<string, Rigidbody> bodiesCache;

	private BoneTransform[] mPreviousTransforms;

	private bool mPreviousTransformsValid;

	private bool mDeadAsleep;

	private bool mBeenBaked;

	private bool mUpdateBones;

	private bool mBonesParented;

	private Actor mCachedActorRef;

	private int mSleepPreviousFrameCount;

	private bool mCheckForFall;

	private SkinnedMeshRenderer[] mSMRArray;

	private bool mKinematic;

	private bool mRagdollAttached;

	public bool TrackVelocities { get; set; }

	public bool Kinematic
	{
		get
		{
			return mKinematic;
		}
	}

	public void Start()
	{
		mPreviousTransforms = new BoneTransform[Bones.Length];
		SwitchToKinematic();
		if (!(Pelvis == null))
		{
			return;
		}
		HitLocation[] bones = Bones;
		foreach (HitLocation hitLocation in bones)
		{
			if (hitLocation.Location == "Pelvis")
			{
				Pelvis = hitLocation;
				break;
			}
		}
		if (Pelvis == null && Bones.Length > 0)
		{
			Pelvis = Bones[0];
		}
		if (Pelvis.Owner != null)
		{
			mCachedActorRef = Pelvis.Owner.GetComponent<Actor>();
		}
	}

	public void OnDestroy()
	{
		HitLocation[] bones = Bones;
		foreach (HitLocation hitLocation in bones)
		{
			if (hitLocation != null)
			{
				UnityEngine.Object.Destroy(hitLocation.gameObject);
			}
		}
	}

	public void SwitchToKinematic()
	{
		mKinematic = true;
		HitLocation[] bones = Bones;
		foreach (HitLocation hitLocation in bones)
		{
			if (hitLocation.GetComponent<Rigidbody>() != null)
			{
				hitLocation.GetComponent<Rigidbody>().isKinematic = true;
				hitLocation.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
				hitLocation.GetComponent<Rigidbody>().detectCollisions = true;
			}
			hitLocation.transform.localPosition = Vector3.zero;
			hitLocation.transform.localRotation = Quaternion.identity;
		}
	}

	public void ParentBonesPerm()
	{
		for (int i = 0; i < Bones.Length; i++)
		{
			Bones[i].transform.parent = Bones[i].Bone;
			Bones[i].transform.localPosition = Vector3.zero;
			Bones[i].transform.localRotation = Quaternion.identity;
		}
	}

	public void ParentBones()
	{
		if (mKinematic)
		{
			for (int i = 0; i < Bones.Length; i++)
			{
				Bones[i].transform.parent = Bones[i].Bone;
				Bones[i].transform.localPosition = Vector3.zero;
				Bones[i].transform.localRotation = Quaternion.identity;
			}
			mUpdateBones = true;
			mBonesParented = true;
		}
	}

	public void UnParentBones()
	{
		mBonesParented = false;
		if (mKinematic)
		{
			Vector3 localPosition = new Vector3(0f, 5000f, 0f);
			for (int i = 0; i < Bones.Length; i++)
			{
				Bones[i].transform.parent = null;
				Bones[i].transform.localPosition = localPosition;
			}
			mUpdateBones = false;
		}
	}

	public void SwitchToRagdoll()
	{
		if (!mKinematic)
		{
			return;
		}
		for (int i = 0; i < Bones.Length; i++)
		{
			Bones[i].transform.parent = null;
			Bones[i].transform.position = Bones[i].Bone.transform.position;
			Bones[i].transform.rotation = Bones[i].Bone.transform.rotation;
			Rigidbody body = Bones[i].GetComponent<Rigidbody>();
			if (body != null)
			{
				body.position = Bones[i].Bone.position;
				body.rotation = Bones[i].Bone.rotation;
			}
		}
		mKinematic = false;
		mBonesParented = false;
		mCheckForFall = true;
		mSleepPreviousFrameCount = 0;
		if (!mRagdollAttached)
		{
			AddRagdollParts();
		}
		// Bind linear anchors to the current animated pose before waking any body.
		foreach (HitLocation bone in Bones)
		{
			ConfigurableJoint joint = bone.GetComponent<ConfigurableJoint>();
			if (joint != null && joint.connectedBody != null)
			{
				joint.autoConfigureConnectedAnchor = false;
				joint.connectedAnchor = joint.connectedBody.transform.InverseTransformPoint(
					joint.transform.TransformPoint(joint.anchor));
			}
		}
		int num = Bones.Length;
		int layer = LayerMask.NameToLayer("Default");
		bool flag = mCachedActorRef != null && mCachedActorRef.realCharacter != null;
		for (int j = 0; j < num; j++)
		{
			HitLocation hitLocation = Bones[j];
			if (hitLocation.GetComponent<Rigidbody>() != null)
			{
				hitLocation.GetComponent<Rigidbody>().isKinematic = false;
				hitLocation.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
				hitLocation.GetComponent<Rigidbody>().freezeRotation = false;
				hitLocation.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
				hitLocation.GetComponent<Rigidbody>().detectCollisions = true;
				hitLocation.GetComponent<Rigidbody>().gameObject.layer = layer;
				hitLocation.GetComponent<Rigidbody>().velocity = Vector3.zero;
				hitLocation.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				if (mPreviousTransforms != null && mPreviousTransformsValid && Time.deltaTime > 0f)
				{
					BoneTransform boneTransform = mPreviousTransforms[j];
					hitLocation.GetComponent<Rigidbody>().velocity = (hitLocation.transform.position - boneTransform.Position) / Time.deltaTime;
					hitLocation.GetComponent<Rigidbody>().angularVelocity = CalculateAngularVelocity(
						boneTransform.Rotation, hitLocation.transform.rotation, Time.deltaTime);
				}
				else
				{
					if (flag)
					{
						hitLocation.GetComponent<Rigidbody>().velocity = mCachedActorRef.realCharacter.lastVelocity * 1.5f;
					}
					if (hitLocation.Location == "Shield")
					{
						hitLocation.GetComponent<Rigidbody>().angularVelocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
					}
				}
			}
			hitLocation.transform.parent = base.transform;
		}
	}

	public void CopyFrom(Ragdoll other)
	{
		if (Bones.Length != other.Bones.Length)
		{
			Debug.LogWarning("Tried to copy from a ragdoll with non-matching bones");
			return;
		}
		mPreviousTransformsValid = mPreviousTransforms != null && other.mPreviousTransforms != null && other.mPreviousTransformsValid;
		for (int i = 0; i < Bones.Length; i++)
		{
			Bones[i].transform.position = other.Bones[i].transform.position;
			Bones[i].transform.rotation = other.Bones[i].transform.rotation;
			Bones[i].GetComponent<Rigidbody>().velocity = other.Bones[i].GetComponent<Rigidbody>().velocity;
			Bones[i].GetComponent<Rigidbody>().angularVelocity = other.Bones[i].GetComponent<Rigidbody>().angularVelocity;
			if (mPreviousTransformsValid)
			{
				mPreviousTransforms[i].Position = other.mPreviousTransforms[i].Position;
				mPreviousTransforms[i].Rotation = other.mPreviousTransforms[i].Rotation;
			}
		}
	}

	private void Update()
	{
		mPreviousTransformsValid = false;
		if (TrackVelocities && mKinematic && mPreviousTransforms != null)
		{
			int num = Bones.Length;
			for (int i = 0; i < num; i++)
			{
				HitLocation hitLocation = Bones[i];
				BoneTransform boneTransform = default(BoneTransform);
				// Query-only hitboxes may be parked offscreen between updates.
				boneTransform.Position = hitLocation.Bone.position;
				boneTransform.Rotation = hitLocation.Bone.rotation;
				mPreviousTransforms[i] = boneTransform;
			}
			mPreviousTransformsValid = true;
		}
	}

	private static Vector3 CalculateAngularVelocity(Quaternion previous, Quaternion current, float deltaTime)
	{
		if (deltaTime <= 0f) return Vector3.zero;
		float angle;
		Vector3 axis;
		(current * Quaternion.Inverse(previous)).ToAngleAxis(out angle, out axis);
		if (angle > 180f) angle -= 360f;
		if (Mathf.Abs(angle) < 0.0001f) return Vector3.zero;
		return axis * (angle * Mathf.Deg2Rad / deltaTime);
	}

	private void CopyRagdollToBones(float drag)
	{
		for (int i = 0; i < Bones.Length; i++)
		{
			Bones[i].Bone.position = Bones[i].transform.position;
			Bones[i].Bone.rotation = Bones[i].transform.rotation;
			if (Bones[i].Location != "Shield")
			{
				Bones[i].GetComponent<Rigidbody>().drag = drag;
			}
		}
	}

	private void LateUpdate()
	{
		bool flag = true;
		for (int i = 0; i < Bones.Length; i++)
		{
			if (!Bones[i].GetComponent<Rigidbody>().IsSleeping())
			{
				flag = false;
				mSleepPreviousFrameCount = 0;
				break;
			}
		}
		if (!mKinematic)
		{
			float drag = 0f;
			if (Pelvis != null)
			{
				float sqrMagnitude = Pelvis.GetComponent<Rigidbody>().velocity.sqrMagnitude;
				if (sqrMagnitude > 0f && sqrMagnitude < 0.05f)
				{
					drag = 10f;
				}
			}
			if (!mDeadAsleep)
			{
				CopyRagdollToBones(drag);
			}
			if (flag && !mDeadAsleep && mSleepPreviousFrameCount == 25 && Pelvis != null)
			{
				if (mCachedActorRef != null && (bool)mCachedActorRef.Pose)
				{
					mCachedActorRef.OnScreen = true;
					mCachedActorRef.Pose.PostModuleUpdate();
				}
				CopyRagdollToBones(drag);
				BakeSkinnedMesh();
				SwitchToKinematic();
			}
		}
		else if (mDeadAsleep && mCachedActorRef != null && (mCachedActorRef.realCharacter.IsBeingCarried || mCachedActorRef.realCharacter.IsInASetPiece))
		{
			ReawakenSkinnedMesh();
		}
		else if (!mDeadAsleep && flag && mBeenBaked && mCachedActorRef != null && !mCachedActorRef.realCharacter.IsBeingCarried && !mCachedActorRef.realCharacter.IsInASetPiece && mCachedActorRef.Pose.direction == PoseModuleSharedData.BlendDirection.None)
		{
			BakeSkinnedMesh();
		}
		else if (mUpdateBones)
		{
			mUpdateBones = false;
		}
		else if (mBonesParented)
		{
			UnParentBones();
		}
		if (flag && !mKinematic && !mCheckForFall)
		{
			mSleepPreviousFrameCount++;
		}
		if (mCheckForFall && !mKinematic && !flag)
		{
			mCheckForFall = false;
		}
	}

	private void BakeSkinnedMesh()
	{
		if (mCachedActorRef == null && Pelvis != null && Pelvis.Owner != null)
		{
			mCachedActorRef = Pelvis.Owner.GetComponent<Actor>();
		}
		if (!(mCachedActorRef != null) || !(mCachedActorRef.model != null) || mCachedActorRef.realCharacter.IsBeingCarried)
		{
			return;
		}
		SkinnedMeshRenderer[] componentsInChildren = mCachedActorRef.model.GetComponentsInChildren<SkinnedMeshRenderer>();
		List<Renderer> list = new List<Renderer>();
		mSMRArray = componentsInChildren;
		LODGroup component = mCachedActorRef.model.GetComponent<LODGroup>();
		int num = 0;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!componentsInChildren[i].name.Contains("RPG"))
			{
				num++;
			}
		}
		LOD[] array = new LOD[num];
		int num2 = 0;
		SkinnedMeshRenderer[] array2 = componentsInChildren;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array2)
		{
			if (!(skinnedMeshRenderer.gameObject.GetComponent<MeshFilter>() == null) || skinnedMeshRenderer.name.Contains("RPG"))
			{
				continue;
			}
			Mesh mesh = new Mesh();
			skinnedMeshRenderer.BakeMesh(mesh);
			mesh.name = skinnedMeshRenderer.sharedMesh.name;
			MeshFilter meshFilter = skinnedMeshRenderer.gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			meshFilter.mesh.RecalculateBounds();
			MeshRenderer meshRenderer = skinnedMeshRenderer.gameObject.AddComponent<MeshRenderer>();
			meshRenderer.materials = skinnedMeshRenderer.materials;
			skinnedMeshRenderer.gameObject.transform.localScale = Vector3.one;
			skinnedMeshRenderer.enabled = false;
			if (meshRenderer.material.shader.name.Contains("Probe"))
			{
				list.Add(meshRenderer);
			}
			Renderer[] renderers = new Renderer[1] { meshRenderer };
			switch (num2)
			{
			case 0:
				array[num2] = new LOD(0.05f, renderers);
				break;
			case 1:
				array[num2] = new LOD(0.01f, renderers);
				break;
			default:
				if (0.009999999776482582 - 0.001 * (double)num2 > 0.0)
				{
					array[num2] = new LOD(0.01f - 0.001f * (float)num2, renderers);
				}
				else
				{
					array[num2] = new LOD(0f, renderers);
				}
				break;
			}
			num2++;
		}
		if (mCachedActorRef.realCharacter.Lighting != null)
		{
			mCachedActorRef.realCharacter.Lighting.Renderers = list.ToArray();
			mCachedActorRef.realCharacter.Lighting.UpdateMaterials(true);
		}
		if (component != null)
		{
			component.SetLODS(array);
			component.RecalculateBounds();
		}
		mDeadAsleep = true;
		mBeenBaked = true;
	}

	public void ReawakenSkinnedMesh()
	{
		if (!mDeadAsleep || !(mCachedActorRef != null) || !(mCachedActorRef.model != null))
		{
			return;
		}
		SkinnedMeshRenderer[] array = mSMRArray;
		List<Renderer> list = new List<Renderer>();
		SkinnedMeshRenderer[] array2 = array;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array2)
		{
			MeshFilter component = skinnedMeshRenderer.gameObject.GetComponent<MeshFilter>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			MeshRenderer component2 = skinnedMeshRenderer.gameObject.GetComponent<MeshRenderer>();
			if (component2 != null)
			{
				UnityEngine.Object.Destroy(component2);
			}
			skinnedMeshRenderer.enabled = true;
			if (skinnedMeshRenderer.material.shader.name.Contains("Probe"))
			{
				list.Add(skinnedMeshRenderer);
			}
		}
		if (mCachedActorRef.realCharacter.Lighting != null)
		{
			mCachedActorRef.realCharacter.Lighting.Renderers = list.ToArray();
			mCachedActorRef.realCharacter.Lighting.UpdateMaterials(true);
		}
		mDeadAsleep = false;
	}

	public void AddRagdollParts()
	{
		Dictionary<string, Rigidbody> dictionary = new Dictionary<string, Rigidbody>();
		HitLocation[] bones = Bones;
		foreach (HitLocation hitLocation in bones)
		{
			Rigidbody rigidbody = hitLocation.gameObject.AddComponent<Rigidbody>();
			rigidbody.freezeRotation = true;
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			rigidbody.mass = hitLocation.Mass;
			rigidbody.solverIterations = 12;
			rigidbody.solverVelocityIterations = 4;
			dictionary[hitLocation.Location] = rigidbody;
			hitLocation.GetComponent<Collider>().enabled = true;
			rigidbody.isKinematic = true;
			rigidbody.detectCollisions = true;
		}
		bodiesCache = dictionary;
		mRagdollAttached = true;
		AddRagdollConstraints();
	}

	public void AddRagdollConstraints()
	{
		foreach (HitBoxDescriptor.Constraint constraint in HitBoxDesc.Constraints)
		{
			if (bodiesCache.ContainsKey(constraint.Body) && bodiesCache.ContainsKey(constraint.Connected))
			{
				Rigidbody rigidbody = bodiesCache[constraint.Body];
				Rigidbody connectedBody = bodiesCache[constraint.Connected];
				ConfigurableJoint configurableJoint = rigidbody.gameObject.AddComponent<ConfigurableJoint>();
				configurableJoint.connectedBody = connectedBody;
				configurableJoint.axis = constraint.PrimaryAxis;
				configurableJoint.secondaryAxis = constraint.SecondaryAxis;
				configurableJoint.xMotion = ConfigurableJointMotion.Locked;
				configurableJoint.yMotion = ConfigurableJointMotion.Locked;
				configurableJoint.zMotion = ConfigurableJointMotion.Locked;
				configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
				configurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
				configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;
				HitBoxDescriptor.SoftJointLimitSpring low = constraint.LowerAngularXLimit;
				HitBoxDescriptor.SoftJointLimitSpring high = constraint.UpperAngularXLimit;
				if (low.Limit > high.Limit)
				{
					HitBoxDescriptor.SoftJointLimitSpring swap = low;
					low = high;
					high = swap;
				}
				// Unity clamps each endpoint against the other immediately; both defaults are zero.
				if (low.Limit > 0f)
				{
					configurableJoint.highAngularXLimit = MakeJointLimit(high.Limit, high.Bounciness);
					configurableJoint.lowAngularXLimit = MakeJointLimit(low.Limit, low.Bounciness);
				}
				else
				{
					configurableJoint.lowAngularXLimit = MakeJointLimit(low.Limit, low.Bounciness);
					configurableJoint.highAngularXLimit = MakeJointLimit(high.Limit, high.Bounciness);
				}
				configurableJoint.angularYLimit = MakeJointLimit(Mathf.Abs(constraint.AngularYLimit.Limit), constraint.AngularYLimit.Bounciness);
				configurableJoint.angularZLimit = MakeJointLimit(Mathf.Abs(constraint.AngularZLimit.Limit), constraint.AngularZLimit.Bounciness);
				configurableJoint.angularXLimitSpring = MakeLimitSpring(low, high);
				configurableJoint.angularYZLimitSpring = MakeLimitSpring(constraint.AngularYLimit, constraint.AngularZLimit);
				configurableJoint.enableCollision = false;
				configurableJoint.enablePreprocessing = false;
				configurableJoint.projectionMode = JointProjectionMode.PositionAndRotation;
				configurableJoint.projectionDistance = 0.05f;
				// Angular projection fights articulated limits; retain linear anti-stretch projection only.
				configurableJoint.projectionAngle = 180f;
			}
		}
	}
	private static SoftJointLimit MakeJointLimit(float limit, float bounciness)
	{
		SoftJointLimit result = new SoftJointLimit();
		result.limit = limit;
		result.bounciness = bounciness;
		return result;
	}

	private static SoftJointLimitSpring MakeLimitSpring(HitBoxDescriptor.SoftJointLimitSpring first, HitBoxDescriptor.SoftJointLimitSpring second)
	{
		SoftJointLimitSpring result = new SoftJointLimitSpring();
		result.spring = Mathf.Max(first.Spring, second.Spring);
		// A damper-only spring softens limits in PhysX 3; legacy zero-spring limits are hard.
		result.damper = result.spring > 0f ? Mathf.Max(first.Damper, second.Damper) : 0f;
		return result;
	}

}
