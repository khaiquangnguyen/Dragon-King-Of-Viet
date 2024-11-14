using System.Collections.Generic;
using UnityEngine;

namespace CharacterBehavior {
    public class CharacterBasicAttack : BaseCharacterAttack {
        private int attackMoveCount = 0;
        private SkillState skillState = SkillState.Ready;
        private float attackStartTimestamp;
        private float newStateStartAt;
        public List<GameCharacter> hitCharacters = new();

        public CharacterBasicAttack(AIGameCharacter gameCharacter, CharacterController2D controller) : base(
            gameCharacter, controller, CharacterState.BasicAttack) { }

        public override void FixedUpdate() {
            var attackStartupTime = gameCharacter.combatStats.attackStats[attackMoveCount].startupDuration;
            var attackActiveTime = gameCharacter.combatStats.attackStats[attackMoveCount].activeDuration;
            var attackRecoveryTime = gameCharacter.combatStats.attackStats[attackMoveCount].recoveryDuration;
            var startupAnimation = gameCharacter.combatStats.attackStats[attackMoveCount].startupAnimation;
            var activeAnimation = gameCharacter.combatStats.attackStats[attackMoveCount].activeAnimation;
            var recoveryAnimation = gameCharacter.combatStats.attackStats[attackMoveCount].recoveryAnimation;
            if (skillState == SkillState.Ready) {
                EnterStartupCurrentAttack(startupAnimation);
            }
            else if (skillState == SkillState.Startup) {
                if (Time.time - newStateStartAt > attackStartupTime) {
                    EnterActiveCurrentAttack(activeAnimation);
                }
            }
            else if (skillState == SkillState.Active) {
                CheckAttackHit();
                if (Time.time - newStateStartAt > attackActiveTime) {
                    EnterRecoveryCurrentAttack(recoveryAnimation);
                }
            }
            else if (skillState == SkillState.Recovery) {
                hitCharacters.Clear();
                if (Time.time - newStateStartAt < attackRecoveryTime) return;
                // more attack to fire
                if (attackMoveCount < gameCharacter.combatStats.attackStats.Count - 1) {
                    attackMoveCount++;
                    skillState = SkillState.Ready;
                    newStateStartAt = Time.time;
                }
                else {
                    gameCharacter.stateMachine.ChangeState(CharacterState.Idle);
                }
            }

            if (gameCharacter.environment == Environment.Ground)
                characterController.Move(0, 0);
            else
                characterController.Move(0, characterController.velocity.y);
        }
    }
}