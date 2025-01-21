using UnityEngine;

namespace CharacterBehavior {
    public class CharacterDefense : CharacterStateBehavior {
        private float newStateStartAt;
        private float defenseStartTimestamp;
        private DefenseState defenseState = DefenseState.Ready;

        public CharacterDefense(AIGameCharacter gameCharacter, CharacterController2D controller) : base(gameCharacter,
            controller, CharacterMovementState.Defense) { }

        public override void OnStateEnter() {
            characterController.Move(0, 0);
            defenseState = DefenseState.Ready;
            newStateStartAt = Time.time;
        }

        public override void FixedUpdate() {
            if (defenseState == DefenseState.Ready) {
                defenseState = DefenseState.Startup;
                defenseStartTimestamp = Time.time;
                newStateStartAt = Time.time;
            }
            else if (defenseState == DefenseState.Startup) {
                gameCharacter.animator.Play(gameCharacter.combatStats.defenseStartupAnimation.name);
                if (Time.time - newStateStartAt > gameCharacter.combatStats.defenseStartupDuration) {
                    defenseState = DefenseState.ActiveNoCounter;
                    newStateStartAt = Time.time;
                }
            }

            else if (defenseState == DefenseState.ActiveNoCounter) {
                gameCharacter.animator.Play(gameCharacter.combatStats.defenseActiveAnimation.name);
                if (Time.time - newStateStartAt > gameCharacter.combatStats.defenseActiveDuration) {
                    defenseState = DefenseState.ActiveCounter;
                    newStateStartAt = Time.time;
                }
            }
            else if (defenseState == DefenseState.Recovery) {
                gameCharacter.animator.Play(gameCharacter.combatStats.defenseRecoveryAnimation.name);
            }
        }
    }
}