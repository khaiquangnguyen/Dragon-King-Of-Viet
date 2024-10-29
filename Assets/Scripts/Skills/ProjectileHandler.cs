using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectileHandler : MonoBehaviour {
    private GameObject target;
    private Vector2 targetLocation;
    private float damage;
    private LayerMask collidableLayerMask;
    public GameObject selfDestructEffect;
    public GameObject impactEffect;
    [NonSerialized]
    private bool destroyOnHit = true;
    [NonSerialized]
    public Skill ParentSkill;
    [NonSerialized]
    public string UniqueId;
    [NonSerialized]
    public Character Caster;
    [NonSerialized]
    public Rigidbody2D projectileBody;
    [NonSerialized]
    public ProjectileSpecs specs;
    [NonSerialized]
    public Quaternion spawnRotation;
    [NonSerialized]
    public Vector3 spawnLocation;
    private float createdAt;

    private void Awake() {
        projectileBody = GetComponent<Rigidbody2D>();
    }

    public void InitWith(float newDamage, ProjectileSpecs projectileSpecs, LayerMask newCollidableLayerMask, bool newDestroyOnHit) {
        damage = newDamage;
        specs = projectileSpecs;
        collidableLayerMask = newCollidableLayerMask;
        destroyOnHit = newDestroyOnHit;
        spawnRotation = transform.rotation;
        spawnLocation = transform.position;
        createdAt = Time.time;
        Invoke(nameof(DestroyOnExpired), specs.lifetime);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        var damageTaker = other.gameObject.GetComponent<Character>();
        if (damageTaker != null) {
            OnHit(damageTaker);
            if (destroyOnHit) DestroyOnHit();
        }
    }

    private void FixedUpdate() {
        specs.OnFixedUpdate(this, Time.time - createdAt, Time.fixedDeltaTime);
    }

    private void DestroyOnExpired() {
        Instantiate(selfDestructEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void DestroyOnHit() {
        Instantiate(selfDestructEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    private void OnHit(Character character) {
        character.TakeDamage(damage, Caster);
        ParentSkill.OnDamageDealt(damage, character);
        Caster.OnDamageDealt(damage, character);
    }
}
