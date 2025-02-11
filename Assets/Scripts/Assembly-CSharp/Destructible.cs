using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Destructible : MonoBehaviour
{
	public enum Toughness
	{
		None = 0,
		Wood = 1,
		Metal = 2,
		Concrete = 3
	}

	public DestructibleData m_Interface;

	public Toughness m_Toughness;

	public float m_Health;

	public float m_SelfHarmValue = 1f;

	public float m_ExplosionRadius;

	public ExplosionManager.ExplosionType m_ExplosionType;

	public GameObject m_ExplosionPosition;

	public bool m_CanTakeBulletDamage = true;

	public bool m_CanTakeExplosionDamage = true;

	public GameObject m_UndamagedObject;

	public GameObject m_DamagedObject;

	public List<DestructibleFX> m_ParticleEffectsWhilstAlive = new List<DestructibleFX>();

	public List<DestructibleFX> m_ParticleEffectsWhilstDead = new List<DestructibleFX>();

	[HideInInspector]
	public List<DestructibleSFX> m_SoundEffectsWhilstAlive = new List<DestructibleSFX>();

	[HideInInspector]
	public List<DestructibleSFX> m_SoundEffectsWhilstDead = new List<DestructibleSFX>();

	public float ModelSwapDelay;

	private float m_PreviousHealth;

	private Actor mOwner;

	private bool m_Alive;

	private float m_ToughnessValue;

	private float m_Delay;

	private bool m_SelfHarm;

	private RSProjectileAttackDamageInterface ProjectileAttackDamageInterface = new RSProjectileAttackDamageInterface();

	public Actor Owner
	{
		get
		{
			return mOwner;
		}
	}

	private void Awake()
	{
		m_Alive = true;
		if (m_DamagedObject != null)
		{
			m_DamagedObject.SetActive(false);
		}
		switch (m_Toughness)
		{
		case Toughness.Wood:
			m_ToughnessValue = 0.8f;
			break;
		case Toughness.Metal:
			m_ToughnessValue = 0.5f;
			break;
		case Toughness.Concrete:
			m_ToughnessValue = 0.1f;
			break;
		default:
			m_ToughnessValue = 1f;
			break;
		}
		ProjectileAttackDamageInterface.Initialise(true, false, false, 1f);
		if (m_SoundEffectsWhilstAlive != null)
		{
			foreach (DestructibleSFX item in m_SoundEffectsWhilstAlive)
			{
				if (item != null)
				{
					if (item.m_HealthToTrigger >= m_Health)
					{
						SoundFXData soundData = GetSoundData(item);
						if (soundData != null)
						{
							SoundManager.Instance.Play(soundData, base.gameObject);
						}
						item.Actived = true;
					}
				}
				else
				{
					TBFAssert.DoAssert(item != null, "Sound effect missing from destructible prop!");
				}
			}
		}
		m_PreviousHealth = m_Health;
	}

	private void Start()
	{
		RulesSystem.OnExplosion += CheckForExplosionDamage;
	}

	private void OnDestroy()
	{
		RulesSystem.OnExplosion -= CheckForExplosionDamage;
	}

	private void ProcessDamage()
	{
		if (!m_Alive || m_Health == m_PreviousHealth)
		{
			return;
		}
		if (m_ParticleEffectsWhilstAlive != null)
		{
			foreach (DestructibleFX item in m_ParticleEffectsWhilstAlive)
			{
				if (item != null)
				{
					if (m_Health <= item.m_HealthToTrigger && !item.Active)
					{
						Vector3 position = base.gameObject.transform.position;
						if (item.m_PositionTransform != null)
						{
							position = item.m_PositionTransform.position;
						}
						item.Instance = Object.Instantiate(item.m_ParticleEffect) as GameObject;
						item.Instance.transform.position = position;
						item.Active = true;
						if (item.m_TriggerSelfHarm && !m_SelfHarm)
						{
							m_SelfHarm = true;
							StartCoroutine(SelfHarm());
						}
					}
				}
				else
				{
					TBFAssert.DoAssert(item != null, "Particle effect missing from destructible prop!");
				}
			}
		}
		if (m_SoundEffectsWhilstAlive != null)
		{
			foreach (DestructibleSFX item2 in m_SoundEffectsWhilstAlive)
			{
				if (item2 != null)
				{
					if (!(m_Health <= item2.m_HealthToTrigger) || item2.Expired || item2.Actived)
					{
						continue;
					}
					SoundFXData soundData = GetSoundData(item2);
					if (soundData != null)
					{
						SoundManager.Instance.Play(soundData, base.gameObject);
					}
					item2.Actived = true;
					foreach (DestructibleSFX item3 in m_SoundEffectsWhilstAlive)
					{
						if (item3 != item2 && !item3.Expired && item3.Actived)
						{
							item3.Expired = true;
						}
					}
				}
				else
				{
					TBFAssert.DoAssert(item2 != null, "Sound effect missing from destructible prop!");
				}
			}
			foreach (DestructibleSFX item4 in m_SoundEffectsWhilstAlive)
			{
				if (item4 != null)
				{
					if (item4.Expired && item4.Actived)
					{
						item4.Actived = false;
						SoundFXData soundData2 = GetSoundData(item4);
						if (soundData2 != null)
						{
							SoundManager.Instance.Stop(soundData2, base.gameObject);
						}
					}
				}
				else
				{
					TBFAssert.DoAssert(item4 != null, "Sound effect missing from destructible prop!");
				}
			}
		}
		if (m_Health <= 0f && m_PreviousHealth > 0f && m_Alive)
		{
			m_Alive = false;
			StartCoroutine(DoExplosion());
			if (m_Interface.ObjectToCallOnDestruction != null)
			{
				Container.SendMessage(m_Interface.ObjectToCallOnDestruction, m_Interface.FunctionToCallOnDestruction, base.gameObject);
			}
			ProcessDeath();
		}
		m_PreviousHealth = m_Health;
	}

	private void ProcessDeath()
	{
		if (m_ParticleEffectsWhilstDead == null)
		{
			return;
		}
		foreach (DestructibleFX item in m_ParticleEffectsWhilstDead)
		{
			if (item != null)
			{
				if (!item.Active)
				{
					Vector3 position = base.gameObject.transform.position;
					if (item.m_PositionTransform != null)
					{
						position = item.m_PositionTransform.position;
					}
					item.Instance = Object.Instantiate(item.m_ParticleEffect) as GameObject;
					item.Instance.transform.position = position;
					item.Active = true;
				}
			}
			else
			{
				TBFAssert.DoAssert(item != null, "Particle effect missing from destructible prop!");
			}
		}
	}

	private IEnumerator SelfHarm()
	{
		while (this != null && m_Alive)
		{
			m_Health -= m_SelfHarmValue * Time.deltaTime;
			ProcessDamage();
			yield return null;
		}
	}

	private SoundFXData GetSoundData(DestructibleSFX sfx)
	{
		SoundFXData result = null;
		if (sfx != null && sfx.SoundBank != null && sfx.PlayFunction != null)
		{
			PropertyInfo property = sfx.SoundBank.GetType().GetProperty("Instance");
			if (property != null)
			{
				SFXBank sFXBank = property.GetValue(null, null) as SFXBank;
				if (sFXBank != null)
				{
					result = sFXBank.GetSFXDataFromName(sfx.PlayFunction);
				}
			}
		}
		return result;
	}

	public void Activate()
	{
		m_Health = 0f;
		ProcessDamage();
	}

	public void ToggleInvincible()
	{
		m_Interface.Invincible = !m_Interface.Invincible;
	}

	private IEnumerator DoExplosion()
	{
		float delayed = 0f;
		while (delayed < m_Delay)
		{
			delayed += Time.deltaTime;
			yield return null;
		}
		foreach (DestructibleFX fx in m_ParticleEffectsWhilstAlive)
		{
			if (fx != null)
			{
				if (fx.Active && fx.Instance != null)
				{
					Object.DestroyImmediate(fx.Instance);
					fx.Active = false;
				}
			}
			else
			{
				TBFAssert.DoAssert(fx != null, "Particle effect missing from destructible prop!");
			}
		}
		foreach (DestructibleSFX sfx in m_SoundEffectsWhilstAlive)
		{
			if (sfx != null)
			{
				if (sfx.Actived)
				{
					sfx.Actived = false;
					sfx.Expired = true;
					SoundFXData sfxData = GetSoundData(sfx);
					if (sfxData != null)
					{
						SoundManager.Instance.Stop(sfxData, base.gameObject);
					}
				}
			}
			else
			{
				TBFAssert.DoAssert(sfx != null, "Sound effect missing from destructible prop!");
			}
		}
		if (m_SoundEffectsWhilstDead != null)
		{
			foreach (DestructibleSFX sfx2 in m_SoundEffectsWhilstDead)
			{
				if (sfx2 != null && !sfx2.Actived)
				{
					if (sfx2.m_HealthToTrigger >= m_Health)
					{
						SoundFXData sfxData2 = GetSoundData(sfx2);
						if (sfxData2 != null)
						{
							SoundManager.Instance.Play(sfxData2, base.gameObject);
						}
						sfx2.Actived = true;
					}
				}
				else
				{
					TBFAssert.DoAssert(sfx2 != null, "Sound effect missing from destructible prop!");
				}
			}
		}
		Vector3 explosionOrigin = base.transform.position;
		if (m_UndamagedObject != null)
		{
			MeshRenderer renderer = m_UndamagedObject.GetComponentInChildren<MeshRenderer>();
			if (renderer != null)
			{
				explosionOrigin.y += renderer.bounds.size.y / 2f;
			}
		}
		StartCoroutine(SwapObjects());
		GameObject damageDealer = null;
		if ((bool)mOwner)
		{
			damageDealer = mOwner.gameObject;
		}
		Vector3 explosionPosition = ((!(m_ExplosionPosition != null)) ? explosionOrigin : m_ExplosionPosition.transform.position);
		bool regularDifficulty = !(ActStructure.Instance != null) || ActStructure.Instance.CurrentMissionMode != DifficultyMode.Veteran;
		if (m_Interface.DontDoAOEDamage)
		{
			ExplosionManager.Instance.StartExplosion(explosionPosition, m_ExplosionRadius, m_ExplosionType);
		}
		else
		{
			RulesSystem.DoAreaOfEffectDamage(explosionPosition, m_ExplosionRadius, 1f, damageDealer, m_ExplosionType, "Explosion", regularDifficulty);
		}
		yield return null;
	}

	private IEnumerator SwapObjects()
	{
		if (m_UndamagedObject != null)
		{
			m_UndamagedObject.SetActive(false);
			Object.DestroyImmediate(m_UndamagedObject);
			BoxCollider collider = GetComponent<BoxCollider>();
			if (collider != null)
			{
				collider.enabled = false;
			}
		}
		yield return new WaitForSeconds(ModelSwapDelay);
		if (m_DamagedObject != null)
		{
			m_DamagedObject.SetActive(true);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		DecalManager.Instance.RemoveBulletHolesInRadius(base.gameObject.transform.position, m_ExplosionRadius * 3f);
	}

	private void TakeDamage(float damageToTake, Actor damageDealer)
	{
		if (m_Alive && !m_Interface.Invincible && base.gameObject.activeInHierarchy && (m_Interface.DamagedBy == FactionHelper.Category.Count || !(damageDealer != null) || m_Interface.DamagedBy == damageDealer.awareness.faction))
		{
			mOwner = damageDealer;
			m_Health -= damageToTake * m_ToughnessValue;
			ProcessDamage();
		}
	}

	public void DoBulletDamage(float damageToTake, GameObject damageDealer)
	{
		if (m_CanTakeBulletDamage)
		{
			Actor damageDealer2 = null;
			if (damageDealer != null)
			{
				damageDealer2 = damageDealer.GetComponent<Actor>();
			}
			TakeDamage(damageToTake, damageDealer2);
		}
	}

	private void CheckForExplosionDamage(Vector3 origin, float radius, GameObject damageDealer, ExplosionManager.ExplosionType explosionType, string damageType)
	{
		if (!m_CanTakeExplosionDamage || !m_Alive)
		{
			return;
		}
		float num = radius * radius;
		float sqrMagnitude = (origin - base.gameObject.transform.position).sqrMagnitude;
		if (sqrMagnitude < num)
		{
			m_Delay = sqrMagnitude / num;
			float damageToTake = GetActualHealthFromDelta(RulesSystemSettings.ProjectileDamageExplosion) / m_ToughnessValue;
			Actor damageDealer2 = null;
			if ((bool)damageDealer)
			{
				damageDealer2 = damageDealer.GetComponent<Actor>();
			}
			TakeDamage(damageToTake, damageDealer2);
		}
	}

	private float GetActualHealthFromDelta(float delta)
	{
		return m_Health * delta;
	}
}
