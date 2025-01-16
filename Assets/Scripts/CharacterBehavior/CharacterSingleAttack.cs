using System.Collections.Generic;
using UnityEngine;

namespace CharacterBehavior {
    public class CharacterSingleAttack : BaseCharacterAction {
        private float attackStartTimestamp;
        private readonly AttackStats attackStats;

        public CharacterSingleAttack(AIGameCharacter gameCharacter, CharacterController2D controller,
            AttackStats attackStats) : base(
            gameCharacter, controller, CharacterMovementState.ComboAttacks) {
            this.attackStats = attackStats;
        }

        public override void OnStateEnter() {
            base.OnStateEnter();
        }

        public override void FixedUpdate() {
            var attackStartupTime = attackStats.startupDuration;
            var attackActiveTime = attackStats.activeDuration;
            var attackRecoveryTime = attackStats.recoveryDuration;
            var startupAnimation = attackStats.startupAnimation;
            var activeAnimation = attackStats.activeAnimation;
            var recoveryAnimation = attackStats.recoveryAnimation;
            if (skillState == SkillState.Ready) {
                EnterStartup(startupAnimation);
            }
            else if (skillState == SkillState.Startup) {
                if (Time.time - newStateStartAt > attackStartupTime) {
                    EnterActive(activeAnimation);
                }
            }
            else if (skillState == SkillState.Active) {
                CheckAttackHit(attackStats.damage);
                if (Time.time - newStateStartAt > attackActiveTime) {
                    EnterRecovery(recoveryAnimation);
                }
            }
            else if (skillState == SkillState.Recovery) {
                hitCharacters.Clear();
                if (Time.time - newStateStartAt < attackRecoveryTime) return;
            }
        }
    }
}
