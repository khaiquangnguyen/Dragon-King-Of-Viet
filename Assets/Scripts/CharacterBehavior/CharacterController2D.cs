using System.ComponentModel;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterBehavior {
    public class CharacterController2D : MonoBehaviour {
        [HideInInspector]
        public float groundCheckDistance = 0.1f;
        [HideInInspector]
        public float stickToGroundDistance = 0.4f;
        [HideInInspector]
        public float slopeCheckDistance = 0.1f;
        [HideInInspector]
        public float maxVyStickToGroundCorrectionVelocity = 20f;
        [HideInInspector]
        public LayerMask groundLayer = LayerMask.GetMask("Ground");
        public CharacterControllerStats stats;
        public Rigidbody2D body;
        public Collider2D bodyCollider;
        public bool isOnWalkableSlope;
        public float slopeSideAngle;
        private float lastSlopeAngle;
        private float maxSlopeAngle;
        public float slopeDownAngle;
        public bool isWalkableGroundCheckPassed;
        public bool shouldStickToGround = true;
        public bool isOnSlope;
        public Vector2 velocity;

        public void OnEnable() {
            groundCheckDistance = stats.groundCheckDistance;
            stickToGroundDistance = stats.stickToGroundDistance;
            slopeCheckDistance = stats.slopeCheckDistance;
            maxVyStickToGroundCorrectionVelocity = stats.maxVyStickToGroundCorrectionVelocity;
            groundLayer = stats.groundLayer;
            maxSlopeAngle = stats.maxSlopeAngle;
        }

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
            var hit = Physics2D.Raycast(GetCastOrigin(), Vector2.down, stickToGroundDistance, groundLayer);
            if (hit) groundTangent = Vector2.Perpendicular(hit.normal).normalized;
            velocity = new Vector2(movementVelocity * -groundTangent.x,
                movementVelocity * -groundTangent.y);
            StickToGround();
            Move(velocity.x, velocity.y);
        }

        public void MoveOnAirWithGravityApplied(float accel, float decel, float maxSpeedX, float gravity,
            float gravityMult, int facingDirection) {
            var accelerationFactor = Mathf.Abs(velocity.x) > maxSpeedX
                ? accel
                : decel;
            var vX = Mathf.MoveTowards(Mathf.Abs(velocity.x), maxSpeedX,
                accelerationFactor * Time.fixedDeltaTime) * facingDirection;
            var vY = velocity.y;
            vY += gravity * gravityMult * Time.fixedDeltaTime;
            Move(vX, vY);
        }

        public void StickToGround() {
            // can stick to ground but not on ground yet
            var hit = Physics2D.Raycast(GetCastOrigin(), Vector2.down, stickToGroundDistance, groundLayer);
            if (!isWalkableGroundCheckPassed && !isOnWalkableSlope && hit && shouldStickToGround) {
                velocity.y = -hit.distance / Time.fixedDeltaTime;
                velocity.y = Mathf.Clamp(velocity.y, -maxVyStickToGroundCorrectionVelocity,
                    maxVyStickToGroundCorrectionVelocity);
            }
        }

        /*
         * Check if the character is on ground, including stick to ground ray cast hit
         */
        public bool isOnGround() {
            var stickToGroundHit = Physics2D.Raycast(GetCastOrigin(), Vector2.down, stickToGroundDistance, groundLayer);
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
            var groundedHit = Physics2D.Raycast(castOrigin, Vector2.down, groundCheckDistance, groundLayer);
            // var boxCastSize = new Vector2(bodyCollider.bounds.size.x * 0.8f, groundCheckDistance);
            // var groundedHit =
                // Physics2D.BoxCast(castOrigin, boxCastSize, 0, Vector2.down, groundCheckDistance, groundLayer);
            isWalkableGroundCheckPassed = groundedHit.collider is not null;
        }

        public void GetGroundNormal() { }

        private void CheckOnSlope() {
            var groundCheckPos = new Vector2(bodyCollider.bounds.center.x, bodyCollider.bounds.min.y);
            SlopeCheckHorizontal(groundCheckPos);
            SlopeCheckVertical(groundCheckPos);
        }

        private void SlopeCheckHorizontal(Vector2 checkPos) {
            var slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, groundLayer);
            var slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, groundLayer);
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
            var hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);
            if (hit) {
                slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (!Mathf.Approximately(slopeDownAngle, lastSlopeAngle)) isOnSlope = true;
                lastSlopeAngle = slopeDownAngle;
                isOnWalkableSlope = isOnSlope && slopeDownAngle <= maxSlopeAngle && slopeSideAngle <= maxSlopeAngle;
            }
            else {
                isOnWalkableSlope = false;
            }
        }
    }
}
