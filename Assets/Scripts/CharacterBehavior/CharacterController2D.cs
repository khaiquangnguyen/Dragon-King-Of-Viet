using Unity.Collections;
using UnityEngine;

namespace CharacterBehavior {
    public class CharacterController2D : MonoBehaviour {
        public CharacterControllerStats stats;
        public Rigidbody2D body;
        public Collider2D bodyCollider;
        [ReadOnly]
        public bool isOnWalkableSlope;
        public float slopeSideAngle;
        private float lastSlopeAngle;
        public float slopeDownAngle;
        public bool isWalkableGroundCheckPassed;
        public bool shouldStickToGround = true;
        public bool isOnSlope;
        public Vector2 velocity;

        public void OnEnable() { }

        public void MoveAlongGround(float acceleration, float deceleration, float maxSpeed, int facingDirection) {
            var accelerationFactor = velocity.magnitude > maxSpeed
                ? deceleration
                : acceleration;
            var maxCurrentVelocity = Mathf.MoveTowards(velocity.magnitude, maxSpeed,
                accelerationFactor * Time.fixedDeltaTime) * facingDirection;
            MoveAlongGround(maxCurrentVelocity);
        }

        public void MoveAlongGround(float movementVelocity) {
            var groundTangent = Vector2.one;
            var hit = Physics2D.Raycast(GetCastOrigin(), Vector2.down, stats.stickToGroundDistance, stats.groundLayer);
            if (hit) groundTangent = Vector2.Perpendicular(hit.normal).normalized;
            velocity = new Vector2(movementVelocity * -groundTangent.x,
                movementVelocity * -groundTangent.y);
            StickToGround();
            Move(velocity.x, velocity.y);
        }

        public void MoveOnAirWithGravityApplied(float accel, float decel, float maxSpeedX, float gravity,
            float gravityMult, int facingDirection, float maxFallSpeed = 10000) {
            var accelerationFactor = Mathf.Abs(velocity.x) > maxSpeedX
                ? accel
                : decel;
            var vX = Mathf.MoveTowards(Mathf.Abs(velocity.x), maxSpeedX,
                accelerationFactor * Time.fixedDeltaTime) * facingDirection;
            var vY = velocity.y;
            vY += gravity * gravityMult * Time.fixedDeltaTime;
            vY = Mathf.Clamp(vY, -maxFallSpeed, maxFallSpeed);
            Move(vX, vY);
        }

        public void StickToGround() {
            // can stick to ground but not on ground yet
            var hit = Physics2D.Raycast(GetCastOrigin(), Vector2.down, stats.stickToGroundDistance, stats.groundLayer);
            Debug.DrawRay(GetCastOrigin(), Vector2.down * stats.stickToGroundDistance, Color.red);
            if (!isWalkableGroundCheckPassed && !isOnWalkableSlope && hit && shouldStickToGround) {
                velocity.y = -hit.distance / Time.fixedDeltaTime;
                velocity.y = Mathf.Clamp(velocity.y, -stats.maxVyStickToGroundCorrectionVelocity,
                    stats.maxVyStickToGroundCorrectionVelocity);
            }
        }

        /*
         * Check if the character is on ground, including stick to ground ray cast hit
         */
        public bool isOnGround() {
            var stickToGroundHit = Physics2D.Raycast(GetCastOrigin(), Vector2.down, stats.stickToGroundDistance,
                stats.groundLayer);
            var isGrounded = isWalkableGroundCheckPassed || isOnWalkableSlope || stickToGroundHit;
            return isGrounded;
        }

        /*
         * Check if the character is on walkable ground, aka when the feet (or end of ground raycast in this case) actually touch the ground
         */
        public bool isOnWalkableGround() {
            return isWalkableGroundCheckPassed || isOnWalkableSlope;
        }

        public void Move(float newX, float newY) {
            velocity.Set(newX, newY);
            body.MovePosition(body.position + new Vector2(newX, newY) * Time.fixedDeltaTime);
        }

        public void MoveX(float newX) {
            velocity.Set(newX, velocity.y);
            Move(velocity.x, velocity.y);
        }

        public void MoveY(float newY) {
            velocity.Set(velocity.x, newY);
            Move(velocity.x, velocity.y);
        }

        public void MoveToX(float newX) {
            var newVelocityX = newX - body.position.x;
            velocity.Set(newVelocityX, velocity.y);
            body.MovePosition(new Vector2(newX, body.position.y));
        }

        public void FixedUpdate() {
            CheckOnSlope();
            CheckRaycastGround();
        }

        private Vector2 GetCastOrigin() {
            return new Vector2(bodyCollider.bounds.center.x, bodyCollider.bounds.min.y);
        }

        // Check if the character is on ground, aka when the feet (or end of raycast in this case) actually touch the ground
        public void CheckRaycastGround() {
            var castOrigin = GetCastOrigin();
            var groundedHit = Physics2D.Raycast(castOrigin, Vector2.down, stats.groundCheckDistance, stats.groundLayer);
            isWalkableGroundCheckPassed = groundedHit.collider is not null;
        }

        private void CheckOnSlope() {
            var groundCheckPos = new Vector2(bodyCollider.bounds.center.x, bodyCollider.bounds.min.y);
            SlopeCheckHorizontal(groundCheckPos);
            SlopeCheckVertical(groundCheckPos);
            isOnWalkableSlope = isOnSlope && slopeDownAngle <= stats.maxSlopeAngle &&
                                slopeSideAngle <= stats.maxSlopeAngle;
        }

        private void SlopeCheckHorizontal(Vector2 checkPos) {
            var slopeHitFront =
                Physics2D.Raycast(checkPos, transform.right, stats.slopeCheckDistance, stats.groundLayer);
            var slopeHitBack =
                Physics2D.Raycast(checkPos, -transform.right, stats.slopeCheckDistance, stats.groundLayer);
            // debug draw ray
            Debug.DrawRay(checkPos, transform.right * stats.slopeCheckDistance, Color.green);
            Debug.DrawRay(checkPos, -transform.right * stats.slopeCheckDistance, Color.green);
            if (slopeHitFront) {
                slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
                isOnSlope = true;
            }
            else if (slopeHitBack) {
                slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
                isOnSlope = true;
            }
            else {
                slopeSideAngle = 0.0f;
                isOnSlope = false;
            }
        }

        private void SlopeCheckVertical(Vector2 checkPos) {
            var hit = Physics2D.Raycast(checkPos, Vector2.down, stats.slopeCheckDistance, stats.groundLayer);
            Debug.DrawRay(checkPos, Vector2.down * stats.slopeCheckDistance, Color.green);
            if (hit) {
                slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (!Mathf.Approximately(slopeDownAngle, lastSlopeAngle)) isOnSlope = true;
                lastSlopeAngle = slopeDownAngle;
            }
        }
    }
}
