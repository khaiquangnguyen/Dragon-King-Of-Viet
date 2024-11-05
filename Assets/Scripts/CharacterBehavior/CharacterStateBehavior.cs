namespace CharacterBehavior {
    public abstract class CharacterStateBehavior {
        public readonly CharacterState name;
        protected readonly AICharacter character;
        protected readonly CharacterController2D characterController;

        protected CharacterStateBehavior(AICharacter character, CharacterController2D controller, CharacterState name) {
            this.name = name;
            this.character = character;
            characterController = controller;
        }

        public virtual void OnStateEnter() { }
        public virtual void Update() { }

        public virtual void FixedUpdate() { }
        public virtual void OnStateExit() { }
    }
}
