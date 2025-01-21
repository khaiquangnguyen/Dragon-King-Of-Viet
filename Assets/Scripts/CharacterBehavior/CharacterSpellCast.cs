using UnityEngine;

namespace CharacterBehavior {
    public class CharacterSpellCast : BaseCharacterAction {
        public SkillState skillState = SkillState.Ready;
        public float skillStartTimestamp;
        public float newStateStartAt;
        public float skillStartupTime;
        public float skillActiveTime;
        public float skillRecoveryTime;
        public Animator animator;
        public AnimationClip skillStartupAnimation;
        public AnimationClip skillActiveAnimation;
        public AnimationClip skillRecoveryAnimation;
        public bool skillUsed;
        public Skill currentSkill;

        public CharacterSpellCast(AIGameCharacter gameCharacter, CharacterController2D controller) : base(gameCharacter,
            controller, CharacterMovementState.CastSpell) { }

        public void SetSkillData(Skill skill, float startupTime, float activeTime, float recoveryTime,
            AnimationClip startupAnimation, AnimationClip activeAnimation, AnimationClip recoveryAnimation) {
            currentSkill = skill;
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
                EnterStartup(skillStartupAnimation);
            }
            else if (skillState == SkillState.Startup) {
                if (Time.time - newStateStartAt > skillStartupTime) {
                    EnterActive(skillActiveAnimation);
                }
            }
            else if (skillState == SkillState.Active) {
                if (Time.time - newStateStartAt > skillActiveTime) {
                    EnterRecovery(skillRecoveryAnimation);
                }
            }
            else if (skillState == SkillState.Recovery) {
                if (Time.time - newStateStartAt >= skillRecoveryTime) { }
            }
        }
    }
}