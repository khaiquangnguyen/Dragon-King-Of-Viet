using System;
using UnityEngine;

namespace WaterLaser {
    [CreateAssetMenu(fileName = "WaterLaser", menuName = "Skills/WaterLaser", order = 0)]

    public class WaterLaser: Skill {
        public GameObject waterLaserBurstPrefab;
        public GameObject waterLaserCastPrefab;
        public GameObject waterLaserCorePrefab;
        public GameObject waterLaserImpactPrefab;
        public GameObject waterLaserDissolvePrefab;
        public float duration;
        public float distance;

        public override void Use() {
            var damage = ComputeDamage();
            if (Camera.main == null) return;
            var spawnLocation = Caster.transform.position;
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var rotation = ComputeProjectileDirection(spawnLocation, mousePosition);
            Instantiate(waterLaserCastPrefab, spawnLocation, rotation);
            Instantiate(waterLaserBurstPrefab, spawnLocation, rotation);
            var (waterLaserRay, waterLaserRayScript) = SpawnSkillCollidablePartAt<WaterLaserRay>(waterLaserCorePrefab, spawnLocation, rotation);
            waterLaserRayScript.selfDestructEffect = waterLaserDissolvePrefab;
            waterLaserRayScript.impactEffect = waterLaserImpactPrefab;
            waterLaserRay.transform.localScale = new Vector3(1,1,distance);
            waterLaserRayScript.destroyOnHit = false;
            waterLaserRayScript.Init();
        }
    }
}
