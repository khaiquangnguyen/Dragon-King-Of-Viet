using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "DragonBlast", menuName = "Skills/DragonBlast", order = 0)]
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
        var spawnedProjectile = SpawnProjectileAt<DragonBlastProjectile>(projectilePrefab, spawnLocation, rotation);
        DragonBlastProjectile projectileScript = spawnedProjectile.GetComponent<DragonBlastProjectile>();
        if (!projectileScript) {
            projectileScript = spawnedProjectile.AddComponent<DragonBlastProjectile>();
        }

        projectileScript.selfDestructEffect = projectileSelfDestructPrefab;
        projectileScript.impactEffect = projectileImpactPrefab;
        projectileScript.movementCurve = projectileMovementCurve;
        projectileScript.distance = distance;
        projectileScript.lifetime = lifetime;
        projectileScript.InitWith(totalDamage, Caster.projectileLayer, true);
    }
}