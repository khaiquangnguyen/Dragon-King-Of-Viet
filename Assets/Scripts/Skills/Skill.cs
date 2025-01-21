using System;
using CharacterBehavior;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Skill : ScriptableObject {
    [ReadOnly]
    public string skillName;
    public Sprite icon;
    public float baseCooldown;
    public float damage;
    protected GameCharacter Caster;
    public int skillLevel;
    private float currentCooldown => baseCooldown / (1 + Caster.abilityHaste);
    public float skillStartupDuration;
    public float skillActiveDuration;
    public float skillRecoveryDuration;
    public AnimationClip skillStartupAnimation;
    public AnimationClip skillActiveAnimation;
    public AnimationClip skillRecoveryAnimation;

    public void Init(GameCharacter caster) {
        Caster = caster;
    }

    protected Tuple<GameObject, T> SpawnSkillCollidablePartAt<T>(GameObject projectilePrefab, Vector3 position, Quaternion rotation)
        where T : SkillCollidablePartBehavior {
        var projectile = Instantiate(projectilePrefab, position, rotation);
        var projectileBehavior = projectile.GetComponent<T>();
        if (!projectileBehavior) {
            projectileBehavior = projectile.AddComponent<T>();
        }
        projectileBehavior.ParentSkill = this;
        projectileBehavior.Caster = Caster;
        projectileBehavior.UniqueId = Caster.uniqueId + "-" + skillName + "-" + Time.time;
        return Tuple.Create(projectile, projectileBehavior);
    }

    protected int ComputeDamage() {
        return (int)(damage * Caster.damageMult);
    }

    protected Quaternion ComputeProjectileDirection(Vector3 spawnLocation, Vector3 targetLocation) {
        var direction = targetLocation - spawnLocation;
        var rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0f, 0f, rotationZ);
    }

    public abstract void Use();
    public void OnDealDamage(float damageDealt, IDamageTaker damageTaker) { }
}
