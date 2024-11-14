using System;
using System.Collections.Generic;
using CharacterBehavior;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour {
    private GameObject target;
    private Vector2 targetLocation;
    private int damage;
    [NonSerialized]
    public GameObject selfDestructEffect;
    [NonSerialized]
    public GameObject impactEffect;
    [HideInInspector]
    private bool destroyOnHit = true;
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
    public float createdAt;
    public float distance;
    public float lifetime;
    public List<IDamageTaker> damagedCharacters = new();

    private void Awake() {
        body = GetComponent<Rigidbody2D>();
    }

    public void InitWith(int newDamage, int layer,
        bool newDestroyOnHit) {
        damage = newDamage;
        gameObject.layer = layer;
        destroyOnHit = newDestroyOnHit;
        spawnRotation = transform.rotation;
        spawnLocation = transform.position;
        createdAt = Time.time;
        Invoke(nameof(DestroyOnExpired), lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var damageTaker = other.gameObject.GetComponent<GameCharacter>();
        if (damageTaker != null && damagedCharacters.Contains(damageTaker) == false) {
            OnHit(damageTaker);
            damagedCharacters.Add(damageTaker);
            if (destroyOnHit) DestroyOnHit();
        }
    }

    private void DestroyOnExpired() {
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