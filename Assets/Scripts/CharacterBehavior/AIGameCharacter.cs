using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;

namespace CharacterBehavior {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class AIGameCharacter : GameCharacter {
        public CharacterStats stats;
        private CharacterSpellCast characterSpellCast;
        private CharacterDefense characterDefense;

        public float inputDirectionX;
        public float inputDirectionY;
        [ReadOnly]
        public CharacterMovementState characterMovementState;
        public bool triggerJump;
        [ReadOnly]
        public int jumpCount = 0;
        public AICharacterCombatStats combatStats;
        [HideInInspector]
        public Animator animator;
        public CircleCollider2D attackCollider;
        public CharacterMovementState movementState = CharacterMovementState.MovingOnGround;
        public GameObject player;
        public bool isPerformingNonMovementAction;
        public Environment lastCharacterEnvironmentState;
        public bool canChangeToWalking = true;
        public bool canChangeToFlying = true;
        public bool canChangeToSwimming = true;

        public void OnEnable() {
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController2D>();
            characterController.body = GetComponent<Rigidbody2D>();
            characterController.bodyCollider = GetComponent<Collider2D>();
            healthManager = GetComponent<HealthManager>();
        }

        public void Update() {
            facingDirection = inputDirectionX switch {
                < 0 => -1,
                > 0 => 1,
                _ => facingDirection
            };
            UpdateEnvironment();
        }

        public bool CheckChangeToMovingOnGround() {
            var canChange = characterController.CheckIsOnGround() && stats.canWalk && canChangeToWalking;
            if (canChange) {
                characterMovementState = CharacterMovementState.MovingOnGround;
                lastCharacterEnvironmentState = Environment.Ground;
                return true;
            }

            return false;
        }

        public bool CheckChangeToMovingInAir() {
            var canChange = characterController.CheckIsInAir() && stats.canFly && canChangeToFlying;
            if (canChange) {
                characterMovementState = CharacterMovementState.MovingInAir;
                lastCharacterEnvironmentState = Environment.Air;
                return true;
            }

            return false;
        }

        public bool CheckChangeToMovingInWater() {
            var canChange = characterController.CheckIsInWater() && stats.canSwim && canChangeToSwimming;
            if (canChange) {
                characterMovementState = CharacterMovementState.MovingInWater;
                lastCharacterEnvironmentState = Environment.Water;
                return true;
            }

            return false;
        }

        public void FixedUpdate() {
            // check state update
            if (characterController.CheckIsInAir() && characterMovementState != CharacterMovementState.Jumping &&
                !stats.canFly) {
                characterMovementState = CharacterMovementState.Falling;
            }

            CheckChangeToMovingOnGround();
            CheckChangeToMovingInAir();
            CheckChangeToMovingInWater();

            if (characterMovementState == CharacterMovementState.Falling) {
                NoFlyAerialFixedUpdate();
            }
        }

        public override void OnDealDamage(int damageDealt, IDamageTaker gameCharacter) {
            throw new NotImplementedException();
        }

        public override DamageResult OnTakeDamage(int damage, IDamageDealer damageDealer) {
            var currentHealth = this.healthManager.currentHealth;
            this.healthManager.TakeDamage(damage);
            var newHealth = this.healthManager.currentHealth;
            var healthLost = currentHealth - newHealth;
            var damageResult = new DamageResult {
                incomingDamage = damage,
                healthLost = healthLost,
                healthRemaining = newHealth,
            };
            return damageResult;
        }

        public void OnSkillOrAttackHit(int baseDamage, GameCharacter damagedCharacter) {
            var damage = (int)(baseDamage * damageMult);
            OnDealDamage(damage, damagedCharacter);
        }

        public void MoveOnGroundFixedUpdate() {
            var acceleration = stats.groundAccel;
            var deceleration = stats.groundDecel;
            var maxSpeed = Mathf.Abs(stats.groundMaxSpeed * inputDirectionX);
            characterController.MoveAlongGround(acceleration, deceleration, maxSpeed, facingDirection);
        }

        /*
         * Character has full control over their movement in the air.
         */
        public void FlyFixedUpdate() {
            var acceleration = stats.airAccel;
            var deceleration = stats.airDecel;
            var inputDirection = new Vector2(inputDirectionX, inputDirectionY);
            var maxSpeed = Mathf.Abs(stats.airMaxSpeed * inputDirection.normalized.magnitude);
            characterController.MoveOnNonGroundAnyDirectionNoGravity(acceleration, deceleration, maxSpeed,
                inputDirection);
        }

        /*
         * Character can't control their y direction in the air, can only control their horizontal movement
         * Useful for jumping and falling
         */
        public void NoFlyAerialFixedUpdate() {
            var acceleration = stats.airAccel;
            var deceleration = stats.airDecel;
            var gravity = stats.gravity;
            var maxSpeed = stats.airMaxSpeed;
            characterController.MoveOnNonGroundHorizontalWithGravity(acceleration, deceleration, maxSpeed, gravity, 1,
                facingDirection, stats.maxFallSpeed);
        }

        public void SwimFixedUpdate() {
            var acceleration = stats.waterAccel;
            var deceleration = stats.waterDecel;
            var maxSpeed = stats.waterMaxSpeed;
            var inputDirection = new Vector2(inputDirectionX, inputDirectionY);
            characterController.MoveOnNonGroundAnyDirectionNoGravity(acceleration, deceleration, maxSpeed,
                inputDirection);
        }

        public bool TriggerJump(float jumpHeight, float distance, float jumpDuration) {
            // given max height and jump duration, calculate initial jump velocity and gravity
            var gravity = 2 * jumpHeight / Mathf.Pow(jumpDuration, 2);
            var jumpSpeed = gravity * jumpDuration;
            var isOnGround = characterController.CheckIsOnGround();
            var canJump = isOnGround && triggerJump;
            if (canJump) {
                characterController.Move(characterController.velocity.x, jumpSpeed);
                characterController.LeaveGround();
                triggerJump = false;
                return true;
            }

            characterMovementState = CharacterMovementState.Jumping;
            return false;
        }

        public Vector2 GetPlayerPosition() {
            return player.transform.position;
        }

        /**
         * Patrol the ground. Move between all the points in the path
         */
        public void SimpleGroundPatrol(Vector2[] paths) { }

        /**
         * patrol an environment that is not affect by gravity. Move between all the points in the path
         */
        public void SimpleSwimPatrol() { }

        /**
         * patrol an environment that is not affect by gravity. Move between all the points in the path
         */
        public void SimpleFlyPatrol() { }

        public void GetPathToward() { }

        public void GetFlyPathToward() { }

        public void GetRunPathToward() { }

        public void GetSwimPathToward() { }

        public void WalkToward() { }

        public void FlyToward() { }

        public void SwimToward() { }

        public void DetectPlayer() { }
    }
}