using System.Collections.Generic;
using CharacterBehavior;
using UnityEngine;

public abstract class BasePlayerAction : PlayerStateBehavior {
    public int attackMoveCount = 0;
    public SkillState skillState = SkillState.Ready;
    public float newStateStartAt;
    public float attackStartTimestamp;
    public List<AttackStats> attackStatsList;
    public readonly List<GameCharacter> hitCharacters = new();
    public Animator animator;

    public BasePlayerAction(Player player, PlayerState playerState, PlayerForm playerForm) : base(player, playerState,
        playerForm) { }

    public (float, float, float, AnimationClip, AnimationClip, AnimationClip) GetAttackTimingAndAnimation() {
        var attackStartupTime = attackStatsList[attackMoveCount].startupDuration;
        var attackActiveTime = player.playerStats.manAttackStats[attackMoveCount].activeDuration;
        var attackRecoveryTime = player.playerStats.manAttackStats[attackMoveCount].recoveryDuration;
        var startupAnimation = player.playerStats.manAttackStats[attackMoveCount].startupAnimation;
        var activeAnimation = player.playerStats.manAttackStats[attackMoveCount].activeAnimation;
        var recoveryAnimation = player.playerStats.manAttackStats[attackMoveCount].recoveryAnimation;
        return (attackStartupTime, attackActiveTime, attackRecoveryTime, startupAnimation, activeAnimation,
            recoveryAnimation);
    }

    public void EnterStartup(AnimationClip startupAnimation) {
        if (skillState == SkillState.Ready) {
            hitCharacters.Clear();
            animator.Play(startupAnimation.name);
            skillState = SkillState.Startup;
            newStateStartAt = Time.time;
            attackStartTimestamp = Time.time;
        }
    }

    public void EnterActive(AnimationClip activeAnimation) {
        if (skillState == SkillState.Startup) {
            animator.Play(activeAnimation.name);
            skillState = SkillState.Active;
            newStateStartAt = Time.time;
        }
    }

    public void EnterRecovery(AnimationClip recoveryAnimation) {
        if (skillState == SkillState.Active) {
            animator.Play(recoveryAnimation.name);
            skillState = SkillState.Recovery;
            newStateStartAt = Time.time;
        }
    }

    public override void OnStateEnter() {
        skillState = SkillState.Ready;
        newStateStartAt = 0;
    }

    public void CheckAttackHit(CircleCollider2D hitboxCollider) {
        if (hitboxCollider == null) return;
        //get all colliders that are in the hitbox
        var collidedCharacters = Physics2D.OverlapCircleAll(hitboxCollider.bounds.center, hitboxCollider.radius);
        foreach (var character in collidedCharacters) {
            if (character.GetComponent<GameCharacter>() is not null) {
                var gameCharacter = character.GetComponent<GameCharacter>();
                if (gameCharacter == player) continue;
                if (hitCharacters.Contains(gameCharacter)) continue;
                player.OnSkillOrAttackHit(attackStatsList[attackMoveCount].damage, gameCharacter);
                hitCharacters.Add(gameCharacter);
            }
        }
    }

    public void AfterManAttackHit() { }

    public void AfterManAirAttackHit() { }

    public void AfterManWaterAttackHit() { }

    public bool GetAttackAnimationCancellable() {
        var cancelable = attackStatsList[attackMoveCount].cancelable;
        var attackCancelableAfter = attackStatsList[attackMoveCount].attackCancelableAfter;
        return cancelable && Time.time - attackStartTimestamp > attackCancelableAfter;
    }
}
