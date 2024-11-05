using UnityEngine;

namespace CharacterBehavior {
    public class CharacterBaseMovement : MonoBehaviour {
        public CharacterController2D characterController;
        public Environment environment;

        public void RunOnGround(float acceleration, float deceleration, float maxSpeed, int facingDirection) {
            environment = Environment.Ground;
            var accelerationFactor = characterController.velocity.magnitude > maxSpeed
                ? deceleration
                : acceleration;
            var maxCurrentVelocity = Mathf.MoveTowards(characterController.velocity.magnitude, maxSpeed,
                accelerationFactor) * facingDirection;
            characterController.MoveAlongGround(maxCurrentVelocity);
        }

        public void Fall(float vX, float gravity, float gravityMult) {
            environment = Environment.Air;
            var velocityY = characterController.velocity.y;
            velocityY += gravity * gravityMult * Time.fixedDeltaTime;
            characterController.MoveOnNonGround(vX, velocityY);
            if (characterController.isOnWalkableGround()) environment = Environment.Ground;
        }

        public void Jump(float vX, float vY) {
            environment = Environment.Air;
            characterController.MoveOnNonGround(vX, vY);
        }
    }
}
