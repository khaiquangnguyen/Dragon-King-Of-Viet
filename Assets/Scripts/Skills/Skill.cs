using CharacterBehavior;
using Unity.Collections;
using UnityEngine;

public abstract class Skill : ScriptableObject {
    [ReadOnly]
    public string skillName;
    public GameObject icon;
    public float baseCooldown;
    public float damage;
    protected GameCharacter Caster;
    private float currentCooldown => baseCooldown / (1 + Caster.abilityHaste);
    private float cooldownTimerCountdown;
    public float skillStartupDuration;
    public float skillActiveDuration;
    public float skillRecoveryDuration;
    public AnimationClip skillStartupAnimation;
    public AnimationClip skillActiveAnimation;
    public AnimationClip skillRecoveryAnimation;

    public void Init(GameCharacter caster) {
        Caster = caster;
        cooldownTimerCountdown = 0;
    }

    protected GameObject SpawnProjectileAt<T>(GameObject projectilePrefab, Vector3 position, Quaternion rotation)
        where T : ProjectileBehavior {
        var projectile = Instantiate(projectilePrefab, position, rotation);
        var projectileBehavior = projectile.GetComponent<T>();
        if (!projectileBehavior) {
            projectileBehavior = projectile.AddComponent<T>();
        }

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
    public void OnDealDamage(float damageDealt, IDamageTaker damageTaker) { }
}