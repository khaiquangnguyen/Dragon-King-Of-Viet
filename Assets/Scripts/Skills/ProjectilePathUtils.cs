using UnityEngine;

public static class ProjectilePathUtils {
    public static Vector2 StraightWithParameterizedMovement(Vector2 spawnLocation, Vector2 direction,
        AnimationCurve movementCurve, float timeElapsed, float lifetime, float distance) {
        return spawnLocation + direction * (movementCurve.Evaluate(timeElapsed / lifetime) *
                                            distance);
    }

    public static Vector2 StraightWithVelocityAndAccelerationMovement(Vector2 spawnLocation, Vector2 direction,
        float initialSpeed, float acceleration, float timeElapsed) {
        var newLocation = spawnLocation +
                          direction * (initialSpeed * timeElapsed + 0.5f * acceleration * timeElapsed * timeElapsed);
        return newLocation;
    }

    public static void FollowAfterMovement() { }
}
