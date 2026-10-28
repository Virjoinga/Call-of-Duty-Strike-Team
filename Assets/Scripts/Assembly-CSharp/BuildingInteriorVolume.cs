using System.Collections.Generic;
using UnityEngine;

public class BuildingInteriorVolume : MonoBehaviour
{
    public BuildingWithInterior ParentBuilding;

    public BuildingInteriorVolume PrimaryVolume;

    private readonly Dictionary<Actor, Dictionary<BuildingInteriorVolume, HashSet<Collider>>> mContacts =
        new Dictionary<Actor, Dictionary<BuildingInteriorVolume, HashSet<Collider>>>();

    private static Actor FindActor(Collider other)
    {
        Actor actor = other.GetComponent<Actor>();
        if (actor != null) return actor;
        HitLocation hit = other.GetComponent<HitLocation>();
        if (hit == null) return null;
        actor = hit.Actor;
        if (actor == null && hit.Owner != null) actor = hit.Owner.GetComponent<Actor>();
        return actor != null && actor.realCharacter != null && actor.realCharacter.SimpleHitBounds == hit ? actor : null;
    }

    private void OnTriggerEnter(Collider other)
    {
        (PrimaryVolume != null ? PrimaryVolume : this).Enter(this, other);
    }

    private void Enter(BuildingInteriorVolume source, Collider other)
    {
        Actor actor = FindActor(other);
        if (actor == null || ParentBuilding == null || actor.GetComponent<Collider>() == null) return;
        Dictionary<BuildingInteriorVolume, HashSet<Collider>> parts;
        bool firstContact = !mContacts.TryGetValue(actor, out parts);
        if (firstContact)
        {
            parts = new Dictionary<BuildingInteriorVolume, HashSet<Collider>>();
            mContacts.Add(actor, parts);
        }
        HashSet<Collider> contacts;
        if (!parts.TryGetValue(source, out contacts))
        {
            contacts = new HashSet<Collider>();
            parts.Add(source, contacts);
        }
        contacts.Add(other);
        if (firstContact) ParentBuilding.OnTriggerEnter(actor.GetComponent<Collider>());
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        (PrimaryVolume != null ? PrimaryVolume : this).Exit(this, other);
    }

    private void Exit(BuildingInteriorVolume source, Collider other)
    {
        Actor actor = FindActor(other);
        Dictionary<BuildingInteriorVolume, HashSet<Collider>> parts;
        HashSet<Collider> contacts;
        if (actor == null || !mContacts.TryGetValue(actor, out parts) ||
            !parts.TryGetValue(source, out contacts) || !contacts.Remove(other)) return;
        if (contacts.Count == 0) parts.Remove(source);
        if (parts.Count == 0) FinishExit(actor);
    }

    private void FinishExit(Actor actor)
    {
        mContacts.Remove(actor);
        if (actor == null || ParentBuilding == null) return;
        Collider root = actor.GetComponent<Collider>();
        if (root != null) ParentBuilding.OnTriggerExit(root);
    }

    private void ForgetPart(BuildingInteriorVolume source)
    {
        var exiting = new List<Actor>();
        foreach (var entry in mContacts)
        {
            entry.Value.Remove(source);
            if (entry.Value.Count == 0) exiting.Add(entry.Key);
        }
        foreach (Actor actor in exiting) FinishExit(actor);
    }

    private void OnDisable()
    {
        if (PrimaryVolume != null)
        {
            PrimaryVolume.ForgetPart(this);
            return;
        }
        foreach (Actor actor in new List<Actor>(mContacts.Keys)) FinishExit(actor);
    }
}
