using System;
using System.Linq;
using UnityEngine;

public class ManAttack : PlayerStateBehavior {
    private int attackMoveCount = 0;
    private SkillState skillState = SkillState.Ready;
    private float newStateStartAt;
    private float attackStartTimestamp;

    public ManAttack(Player player) : base(player, PlayerState.ManAttack, PlayerForm.Man) { }

    public override void OnStateEnter() {
        // the attack count is only reset after a certain time has passed, to create a bit of a buffer even
        // when player enter attack after previous attack has ended
        if (Time.time - newStateStartAt >= player.playerStats.attackInputBufferPostAttackDuration) {
            attackMoveCount = 0;
        }
        else {
            attackMoveCount = (attackMoveCount + 1 ) % player.playerStats.attackStats.Count;
        }
        newStateStartAt = 0;
        skillState = SkillState.Ready;
        player.ResetEmpowermentAfterTrigger();
        player.UpdateVelocity(0, 0);
    }

    public override void FixedUpdate() {
        var attackStartupTime = player.playerStats.attackStats[attackMoveCount].startupDuration;
        var attackActiveTime = player.playerStats.attackStats[attackMoveCount].activeDuration;
        var attackRecoveryTime = player.playerStats.attackStats[attackMoveCount].recoveryDuration;

        if (skillState == SkillState.Ready) {
            player.humanAnimator.Play("Attack_" + (attackMoveCount + 1) + "_Startup");
            player.attackInputBufferCountdown = -1;
            skillState = SkillState.Startup;
            newStateStartAt = Time.time;
            attackStartTimestamp = Time.time;
        }
        else if (skillState == SkillState.Startup) {
            if (Time.time - newStateStartAt > attackStartupTime) {
                player.humanAnimator.Play("Attack_" + (attackMoveCount + 1) + "_Active");
                skillState = SkillState.Active;
                newStateStartAt = Time.time;
            }
        }
        else if (skillState == SkillState.Active) {
            if (Time.time - newStateStartAt > attackActiveTime) {
                player.humanAnimator.Play("Attack_" + (attackMoveCount + 1) + "_Recovery");
                skillState = SkillState.Recovery;
                newStateStartAt = Time.time;
            }
        }
        else if (skillState == SkillState.Recovery) {
            if (Time.time - newStateStartAt < attackRecoveryTime) return;
            // once attack recovery is done and next attack input is ready, go to next attack
            if (player.attackInputBufferCountdown > 0 &&
                attackMoveCount < player.playerStats.attackStats.Count - 1) {
                attackMoveCount++;
                skillState = SkillState.Ready;
                newStateStartAt = Time.time;
            }
            else {
                player.stateMachine.ChangeState(PlayerState.ManIdle);
            }
        }

        if (player.environment == Environment.Ground)
            player.UpdateVelocity(0, 0);
        else
            player.UpdateVelocity(0, player.body.linearVelocity.y);
    }

    public void End() { }

    public bool GetAttackAnimationCancellable() {
        var cancelable = player.playerStats.attackStats[attackMoveCount].cancelable;
        var attackCancelableAfter = player.playerStats.attackStats[attackMoveCount].attackCancelableAfter;
        return cancelable && Time.time - attackStartTimestamp > attackCancelableAfter;
    }
}
