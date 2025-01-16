namespace CharacterBehavior {
    public abstract class CharacterStateBehavior {
        public readonly CharacterMovementState characterMovementState;
        protected readonly AIGameCharacter gameCharacter;
        protected readonly CharacterController2D characterController;

        protected CharacterStateBehavior(AIGameCharacter gameCharacter, CharacterController2D controller,
            CharacterMovementState characterMovementState) {
            this.characterMovementState = characterMovementState;
            this.gameCharacter = gameCharacter;
            characterController = controller;
        }

        public virtual void OnStateEnter() { }
        public virtual void Update() { }

        public virtual void FixedUpdate() { }
        public virtual void OnStateExit() { }
    }
}