using System.Collections.Generic;
using UnityEngine;

namespace CharacterBehavior {
    public class CharacterComboAttacks : BaseCharacterAction {
        private int attackMoveCount = 0;
        private float attackStartTimestamp;
        private readonly List<AttackStats> comboAttackStats;

        public CharacterComboAttacks(AIGameCharacter gameCharacter, CharacterController2D controller,
            List<AttackStats> comboAttackStats) : base(
            gameCharacter, controller, CharacterState.ComboAttacks) {
            this.comboAttackStats = comboAttackStats;
        }

        public override void OnStateEnter() {
            base.OnStateEnter();
            attackMoveCount = 0;
        }

        public override void FixedUpdate() {
            var attackStartupTime = comboAttackStats[attackMoveCount].startupDuration;
            var attackActiveTime = comboAttackStats[attackMoveCount].activeDuration;
            var attackRecoveryTime = comboAttackStats[attackMoveCount].recoveryDuration;
            var startupAnimation = comboAttackStats[attackMoveCount].startupAnimation;
            var activeAnimation = comboAttackStats[attackMoveCount].activeAnimation;
            var recoveryAnimation = comboAttackStats[attackMoveCount].recoveryAnimation;
            if (skillState == SkillState.Ready) {
                EnterStartup(startupAnimation);
            }
            else if (skillState == SkillState.Startup) {
                if (Time.time - newStateStartAt > attackStartupTime) {
                    EnterActive(activeAnimation);
                }
            }
            else if (skillState == SkillState.Active) {
                CheckAttackHit(comboAttackStats[attackMoveCount].damage);
                if (Time.time - newStateStartAt > attackActiveTime) {
                    EnterRecovery(recoveryAnimation);
                }
            }
            else if (skillState == SkillState.Recovery) {
                hitCharacters.Clear();
                if (Time.time - newStateStartAt < attackRecoveryTime) return;
                // more attack to fire
                if (attackMoveCount < comboAttackStats.Count - 1) {
                    attackMoveCount++;
                    skillState = SkillState.Ready;
                    newStateStartAt = Time.time;
                }
                else {
                    gameCharacter.stateMachine.ChangeState(CharacterState.Idle);
                }
            }
        }
    }
}
