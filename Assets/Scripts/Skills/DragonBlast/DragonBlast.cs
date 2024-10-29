using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "DragonBlast", menuName = "Skills/DragonBlast", order = 0)]
public class DragonBlast : Skill {
    public GameObject projectilePrefab;
    public LayerMask projectileCollidableLayerMask = new LayerMask();
    public ProjectileSpecs projectileSpecs;

    public override void Use() {
        var totalDamage = damage * Caster.damageMult;
        if (Camera.main == null) return;
        var spawnLocation = Caster.transform.position;
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var rotation = ComputeProjectileDirection(spawnLocation, mousePosition);
        var spawnedProjectile = SpawnProjectileAt(projectilePrefab, spawnLocation, rotation);
        var spawnedProjectileBehavior = spawnedProjectile.GetComponent<ProjectileHandler>();
        spawnedProjectileBehavior.InitWith(totalDamage, projectileSpecs, projectileCollidableLayerMask,true);
    }
}
