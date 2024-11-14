using UnityEngine;

namespace CharacterBehavior {
    public class CharacterSkillCast : CharacterStateBehavior {
        private SkillState skillState = SkillState.Ready;
        private float skillStartTimestamp;
        private float newStateStartAt;
        public float skillStartupTime;
        public float skillActiveTime;
        public float skillRecoveryTime;
        public AnimationClip skillStartupAnimation;
        public AnimationClip skillActiveAnimation;
        public AnimationClip skillRecoveryAnimation;

        public CharacterSkillCast(AIGameCharacter gameCharacter, CharacterController2D controller) : base(gameCharacter,
            controller, CharacterState.CastSkill) { }

        public void SetSkillData(float startupTime, float activeTime, float recoveryTime,
            AnimationClip startupAnimation, AnimationClip activeAnimation, AnimationClip recoveryAnimation) {
            skillStartupTime = startupTime;
            skillActiveTime = activeTime;
            skillRecoveryTime = recoveryTime;
            skillStartupAnimation = startupAnimation;
            skillActiveAnimation = activeAnimation;
            skillRecoveryAnimation = recoveryAnimation;
        }

        public override void OnStateEnter() {
            skillState = SkillState.Ready;
            newStateStartAt = 0;
            skillState = SkillState.Ready;
            characterController.Move(0, 0);
        }

        public override void FixedUpdate() {
            if (skillState == SkillState.Ready) {
                gameCharacter.animator.Play(skillStartupAnimation.name);
                skillState = SkillState.Startup;
                newStateStartAt = Time.time;
                skillStartTimestamp = Time.time;
            }
            else if (skillState == SkillState.Startup) {
                if (Time.time - newStateStartAt > skillStartupTime) {
                    gameCharacter.animator.Play(skillActiveAnimation.name);
                    skillState = SkillState.Active;
                    newStateStartAt = Time.time;
                }
            }
            else if (skillState == SkillState.Active) {
                if (Time.time - newStateStartAt > skillActiveTime) {
                    gameCharacter.animator.Play(skillRecoveryAnimation.name);
                    skillState = SkillState.Recovery;
                    newStateStartAt = Time.time;
                }
            }
            else if (skillState == SkillState.Recovery) {
                if (Time.time - newStateStartAt >= skillRecoveryTime) {
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