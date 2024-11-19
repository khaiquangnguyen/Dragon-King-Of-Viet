using System.Collections.Generic;
using UnityEngine;

public class DragonAttack : BasePlayerAction {
    public DragonAttack(Player player) : base(player, PlayerState.DragonAttack,
        PlayerForm.Dragon) {
        this.attackStatsList = player.playerStats.dragonAttackStats;
        animator = player.dragonAnimator;
    }

    public override void Update() {
        if (player.CheckChangeToDragonFloat()) return;
        if (player.CheckChangeToDragonFly()) return;
        if (player.CheckChangeToDragonDefenseState()) return;
    }

    public override void OnStateEnter() {
        // the attack count is only reset after a certain time has passed, to create a bit of a buffer even
        // when player enter attack after previous attack has ended
        if (Time.time - newStateStartAt >= player.playerStats.attackInputBufferPostAttackDuration)
            attackMoveCount = 0;
        else
            attackMoveCount = (attackMoveCount + 1) % attackStatsList.Count;
        newStateStartAt = 0;
        skillState = SkillState.Ready;
        player.UpdateVelocity(0, 0);
    }

    public override void FixedUpdate() {
        var (attackStartupTime, attackActiveTime, attackRecoveryTime, startupAnimation, activeAnimation,
            recoveryAnimation) = GetAttackTimingAndAnimation();

        if (skillState == SkillState.Ready) {
            player.attackInputBufferCountdown = -1;
            EnterStartup(startupAnimation, attackStartupTime);
        }
        else if (skillState == SkillState.Startup) {
            if (Time.time - newStateStartAt > attackStartupTime) {
                hitCharacters.Clear();
                EnterActive(activeAnimation, attackActiveTime);
            }
        }
        else if (skillState == SkillState.Active) {
            if (Time.time - newStateStartAt > attackActiveTime) {
                EnterRecovery(recoveryAnimation, attackRecoveryTime);
            }

            CheckAttackHit(player.dragonAttackCollider);
        }
        else if (skillState == SkillState.Recovery) {
            hitCharacters.Clear();
            if (Time.time - newStateStartAt < attackRecoveryTime) return;
            // once attack recovery is done and next attack input is ready, go to next attack
            if (player.attackInputBufferCountdown > 0 &&
                attackMoveCount < attackStatsList.Count - 1) {
                attackMoveCount++;
                skillState = SkillState.Ready;
                newStateStartAt = Time.time;
            }
            else {
                player.stateMachine.ChangeState(PlayerState.DragonIdle);
            }
        }

        if (player.environment == Environment.Ground)
            player.UpdateVelocity(0, 0);
        else
            player.UpdateVelocity(0, player.body.linearVelocity.y);
    }

    public override void OnStateExit() {
        player.transform.localRotation = Quaternion.identity;
    }
}