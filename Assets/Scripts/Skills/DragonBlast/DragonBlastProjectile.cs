using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DragonBlastProjectile : ProjectileBehavior {
    public AnimationCurve movementCurve;

    private void FixedUpdate() {
        var timeElapsed = Time.time - createdAt;
        var direction = spawnRotation * Vector2.right;
        var newPosition = ProjectilePathUtils.StraightWithParameterizedMovement(spawnLocation, direction,
            movementCurve, timeElapsed, lifetime, distance);
        body.MovePosition(newPosition);
    }
}