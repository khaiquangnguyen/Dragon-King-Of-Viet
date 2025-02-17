using UnityEngine;

public class DragonBlastProjectile : SkillCollidablePartBehavior {
    public AnimationCurve movementCurve;

    private void FixedUpdate() {
        var timeElapsed = Time.time - createdAt;
        var direction = spawnRotation * Vector2.right;
        var newPosition = ProjectilePathUtils.StraightWithParameterizedMovement(spawnLocation, direction,
            movementCurve, timeElapsed, lifetime, distance);
        body.MovePosition(newPosition);
    }
}
