using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterBehavior {
    [RequireComponent(typeof(CharacterController2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class AIGameCharacter : GameCharacter {
        public CharacterStateMachine stateMachine = new();
        [HideInInspector]
        public CharacterController2D controller;
        public CharacterStats stats;
        // private CharacterStateBehavior characterRun;
        private CharacterRun characterRun;
        private CharacterIdle characterIdle;
        private CharacterFall characterFall;
        private CharacterJump characterJump;
        private BaseCharacterAttack baseCharacterAttack;
        private CharacterSkillCast characterSkillCast;
        private CharacterDefense characterDefense;

        public float inputDirectionX;
        [ReadOnly]
        public CharacterState characterState;
        public bool triggerJump;
        [ReadOnly]
        public int jumpCount = 0;
        public AICharacterCombatStats combatStats;
        public Animator animator;
        public Environment environment;
        public Collider2D attackCollider;

        public void OnEnable() {
            controller = GetComponent<CharacterController2D>();
            controller.body = GetComponent<Rigidbody2D>();
            controller.bodyCollider = GetComponent<Collider2D>();
            healthManager = GetComponent<HealthManager>();
            stateMachine.AddState(characterRun = new CharacterRun(this, controller));
            stateMachine.AddState(characterFall = new CharacterFall(this, controller));
            stateMachine.AddState(characterIdle = new CharacterIdle(this, controller));
            stateMachine.AddState(characterJump = new CharacterJump(this, controller));
            stateMachine.AddState(baseCharacterAttack = new CharacterBasicAttack(this, controller));
            stateMachine.AddState(characterSkillCast = new CharacterSkillCast(this, controller));
            stateMachine.AddState(characterDefense = new CharacterDefense(this, controller));
            stateMachine.ChangeState(CharacterState.Idle);
        }

        public void Update() {
            characterState = stateMachine.characterState;
            facingDirection = inputDirectionX switch {
                < 0 => -1,
                > 0 => 1,
                _ => facingDirection
            };
            if (triggerJump) {
                var canJump = jumpCount < stats.maxJumpsCount;
                if (canJump) {
                    if (stateMachine.characterState != CharacterState.Jumping) {
                        stateMachine.ChangeState(CharacterState.Jumping);
                    }
                    else {
                        characterJump.InitiateJump();
                    }
                }

                triggerJump = false;
            }

            stateMachine.Update();
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
    }
}
