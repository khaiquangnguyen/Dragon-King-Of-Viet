using UnityEngine;

namespace CharacterBehavior {
    public class CharacterRun : CharacterStateBehavior {
        private int facingDirection => gameCharacter.facingDirection;

        public CharacterRun(AIGameCharacter gameCharacter, CharacterController2D controller2D) : base(gameCharacter,
            controller2D,
            CharacterState.Running) { }

        public override void OnStateEnter() { }

        public override void FixedUpdate() {
            if (Mathf.Approximately(characterController.velocity.magnitude, 0))
                gameCharacter.stateMachine.ChangeState(CharacterState.Idle);
            var acceleration = gameCharacter.stats.groundAccel;
            var deceleration = gameCharacter.stats.groundDecel;
            var maxSpeed = Mathf.Abs(gameCharacter.stats.groundMaxSpeed * gameCharacter.inputDirectionX);
            if (!characterController.isOnGround())
                gameCharacter.stateMachine.ChangeState(CharacterState.Falling);
            else
                characterController.MoveAlongGround(acceleration, deceleration, maxSpeed, facingDirection);
        }

        public override void Update() { }
        public override void OnStateExit() { }
    }
}