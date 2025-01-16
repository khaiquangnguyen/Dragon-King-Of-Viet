using System;
using Unity.Collections;
using UnityEngine;
using Object = System.Object;

namespace CharacterBehavior {
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class AIGameCharacter : GameCharacter {
        public readonly CharacterStateMachine stateMachine = new();
        public CharacterStats stats;
        private CharacterSpellCast characterSpellCast;
        private CharacterDefense characterDefense;

        public float inputDirectionX;
        public float inputDirectionY;
        [ReadOnly]
        public CharacterState characterState;
        public bool triggerJump;
        [ReadOnly]
        public int jumpCount = 0;
        public AICharacterCombatStats combatStats;
        [HideInInspector]
        public Animator animator;
        public CircleCollider2D attackCollider;
        public GameObject player;
        public bool shouldAttack;

        public void OnEnable() {
            shouldAttack = false;
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController2D>();
            characterController.body = GetComponent<Rigidbody2D>();
            characterController.bodyCollider = GetComponent<Collider2D>();
            healthManager = GetComponent<HealthManager>();
            stateMachine.AddState(characterSpellCast = new CharacterSpellCast(this, characterController));
            stateMachine.AddState(characterDefense = new CharacterDefense(this, characterController));
            stateMachine.ChangeState(CharacterState.Idle);
        }

        public void Update() {
            characterState = stateMachine.characterState;
            facingDirection = inputDirectionX switch {
                < 0 => -1,
                > 0 => 1,
                _ => facingDirection
            };


            stateMachine.Update();
            UpdateEnvironment();
        }

        public void FixedUpdate() {
            stateMachine.FixedUpdate();
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

        public bool CheckChangeToFallFromNonAirState() {
            if (environment == Environment.Air) {
                stateMachine.ChangeState(CharacterState.Falling);
                return true;
            }
            return false;
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
            var maxSpeed =  Mathf.Abs(stats.airMaxSpeed * inputDirection.normalized.magnitude);
            characterController.MoveOnNonGroundAnyDirectionNoGravity(acceleration, deceleration, maxSpeed, inputDirection);
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
            characterController.MoveOnNonGroundHorizontalWithGravity(acceleration, deceleration, maxSpeed, gravity, 1, facingDirection, stats.maxFallSpeed);
        }

        public void SwimFixedUpdate() {
            var acceleration = stats.waterAccel;
            var deceleration = stats.waterDecel;
            var maxSpeed = stats.waterMaxSpeed;
            var inputDirection = new Vector2(inputDirectionX, inputDirectionY);
            characterController.MoveOnNonGroundAnyDirectionNoGravity(acceleration, deceleration, maxSpeed, inputDirection);
        }

        // Change to idle should be added to fixed update movement
        // since if you put it in update like other CheckChange functions,
        // it will trigger almost immediately
        public bool CheckChangeToIdle() {
            var notMoving = Mathf.Approximately(characterController.velocity.magnitude, 0);
            if (notMoving && environment == Environment.Ground) {
                stateMachine.ChangeState(CharacterState.Idle);
                return true;
            }
            return false;
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
            return false;
        }

        public Vector2 GetPlayerPosition() {
            return player.transform.position;
        }

        /**
         * Patrol the ground. Move between all the points in the path
         */
        public void SimpleGroundPatrol(Vector2[] paths) {
        }

        /**
         * patrol an environment that is not affect by gravity. Move between all the points in the path
         */
        public void SimpleFlySwimPatrol() {

        }

        public void GetPathToward() {
        }

        public void GetFlyPathToward() {

        }

        public void GetRunPathToward() {

        }

        public void GetSwimPathToward() {

        }

        public void WalkToward() {
        }

        public void FlyToward() {
        }

        public void SwimToward() {
        }
    }
}
