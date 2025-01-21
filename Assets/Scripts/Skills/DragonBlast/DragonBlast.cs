using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DragonBlast", menuName = "Skills/DragonBlast")]
public class DragonBlast : Skill {
    public GameObject projectilePrefab;
    public GameObject projectileImpactPrefab;
    public GameObject projectileSelfDestructPrefab;
    public AnimationCurve projectileMovementCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float distance;
    public float lifetime;

    public void OnEnable() {
        skillName = "Dragon Blast";
    }

    public override void Use() {
        var totalDamage = (int)(damage * Caster.damageMult);
        if (Camera.main == null) return;
        var spawnLocation = Caster.transform.position;
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var rotation = ComputeProjectileDirection(spawnLocation, mousePosition);
        var (projectile, projectileScript) = SpawnSkillCollidablePartAt<DragonBlastProjectile>(projectilePrefab, spawnLocation, rotation);
        projectileScript.selfDestructEffect = projectileSelfDestructPrefab;
        projectileScript.impactEffect = projectileImpactPrefab;
        projectileScript.movementCurve = projectileMovementCurve;
        projectileScript.distance = distance;
        projectileScript.lifetime = lifetime;
        projectileScript.damage = totalDamage;
        projectile.layer = Caster.projectileLayer;
        projectileScript.destroyOnHit = true;
        projectileScript.Init();
    }
}
