namespace CharacterBehavior {
    public class CharacterIdle : CharacterStateBehavior {
        public CharacterIdle(AICharacter character, CharacterController2D controller2D) : base(character, controller2D,
            CharacterState.Idle) { }

        public override void OnStateEnter() { }

        public override void Update() {
            if (character.inputDirectionX != 0) character.stateMachine.ChangeState(CharacterState.Running);
        }

        public override void FixedUpdate() {
            characterController.Move(0, 0);
            var isOnGround = characterController.isOnWalkableGround();
            if (!isOnGround) character.stateMachine.ChangeState(CharacterState.Falling);
        }

        public override void OnStateExit() { }
    }
}
