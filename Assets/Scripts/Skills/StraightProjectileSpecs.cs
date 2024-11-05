using System;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = System.Numerics.Vector3;

[CreateAssetMenu(fileName = "StraightMovement", menuName = "MovementPaths/StraightMovement", order = 0)]
public class StraightProjectileSpecs : ProjectileSpecs {
    public AnimationCurve movementCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public override void OnFixedUpdate(ProjectileHandler handler, float t, float fixedDeltaTime) {
        var spawnRotation = handler.spawnRotation;
        var spawnLocation = handler.spawnLocation;
        var direction = spawnRotation * Vector2.right;
        handler.projectileBody.MovePosition(spawnLocation +
                                            direction * (movementCurve.Evaluate(t / lifetime) *
                                                         distance));
    }
}