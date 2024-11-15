using System.Collections.Generic;
using UnityEngine;

public class DragonSwipe : BasePlayerAction {
    public DragonSwipe(Player player, List<AttackStats> attackStatsList) : base(player, PlayerState.DragonAttack,
        PlayerForm.Dragon) {
        this.attackStatsList = attackStatsList;
    }

    public override void OnStateEnter() {
    }

    public override void FixedUpdate() {
        var (attackStartupTime, attackActiveTime, attackRecoveryTime, startupAnimation, activeAnimation,
            recoveryAnimation) = GetAttackTimingAndAnimation();

        if (skillState == SkillState.Ready) {
            player.attackInputBufferCountdown = -1;
            EnterStartup(startupAnimation);
        }
        else if (skillState == SkillState.Startup) {
            if (Time.time - newStateStartAt > attackStartupTime) {
                hitCharacters.Clear();
                EnterActive(activeAnimation);
            }
        }
        else if (skillState == SkillState.Active) {
            if (Time.time - newStateStartAt > attackActiveTime) {
                EnterRecovery(recoveryAnimation);
            }

            CheckAttackHit(player.dragonAttackCollider);
        }
        else if (skillState == SkillState.Recovery) {
            hitCharacters.Clear();
            if (Time.time - newStateStartAt < attackRecoveryTime) return;
            // once attack recovery is done and next attack input is ready, go to next attack
            if (player.attackInputBufferCountdown > 0 &&
                attackMoveCount < player.playerStats.manAttackStats.Count - 1) {
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

    public override void OnStateExit() {
        player.transform.localRotation = Quaternion.identity;
    }
}
