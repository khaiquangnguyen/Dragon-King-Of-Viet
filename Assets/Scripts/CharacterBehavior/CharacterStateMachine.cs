using System.Collections.Generic;
using UnityEngine;

namespace CharacterBehavior {
    public class CharacterStateMachine {
        public CharacterState characterState;

        public readonly Dictionary<CharacterState, CharacterStateBehavior> states = new();
        public CharacterStateBehavior currentStateBehavior => states[characterState];

        public void ChangeState(CharacterState newCharacterState) {
            if (newCharacterState == characterState) return;
            currentStateBehavior.OnStateExit();
            characterState = newCharacterState;
            var newState = states[newCharacterState];
            newState.OnStateEnter();
        }

        public void Update() {
            currentStateBehavior.Update();
        }

        public void FixedUpdate() {
            currentStateBehavior.FixedUpdate();
        }

        public void LateUpdate() { }

        public void AddState(CharacterStateBehavior state) {
            states.Add(state.characterState, state);
        }
    }
}
