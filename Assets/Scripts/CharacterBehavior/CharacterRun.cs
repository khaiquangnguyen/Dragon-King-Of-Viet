using UnityEngine;

namespace CharacterBehavior {
    public class CharacterRun : CharacterStateBehavior {
        private int facingDirection => character.facingDirection;

        public CharacterRun(AICharacter character, CharacterController2D controller2D) : base(character, controller2D,
            CharacterState.Running) { }

        public override void OnStateEnter() { }

        public override void FixedUpdate() {
            if (Mathf.Approximately(characterController.velocity.magnitude, 0))
                character.stateMachine.ChangeState(CharacterState.Idle);
            var acceleration = character.stats.groundAccel;
            var deceleration = character.stats.groundDecel;
            var maxSpeed = Mathf.Abs(character.stats.groundMaxSpeed * character.inputDirectionX);
            if (!characterController.isOnGround())
                character.stateMachine.ChangeState(CharacterState.Falling);
            else
                characterController.MoveAlongGround(acceleration, deceleration, maxSpeed, facingDirection);
        }

        public override void Update() { }
        public override void OnStateExit() { }
    }
}