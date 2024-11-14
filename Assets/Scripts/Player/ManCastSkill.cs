using System.Linq;
using UnityEngine;

public class ManCastSkill : PlayerStateBehavior {
    private SkillState skillState = SkillState.Ready;
    private float skillStartTimestamp;
    private float newStateStartAt;
    public float skillStartupTime;
    public float skillActiveTime;
    public float skillRecoveryTime;
    public AnimationClip skillStartupAnimation;
    public AnimationClip skillActiveAnimation;
    public AnimationClip skillRecoveryAnimation;
    public Animator animator => player.humanAnimator;
    public Skill currentSkill;
    public bool skillUsed = false;

    public ManCastSkill(Player player) : base(player, PlayerState.ManCastSkill, PlayerForm.Man) { }

    public void SetSkillData(Skill skill, float startupTime, float activeTime, float recoveryTime,
        AnimationClip startupAnimation,
        AnimationClip activeAnimation, AnimationClip recoveryAnimation) {
        currentSkill = skill;
        skillStartupTime = startupTime;
        skillActiveTime = activeTime;
        skillRecoveryTime = recoveryTime;
        skillStartupAnimation = startupAnimation;
        skillActiveAnimation = activeAnimation;
        skillRecoveryAnimation = recoveryAnimation;
        skillUsed = false;
    }

    public override void OnStateEnter() {
        skillState = SkillState.Ready;
        newStateStartAt = 0;
        skillState = SkillState.Ready;
        player.characterController.Move(0, 0);
    }

    public override void FixedUpdate() {
        if (skillState == SkillState.Ready) {
            skillState = SkillState.Startup;
            newStateStartAt = Time.time;
            skillStartTimestamp = Time.time;
        }
        else if (skillState == SkillState.Startup) {
            Utils.PlayAnimationMatchingDuration(animator, skillStartupAnimation, skillStartupTime);
            if (Time.time - newStateStartAt > skillStartupTime) {
                skillState = SkillState.Active;
                newStateStartAt = Time.time;
            }
        }
        else if (skillState == SkillState.Active) {
            Utils.PlayAnimationMatchingDuration(animator, skillActiveAnimation, skillActiveTime);
            if (!skillUsed) {
                currentSkill.Use();
                skillUsed = true;
            }

            if (Time.time - newStateStartAt > skillActiveTime) {
                skillState = SkillState.Recovery;
                newStateStartAt = Time.time;
            }
        }
        else if (skillState == SkillState.Recovery) {
            Utils.PlayAnimationMatchingDuration(animator, skillRecoveryAnimation, skillRecoveryTime);
            if (Time.time - newStateStartAt >= skillRecoveryTime) {
                animator.speed = 1;
                player.stateMachine.ChangeState(PlayerState.ManIdle);
            }
        }

        if (player.environment == Environment.Ground)
            player.characterController.Move(0, 0);
        else
            player.characterController.Move(0, player.characterController.velocity.y);
    }
}