namespace CharacterBehavior {
    public abstract class CharacterStateBehavior {
        public readonly CharacterState name;
        protected readonly AIGameCharacter gameCharacter;
        protected readonly CharacterController2D characterController;

        protected CharacterStateBehavior(AIGameCharacter gameCharacter, CharacterController2D controller,
            CharacterState name) {
            this.name = name;
            this.gameCharacter = gameCharacter;
            characterController = controller;
        }

        public virtual void OnStateEnter() { }
        public virtual void Update() { }

        public virtual void FixedUpdate() { }
        public virtual void OnStateExit() { }
    }
}