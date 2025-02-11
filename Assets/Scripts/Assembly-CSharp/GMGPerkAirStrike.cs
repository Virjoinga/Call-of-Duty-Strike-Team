using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMGPerkAirStrike : MonoBehaviour
{
	public GameObject ProgressBlipRef;

	public bool showPoints = true;

	private float MinimumTimeBetweenExplosions = 0.1f;

	private float MaximumTimeBetweenExplosions = 0.5f;

	private int NumberOfImpactPoints = 10;

	private float ImpactPointSpread = 16f;

	private SurfaceImpact[] Impacts = new SurfaceImpact[0];

	private GameObject[] pointCube = new GameObject[16];

	private HackingBlip ProgressBlip;

	private float timer = 10f;

	private float ReqDelay;

	private GameObject debugCube;

	private GameObject locator;

	private Vector3 mousePos = Vector3.zero;

	private Vector3 hitPosition = Vector3.zero;

	private bool isActive;

	private void Start()
	{
		if (!(ProgressBlipRef != null))
		{
			return;
		}
		ProgressBlipRef = Object.Instantiate(ProgressBlipRef) as GameObject;
		if (ProgressBlipRef != null)
		{
			ProgressBlip = ProgressBlipRef.GetComponent<HackingBlip>();
			if (ProgressBlip != null)
			{
				ProgressBlip.Target = base.transform;
			}
		}
	}

	private void Awake()
	{
		debugCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		debugCube.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		Object.Destroy(debugCube.GetComponent<BoxCollider>());
		locator = Object.Instantiate(debugCube, hitPosition, base.transform.rotation) as GameObject;
		locator.SetActive(false);
	}

	private void Update()
	{
		if (!isActive)
		{
			return;
		}
		ReqDelay -= Time.fixedDeltaTime;
		ProgressBlip.SetProgress(ReqDelay / timer);
		if (Input.GetMouseButtonDown(0))
		{
			mousePos = Input.mousePosition;
			Ray ray = Camera.allCameras[0].ScreenPointToRay(mousePos);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity))
			{
				hitPosition = hitInfo.point;
				Debug.Log("*** Navmesh hit @: " + hitPosition);
			}
			base.transform.position = hitPosition;
			locator.transform.position = hitPosition;
		}
	}

	public void Activate()
	{
		isActive = true;
		locator.SetActive(true);
		ReqDelay = timer;
		ProgressBlip.ShowBlip();
	}

	public void Deactivate()
	{
		isActive = false;
		locator.SetActive(false);
		ProgressBlip.HideBlip();
	}

	public IEnumerator RunSequence()
	{
		Debug.Log("**RUN SEQUENCE, START EXPLOSIONS**");
		int i = 0;
		while (i < Impacts.Length)
		{
			SurfaceImpact explosionPoint = Impacts[i];
			float delay = Random.Range(MinimumTimeBetweenExplosions, MaximumTimeBetweenExplosions);
			float mortarLength = 0.5f;
			float mortarDelay = delay - mortarLength;
			StartCoroutine(DoWhistleSFX(mortarDelay));
			StartCoroutine(DoExplosion(delay, explosionPoint));
			Object.Destroy(pointCube[i]);
			i++;
			yield return new WaitForSeconds(delay);
		}
	}

	public void GenerateImpactPoints()
	{
		Debug.Log("**GENERATING IMPACT POINTS**");
		List<SurfaceImpact> list = new List<SurfaceImpact>();
		for (int i = 0; i < NumberOfImpactPoints; i++)
		{
			SurfaceImpact surfaceImpact = GenerateImpactPoint(new Vector3(base.transform.position.x, base.transform.position.y + 10f, base.transform.position.z), -base.transform.up + ImpactPointSpread * 0.05f * Random.insideUnitSphere);
			if (surfaceImpact != null)
			{
				if (showPoints)
				{
					pointCube[i] = Object.Instantiate(debugCube, surfaceImpact.position, base.transform.rotation) as GameObject;
				}
				list.Add(surfaceImpact);
			}
		}
		Impacts = list.ToArray();
		Debug.Log("**NUM OF POINTS: " + Impacts.Length + " **");
		StartCoroutine(RunSequence());
	}

	public SurfaceImpact GenerateImpactPoint(Vector3 origin, Vector3 direction)
	{
		float num = 1000f;
		SurfaceImpact surfaceImpact = ProjectileManager.Trace(origin, origin + num * direction, ProjectileManager.ProjectileMask);
		return (surfaceImpact.material != SurfaceMaterial.None) ? surfaceImpact : null;
	}

	public IEnumerator DoWhistleSFX(float delay)
	{
		float delayed = 0f;
		while (delayed < delay)
		{
			delayed += Time.deltaTime;
			yield return null;
		}
		WeaponSFX.Instance.MortarWhistle.Play(base.gameObject);
	}

	public IEnumerator DoExplosion(float delay, SurfaceImpact explosionPoint)
	{
		float delayed = 0f;
		while (delayed < delay)
		{
			delayed += Time.deltaTime;
			yield return null;
		}
		RulesSystem.DoAreaOfEffectDamage(explosionPoint.position, 6f, 1f, null, ExplosionManager.ExplosionType.Grenade, "Grenade", true);
	}
}
