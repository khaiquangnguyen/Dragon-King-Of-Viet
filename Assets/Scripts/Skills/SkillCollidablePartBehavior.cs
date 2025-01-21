using System;
using System.Collections.Generic;
using CharacterBehavior;
using UnityEngine;

public class SkillCollidablePartBehavior : MonoBehaviour {
    public GameObject target;
    public Vector2 targetLocation;
    public int damage;
    [NonSerialized]
    public GameObject selfDestructEffect;
    [NonSerialized]
    public GameObject impactEffect;
    [HideInInspector]
    public bool destroyOnHit = true;
    [HideInInspector]
    public Skill ParentSkill;
    [HideInInspector]
    public string UniqueId;
    [HideInInspector]
    public GameCharacter Caster;
    [HideInInspector]
    public Rigidbody2D body;
    [HideInInspector]
    public Quaternion spawnRotation;
    [HideInInspector]
    public Vector3 spawnLocation;
    [HideInInspector]
    public bool shouldDestroyOnExpired = false;
    public float createdAt;
    public float distance;
    public float lifetime;
    public List<IDamageTaker> damagedCharacters = new();

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
    }

    public void Init() {
        spawnRotation = transform.rotation;
        spawnLocation = transform.position;
        createdAt = Time.time;
        if (shouldDestroyOnExpired && lifetime > 0) {
            Invoke(nameof(OnExpired), lifetime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var damageTaker = other.gameObject.GetComponent<GameCharacter>();
        if (damageTaker != null && damagedCharacters.Contains(damageTaker) == false) {
            OnHit(damageTaker);
            damagedCharacters.Add(damageTaker);
            if (destroyOnHit) DestroyOnHit();
        }
    }

    private void OnExpired() {
        Instantiate(selfDestructEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void DestroyOnHit() {
        Instantiate(selfDestructEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnHit(IDamageTaker damageTaker) {
        damageTaker.OnTakeDamage(damage, Caster);
        ParentSkill.OnDealDamage(damage, damageTaker);
        Caster.OnDealDamage(damage, damageTaker);
    }
}
