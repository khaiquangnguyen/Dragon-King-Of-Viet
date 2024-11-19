using UnityEngine;

namespace CharacterBehavior {
    public class CharacterFall : CharacterStateBehavior {
        public CharacterFall(AIGameCharacter gameCharacter, CharacterController2D controller2D) : base(gameCharacter,
            controller2D,
            CharacterState.Falling) { }

        public override void OnStateEnter() { }

        public override void Update() {
            gameCharacter.CheckChangeToRunState();
        }


        public override void FixedUpdate() {
            var acceleration = gameCharacter.stats.airAccel;
            var deceleration = gameCharacter.stats.airDecel;
            var maxSpeedX = gameCharacter.stats.airMaxSpeed * Mathf.Abs(gameCharacter.inputDirectionX);
            characterController.MoveOnNonGroundHorizontal(acceleration, deceleration, maxSpeedX,
                gameCharacter.stats.gravity, 1, gameCharacter.facingDirection, gameCharacter.stats.maxFallSpeed);
            if (characterController.isOnWalkableGround()) {
                gameCharacter.jumpCount = 0;
                if (gameCharacter.inputDirectionX == 0) {
                    characterController.Move(0, 0);
                    gameCharacter.stateMachine.ChangeState(CharacterState.Idle);
                }
                else {
                    characterController.Move(maxSpeedX, 0);
                    gameCharacter.stateMachine.ChangeState(CharacterState.Running);
                }
            }
        }

        public override void OnStateExit() {
            base.OnStateExit();
        }
    }
}
