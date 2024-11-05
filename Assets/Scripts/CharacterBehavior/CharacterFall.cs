using UnityEngine;

namespace CharacterBehavior {
    public class CharacterFall : CharacterStateBehavior {
        public CharacterFall(AICharacter character, CharacterController2D controller2D) : base(character, controller2D,
            CharacterState.Falling) { }

        public override void OnStateEnter() { }

        public override void FixedUpdate() {
            var acceleration = character.stats.airAccel;
            var deceleration = character.stats.airDecel;
            var maxSpeedX = character.stats.airMaxSpeed * Mathf.Abs(character.inputDirectionX);
            characterController.MoveOnAirWithGravityApplied(acceleration, deceleration, maxSpeedX,
                character.stats.gravity, 1, character.facingDirection, character.stats.maxFallSpeed);
            if (characterController.isOnWalkableGround()) {
                character.jumpCount = 0;
                if (character.inputDirectionX == 0) {
                    characterController.Move(0, 0);
                    character.stateMachine.ChangeState(CharacterState.Idle);
                }
                else {
                    characterController.Move(maxSpeedX, 0);
                    character.stateMachine.ChangeState(CharacterState.Running);
                }
            }
        }

        public override void OnStateExit() {
            base.OnStateExit();
        }
    }
}
