using System;
using System.Collections.Generic;
using UnityEngine;

public class EventsCreator : BaseCreator
{
	public static Type[] Types = new Type[19]
	{
		typeof(EventOnDead),
		typeof(EventOnMortallyWounded),
		typeof(EventOnCarried),
		typeof(EventOnBecomeVisble),
		typeof(EventOnTakenDamage),
		typeof(EventOnEnterCover),
		typeof(EventOnHacked),
		typeof(EventOnMoveChanged),
		typeof(EventOnSelected),
		typeof(EventOnInRange),
		typeof(EventOnLowHealth),
		typeof(EventOnLowAmmo),
		typeof(EventOnChangeWeapon),
		typeof(EventOnAlerted),
		typeof(EventOnIveBeenDropped),
		typeof(EventOnIveBeenStealthKilled),
		typeof(EventOnHidden),
		typeof(EventOnMove),
		typeof(EventOnReact)
	};

	public EventDescriptor m_EditableDesc;

	public List<EventDescriptor> m_EventDescriptors = new List<EventDescriptor>();

	public override Type[] GetTypes()
	{
		return Types;
	}

	public override List<EventDescriptor> GetEvents()
	{
		if (m_EventDescriptors != null)
		{
			return m_EventDescriptors;
		}
		return null;
	}

	public override void PopulateItems()
	{
		if (m_EventDescriptors != null && m_EventDescriptors.Count > 0)
		{
			m_EditableDesc = m_EventDescriptors[0];
		}
	}

	public override void AddItem(int descriptorTypeIndex)
	{
		EventDescriptor item = base.gameObject.AddComponent(Types[descriptorTypeIndex]) as EventDescriptor;
		if (GetCurrentDescriptor() == null)
		{
			m_EventDescriptors.Add(item);
			SetCurrentItem(m_EventDescriptors.Count - 1);
		}
		else if (m_HighlightedTaskIndex == -1)
		{
			m_EventDescriptors.Add(item);
			SetCurrentItem(m_EventDescriptors.Count - 1);
		}
		else
		{
			m_EventDescriptors.Insert(m_HighlightedTaskIndex + 1, item);
			SetCurrentItem(m_HighlightedTaskIndex + 1);
		}
	}

