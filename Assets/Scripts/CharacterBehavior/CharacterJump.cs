using UnityEngine;

namespace CharacterBehavior {
    public class CharacterJump : CharacterStateBehavior {
        public CharacterJump(AICharacter character, CharacterController2D controller2D) : base(character, controller2D,
            CharacterState.Jumping) { }

        public override void OnStateEnter() {
            this.InitiateJump();
        }

        public void InitiateJump() {
            character.jumpCount++;
            // given max height and gravity, calculate initial jump velocity and duration
            var jumpHeight = character.stats.jumpHeight;
            var gravity = character.stats.gravity;
            var jumpVelocity = Mathf.Sqrt(Mathf.Abs(2 * jumpHeight * gravity));
            characterController.Move(characterController.velocity.x,jumpVelocity);
        }

        public override void FixedUpdate() {
            var acceleration = character.stats.airAccel;
            var deceleration = character.stats.airDecel;
            var maxSpeedX = character.stats.airMaxSpeed * Mathf.Abs(character.inputDirectionX);
            characterController.MoveOnAirWithGravityApplied(acceleration, deceleration, maxSpeedX,
                character.stats.gravity, 1, character.facingDirection, character.stats.maxFallSpeed);
            if (characterController.velocity.y <= 0) {
                character.stateMachine.ChangeState(CharacterState.Falling);
            }
        }

        public override void Update() { }
        public override void OnStateExit() { }
    }
}
