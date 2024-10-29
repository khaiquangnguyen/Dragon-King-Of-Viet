using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Skill : ScriptableObject {
    public string skillName;
    public GameObject icon;
    public float baseCooldown;
    public float damage;
    protected Character Caster;
    private float currentCooldown => baseCooldown / (1 + Caster.abilityHaste);
    private float cooldownTimerCountdown;

    public void Init(Character caster) {
        Caster = caster;
        cooldownTimerCountdown = 0;
    }

    protected GameObject SpawnProjectileAt(GameObject projectilePrefab, Vector3 position, Quaternion rotation) {
        var projectile = Instantiate(projectilePrefab, position, rotation);
        var projectileBehavior = projectilePrefab.GetComponent<ProjectileHandler>();
        projectileBehavior.ParentSkill = this;
        projectileBehavior.Caster = Caster;
        projectileBehavior.UniqueId = Caster.uniqueId + "-" + skillName + "-" + Time.time;
        return projectile;
    }

    protected Quaternion ComputeProjectileDirection(Vector3 spawnLocation, Vector3 targetLocation) {
        var direction = targetLocation - spawnLocation;
        var rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, rotationZ);
    }

    public abstract void Use();
    public void OnDamageDealt(float damageDealt, Character character) { }
}