	public void AddItemCopy(EventDescriptor eventDescriptor)
	{
		if (!(eventDescriptor == null))
		{
			EventDescriptor eventDescriptor2 = null;
			if (eventDescriptor as EventOnReact != null)
			{
				eventDescriptor2 = base.gameObject.AddComponent<EventOnReact>();
			}
			else if (eventDescriptor as EventOnAlerted != null)
			{
				eventDescriptor2 = base.gameObject.AddComponent<EventOnAlerted>();
			}
			else if (eventDescriptor as EventOnBecomeVisble != null)
			{
				EventOnBecomeVisble eventOnBecomeVisble = base.gameObject.AddComponent<EventOnBecomeVisble>();
				EventOnBecomeVisble eventOnBecomeVisble2 = eventDescriptor as EventOnBecomeVisble;
				eventOnBecomeVisble.DistanceCheck = eventOnBecomeVisble2.DistanceCheck;
				eventDescriptor2 = eventOnBecomeVisble;
			}
			else if (eventDescriptor as EventOnCameraActions != null)
			{
				EventOnCameraActions eventOnCameraActions = base.gameObject.AddComponent<EventOnCameraActions>();
				EventOnCameraActions eventOnCameraActions2 = eventDescriptor as EventOnCameraActions;
				eventOnCameraActions.CameraEvent = eventOnCameraActions2.CameraEvent;
				eventDescriptor2 = eventOnCameraActions;
			}
			else if (eventDescriptor as EventOnCarried != null)
			{
				eventDescriptor2 = base.gameObject.AddComponent<EventOnCarried>();
			}
			else if (eventDescriptor as EventOnChangeWeapon != null)
			{
				EventOnChangeWeapon eventOnChangeWeapon = base.gameObject.AddComponent<EventOnChangeWeapon>();
				EventOnChangeWeapon eventOnChangeWeapon2 = eventDescriptor as EventOnChangeWeapon;
				eventOnChangeWeapon.CheckSecondaryWeapon = eventOnChangeWeapon2.CheckSecondaryWeapon;
				eventDescriptor2 = eventOnChangeWeapon;
			}
			else if (eventDescriptor as EventOnDead != null)
			{
				eventDescriptor2 = base.gameObject.AddComponent<EventOnDead>();
			}
			else if (eventDescriptor as EventOnEnterCover != null)
			{
				eventDescriptor2 = base.gameObject.AddComponent<EventOnEnterCover>();
			}
			else if (eventDescriptor as EventOnHacked != null)
			{
				eventDescriptor2 = base.gameObject.AddComponent<EventOnHacked>();
			}
			else if (eventDescriptor as EventOnHudActions != null)
			{
				EventOnHudActions eventOnHudActions = base.gameObject.AddComponent<EventOnHudActions>();
				EventOnHudActions eventOnHudActions2 = eventDescriptor as EventOnHudActions;
				eventOnHudActions.HudEvent = eventOnHudActions2.HudEvent;
				eventDescriptor2 = eventOnHudActions;
			}
			else if (eventDescriptor as EventOnInRange != null)
			{
				EventOnInRange eventOnInRange = base.gameObject.AddComponent<EventOnInRange>();
				EventOnInRange eventOnInRange2 = eventDescriptor as EventOnInRange;
				eventOnInRange.RangedObject = eventOnInRange2.RangedObject;
				eventOnInRange.DistanceToCheck = eventOnInRange2.DistanceToCheck;
				eventDescriptor2 = eventOnInRange;
			}
			else if (eventDescriptor as EventOnLowAmmo != null)
			{
				EventOnLowAmmo eventOnLowAmmo = base.gameObject.AddComponent<EventOnLowAmmo>();
				EventOnLowAmmo eventOnLowAmmo2 = eventDescriptor as EventOnLowAmmo;
				eventOnLowAmmo.ZeroAmmoTest = eventOnLowAmmo2.ZeroAmmoTest;
				eventOnLowAmmo.AmmoCheck = eventOnLowAmmo2.AmmoCheck;
				eventDescriptor2 = eventOnLowAmmo;
			}
			else if (eventDescriptor as EventOnLowHealth != null)
			{
				EventOnLowHealth eventOnLowHealth = base.gameObject.AddComponent<EventOnLowHealth>();
				EventOnLowHealth eventOnLowHealth2 = eventDescriptor as EventOnLowHealth;
				eventOnLowHealth.LowHealthPercentage = eventOnLowHealth2.LowHealthPercentage;
				eventDescriptor2 = eventOnLowHealth;
			}
			else if (eventDescriptor as EventOnMortallyWounded != null)
			{
				eventDescriptor2 = base.gameObject.AddComponent<EventOnMortallyWounded>();
			}
			else if (eventDescriptor as EventOnMoveChanged != null)
			{
				EventOnMoveChanged eventOnMoveChanged = base.gameObject.AddComponent<EventOnMoveChanged>();
				EventOnMoveChanged eventOnMoveChanged2 = eventDescriptor as EventOnMoveChanged;
				eventOnMoveChanged.MovementTypeToTest = eventOnMoveChanged2.MovementTypeToTest;
				eventDescriptor2 = eventOnMoveChanged;
			}
			else if (eventDescriptor as EventOnSelected != null)
			{
				eventDescriptor2 = base.gameObject.AddComponent<EventOnSelected>();
			}
			else if (eventDescriptor as EventOnTakenDamage != null)
			{
				EventOnTakenDamage eventOnTakenDamage = base.gameObject.AddComponent<EventOnTakenDamage>();
				EventOnTakenDamage eventOnTakenDamage2 = eventDescriptor as EventOnTakenDamage;
				eventOnTakenDamage.MinimumDamageToFire = eventOnTakenDamage2.MinimumDamageToFire;
				eventDescriptor2 = eventOnTakenDamage;
			}
			else if (eventDescriptor as EventOnIveBeenDropped != null)
			{
				eventDescriptor2 = base.gameObject.AddComponent<EventOnIveBeenDropped>();
			}
			else if (eventDescriptor as EventOnIveBeenStealthKilled != null)
			{
				eventDescriptor2 = base.gameObject.AddComponent<EventOnIveBeenStealthKilled>();
			}
			else if (eventDescriptor as EventOnHidden != null)
			{
				eventDescriptor2 = base.gameObject.AddComponent<EventOnHidden>();
			}
			else if (eventDescriptor as EventOnMove != null)
			{
				EventOnMove eventOnMove = base.gameObject.AddComponent<EventOnMove>();
				EventOnMove eventOnMove2 = eventDescriptor as EventOnMove;
				eventOnMove.MovementType = eventOnMove2.MovementType;
				eventDescriptor2 = eventOnMove;
			}
			else
			{
				Debug.LogError(string.Concat(eventDescriptor, " is an invalid event type"));
			}
			if (eventDescriptor2 != null)
			{
				eventDescriptor2.ObjectsToCall = eventDescriptor.ObjectsToCall;
				eventDescriptor2.FunctionsToCall = eventDescriptor.FunctionsToCall;
				eventDescriptor2.ObjectParam = eventDescriptor.ObjectParam;
				eventDescriptor2.FireOnlyOnce = eventDescriptor.FireOnlyOnce;
				eventDescriptor2.BaseObject = eventDescriptor.BaseObject;
				m_EventDescriptors.Add(eventDescriptor2);
			}
		}
	}

	public override void RemoveItem(EventDescriptor eventToRemove)
	{
		m_EventDescriptors.Remove(eventToRemove);
		UnityEngine.Object.DestroyImmediate(eventToRemove);
		m_EditableDesc = null;
	}

	public override void SetCurrentItem(int currentDescriptorIndex)
	{
		m_EditableDesc = m_EventDescriptors[currentDescriptorIndex];
		m_HighlightedTaskIndex = currentDescriptorIndex;
	}

	public override EventDescriptor GetCurrentEvent()
	{
		return m_EditableDesc;
	}
}
