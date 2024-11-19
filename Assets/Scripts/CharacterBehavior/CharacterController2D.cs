using Unity.Collections;
using UnityEngine;

public struct WallCollideStatuses {
    public bool BodyOnWall;
    public bool BodyOnHang;
}

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
        [ReadOnly]
        public bool isOnGround;
        public Vector2 velocity;

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
            if (hit) {
                groundTangent = Vector2.Perpendicular(hit.normal).normalized;
                velocity = new Vector2(movementVelocity * -groundTangent.x,
                movementVelocity * -groundTangent.y);
            }
            StickToGround();
            Move(velocity.x, velocity.y);
        }

        public void MoveOnNonGroundHorizontal(float accel, float decel, float maxSpeedX, float gravity,
            float gravityMult, int facingDirection, float maxFallSpeed = 10000) {
            var accelerationFactor = Mathf.Abs(velocity.x) > maxSpeedX
                ? accel
                : decel;
            var vX = Mathf.MoveTowards(Mathf.Abs(velocity.x), maxSpeedX,
                accelerationFactor * Time.fixedDeltaTime) * facingDirection;
            var vY = velocity.y;
            vY += gravity * gravityMult * Time.fixedDeltaTime;
            vY = Mathf.Clamp(vY, -maxFallSpeed, 10000);
            Move(vX, vY);
        }

        public WallCollideStatuses CheckCollideWall(int facingDirection) {
            var footCastOrigin = new Vector2(bodyCollider.bounds.center.x, bodyCollider.bounds.min.y);
            var middleCastOrigin = new Vector2(bodyCollider.bounds.center.x, bodyCollider.bounds.center.y);
            var headCastOrigin = new Vector2(bodyCollider.bounds.center.x, bodyCollider.bounds.max.y);
            var bodyHalfWidth = bodyCollider.bounds.extents.x;
            var castDistance = bodyHalfWidth + stats.wallCheckDistance;
            var hitFoot = Physics2D.Raycast(footCastOrigin, Vector2.right * facingDirection, castDistance,
                stats.wallLayer);
            var hitMiddle = Physics2D.Raycast(middleCastOrigin, Vector2.right * facingDirection, castDistance,
                stats.wallLayer);
            var hitHead = Physics2D.Raycast(headCastOrigin, Vector2.right * facingDirection, castDistance,
                stats.wallLayer);
            // check if the wall has no angle, meaning the ray cast has direction perpendicular to the wall
            // check if hit foot normal is of angle 0, meaning the wall is straight
            var hitFootPerpendicular = hitFoot.collider &&  hitFoot.normal == Vector2.right || hitFoot.normal == Vector2.left;
            var hitMiddlePerpendicular = hitMiddle.collider &&  hitMiddle.normal == Vector2.right || hitMiddle.normal == Vector2.left;
            var hitHeadPerpendicular = hitHead.collider &&  hitHead.normal == Vector2.right || hitHead.normal == Vector2.left;
            var bodyOnWall = hitFoot && hitMiddle && hitHead && hitFootPerpendicular && hitMiddlePerpendicular &&
                              hitHeadPerpendicular;
            var bodyOnHang = hitFoot && hitMiddle && hitFootPerpendicular && hitMiddlePerpendicular &&
                             !hitHeadPerpendicular;
            Debug.DrawRay(footCastOrigin, Vector2.right * facingDirection * castDistance, Color.red);
            Debug.DrawRay(middleCastOrigin, Vector2.right * facingDirection * castDistance, Color.red);
            Debug.DrawRay(headCastOrigin, Vector2.right * facingDirection * castDistance, Color.red);
            return new WallCollideStatuses {
                BodyOnWall = bodyOnWall,
                BodyOnHang = bodyOnHang
            };
        }

        public void MoveOnNonGroundAnyDirection(float accel, float decel, float maxSpeed, float gravity,
            float gravityMult, Vector2 direction) {
            var accelerationFactor = Mathf.Abs(velocity.magnitude) > maxSpeed
                ? accel
                : decel;
            var speed = Mathf.MoveTowards(velocity.magnitude, maxSpeed,
                accelerationFactor * Time.fixedDeltaTime);
            var vX = direction.normalized.x * speed;
            var vY = direction.normalized.y * speed;
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
        public bool CheckIsOnGround() {
            var stickToGroundHit = Physics2D.Raycast(GetCastOrigin(), Vector2.down, stats.stickToGroundDistance,
                stats.groundLayer);
            isOnGround = isWalkableGroundCheckPassed || isOnWalkableSlope || (shouldStickToGround && stickToGroundHit);
            return isOnGround;
        }

        public bool CheckIsOnWater() {
            var hit = Physics2D.Raycast(GetCastOrigin(), Vector2.down, stats.stickToGroundDistance, stats.waterLayer);
            return hit;
        }

        public bool CheckCanWallHang(int facingDirection) {
            var collideWallStatues = CheckCollideWall(facingDirection);
            var isOnGround = CheckIsOnGround();
            var isInWater = CheckIsOnWater();
            var bodyWall = collideWallStatues.BodyOnWall;
            var bodyHang = collideWallStatues.BodyOnHang;
            if (bodyWall || bodyHang && !isOnGround && !isInWater) {
                return true;
            }
            return false;
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
