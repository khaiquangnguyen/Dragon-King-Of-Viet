using UnityEngine;

namespace CharacterBehavior {
    public class CharacterJump : CharacterStateBehavior {
        public CharacterJump(AIGameCharacter gameCharacter, CharacterController2D controller2D) : base(gameCharacter,
            controller2D,
            CharacterState.Jumping) { }

        public override void OnStateEnter() {
            this.InitiateJump();
        }

        public void InitiateJump() {
            gameCharacter.jumpCount++;
            // given max height and gravity, calculate initial jump velocity and duration
            var jumpHeight = gameCharacter.stats.jumpHeight;
            var gravity = gameCharacter.stats.gravity;
            var jumpVelocity = Mathf.Sqrt(Mathf.Abs(2 * jumpHeight * gravity));
            characterController.Move(characterController.velocity.x, jumpVelocity);
        }

        public override void FixedUpdate() {
            var acceleration = gameCharacter.stats.airAccel;
            var deceleration = gameCharacter.stats.airDecel;
            var maxSpeedX = gameCharacter.stats.airMaxSpeed * Mathf.Abs(gameCharacter.inputDirectionX);
            characterController.MoveOnNonGroundHorizontal(acceleration, deceleration, maxSpeedX,
                gameCharacter.stats.gravity, 1, gameCharacter.facingDirection, gameCharacter.stats.maxFallSpeed);
            if (characterController.velocity.y <= 0) {
                gameCharacter.stateMachine.ChangeState(CharacterState.Falling);
            }
        }

        public override void Update() { }
        public override void OnStateExit() { }
    }
}