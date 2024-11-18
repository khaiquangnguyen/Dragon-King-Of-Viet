using UnityEngine;

namespace CharacterBehavior {
    public class CharacterRun : CharacterStateBehavior {
        private int facingDirection => gameCharacter.facingDirection;

        public CharacterRun(AIGameCharacter gameCharacter, CharacterController2D controller2D) : base(gameCharacter,
            controller2D,
            CharacterState.Running) { }

        public override void OnStateEnter() { }

        public override void Update() {
            if (gameCharacter.CheckChangeToFallFromNonAirState()) return;
        }

        public override void FixedUpdate() {
            if (Mathf.Approximately(characterController.velocity.magnitude, 0))
                gameCharacter.stateMachine.ChangeState(CharacterState.Idle);
            var acceleration = gameCharacter.stats.groundAccel;
            var deceleration = gameCharacter.stats.groundDecel;
            var maxSpeed = Mathf.Abs(gameCharacter.stats.groundMaxSpeed * gameCharacter.inputDirectionX);
            characterController.MoveAlongGround(acceleration, deceleration, maxSpeed, facingDirection);
            if (!(gameCharacter.CheckChangeToIdle())) return;
        }

        public override void OnStateExit() { }
    }
}
