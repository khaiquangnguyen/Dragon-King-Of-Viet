namespace CharacterBehavior {
    public class CharacterIdle : CharacterStateBehavior {
        public CharacterIdle(AIGameCharacter gameCharacter, CharacterController2D controller2D) : base(gameCharacter,
            controller2D,
            CharacterState.Idle) { }

        public override void OnStateEnter() {
            gameCharacter.animator.Play(gameCharacter.stats.idleAnimation.name);
        }

        public override void Update() {
            if (gameCharacter.inputDirectionX != 0) gameCharacter.stateMachine.ChangeState(CharacterState.Running);
        }

        public override void FixedUpdate() {
            characterController.Move(0, 0);
            var isOnGround = characterController.isOnWalkableGround();
            if (!isOnGround) gameCharacter.stateMachine.ChangeState(CharacterState.Falling);
        }

        public override void OnStateExit() { }
    }
}
