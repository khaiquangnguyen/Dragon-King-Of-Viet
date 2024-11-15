using UnityEngine;

namespace CharacterBehavior {
    public class CharacterFly : CharacterStateBehavior {
        public CharacterFly(AIGameCharacter gameCharacter, CharacterController2D controller2D) : base(gameCharacter,
            controller2D,
            CharacterState.Flying) { }

        public override void OnStateEnter() { }

        public override void FixedUpdate() {
            var acceleration = gameCharacter.stats.airAccel;
            var deceleration = gameCharacter.stats.airDecel;
            var maxSpeed = gameCharacter.stats.airMaxSpeed;
            var inputDirection = new Vector2(gameCharacter.inputDirectionX, gameCharacter.inputDirectionY);
            characterController.MoveOnNonGroundAnyDirection(acceleration, deceleration, maxSpeed, 0, 0, inputDirection);
        }
    }
}