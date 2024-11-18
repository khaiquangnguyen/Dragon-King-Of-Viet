using UnityEngine;

public abstract class BaseSpellCast : BasePlayerAction {
    public float skillStartTimestamp;
    public float skillStartupTime;
    public float skillActiveTime;
    public float skillRecoveryTime;
    public AnimationClip skillStartupAnimation;
    public AnimationClip skillActiveAnimation;
    public AnimationClip skillRecoveryAnimation;
    public bool skillUsed;
    public Skill currentSkill;

    public BaseSpellCast(Player player, PlayerState state, PlayerForm form) : base(player, state, form) { }

    public override void OnStateEnter() {
        skillState = SkillState.Ready;
        newStateStartAt = 0;
        skillState = SkillState.Ready;
        player.characterController.Move(0, 0);
    }

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

    public abstract void OnRecoveryEnd();

    public override void FixedUpdate() {
        if (skillState == SkillState.Ready) {
            EnterStartup(skillStartupAnimation, skillStartupTime);
        }
        else if (skillState == SkillState.Startup) {
            if (Time.time - newStateStartAt > skillStartupTime) {
                currentSkill.Use();
                skillUsed = true;
                EnterActive(skillActiveAnimation, skillActiveTime);
            }
        }
        else if (skillState == SkillState.Active) {
            if (Time.time - newStateStartAt > skillActiveTime) {
                EnterRecovery(skillRecoveryAnimation, skillRecoveryTime);
            }
        }
        else if (skillState == SkillState.Recovery) {
            if (Time.time - newStateStartAt >= skillRecoveryTime) {
                OnRecoveryEnd();
            }
        }
    }
}
