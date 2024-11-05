using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterBehavior {
    [RequireComponent(typeof(CharacterController2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]

    public class AICharacter : Character {
        public CharacterStateMachine stateMachine = new();
        [HideInInspector]
        public CharacterController2D controller;
        public CharacterStats stats;
        // private CharacterStateBehavior characterRun;
        private CharacterRun characterRun;
        private CharacterIdle characterIdle;
        private CharacterFall characterFall;
        private CharacterJump characterJump;
        public float inputDirectionX;
        [ReadOnly]
        public CharacterState characterState;
        [FormerlySerializedAs("shouldTryJump")]
        public bool triggerJump;
        [ReadOnly]
        public int jumpCount = 0;

        public void OnEnable() {
            controller = GetComponent<CharacterController2D>();
            controller.body = GetComponent<Rigidbody2D>();
            controller.bodyCollider = GetComponent<Collider2D>();
            stateMachine.AddState(characterRun = new CharacterRun(this, controller));
            stateMachine.AddState(characterFall =  new CharacterFall(this, controller));
            stateMachine.AddState(characterIdle = new CharacterIdle(this, controller));
            stateMachine.AddState(characterJump = new CharacterJump(this, controller));
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

        public override void OnDamageDealt(float damageDealt, Character character) {
            throw new NotImplementedException();
        }

        public override float TakeDamage(float damage, Character damageDealer) {
            throw new NotImplementedException();
        }
    }
}
