using System.Collections.Generic;
using UnityEngine;

namespace CharacterBehavior {
    public abstract class BaseCharacterAttack : CharacterStateBehavior {
        private int attackMoveCount = 0;
        private SkillState skillState = SkillState.Ready;
        private float attackStartTimestamp;
        private float newStateStartAt;
        public List<GameCharacter> hitCharacters = new List<GameCharacter>();

        public BaseCharacterAttack(AIGameCharacter gameCharacter, CharacterController2D controller,
            CharacterState characterState) : base(gameCharacter, controller, characterState) { }

        public override void OnStateEnter() {
            skillState = SkillState.Ready;
            newStateStartAt = 0;
            skillState = SkillState.Ready;
            attackMoveCount = 0;
            characterController.Move(0, 0);
        }

        public void EnterStartupCurrentAttack(AnimationClip startupAnimation) {
            if (skillState == SkillState.Ready) {
                hitCharacters.Clear();
                gameCharacter.animator.Play(startupAnimation.name);
                skillState = SkillState.Startup;
                newStateStartAt = Time.time;
                attackStartTimestamp = Time.time;
            }
        }

        public void EnterActiveCurrentAttack(AnimationClip activeAnimation) {
            if (skillState == SkillState.Startup) {
                gameCharacter.animator.Play(activeAnimation.name);
                skillState = SkillState.Active;
                newStateStartAt = Time.time;
            }
        }

        public void EnterRecoveryCurrentAttack(AnimationClip recoveryAnimation) {
            if (skillState == SkillState.Active) {
                gameCharacter.animator.Play(recoveryAnimation.name);
                skillState = SkillState.Recovery;
                newStateStartAt = Time.time;
            }
        }

        public void CheckAttackHit() {
            var hitboxCollider = gameCharacter.attackCollider;
            if (!hitboxCollider) return;
            //get all colliders that are in the hitbox
            var collidedCharacters =
                Physics2D.OverlapCircleAll(hitboxCollider.bounds.center, hitboxCollider.radius);
            foreach (var character in collidedCharacters) {
                if (character.GetComponent<GameCharacter>() is not null) {
                    var otherCharacter = character.GetComponent<GameCharacter>();
                    if (otherCharacter == this.gameCharacter) continue;
                    if (hitCharacters.Contains(otherCharacter)) continue;
                    gameCharacter.OnSkillOrAttackHit(gameCharacter.combatStats.attackStats[attackMoveCount].damage,
                        otherCharacter);
                    hitCharacters.Add(otherCharacter);
                }
            }
        }
    }
}
