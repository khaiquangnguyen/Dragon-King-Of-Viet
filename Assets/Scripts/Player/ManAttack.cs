using UnityEngine;

public class ManAttack : BasePlayerAction {
    public ManAttack(Player player) : base(player, PlayerState.ManAttack, PlayerForm.Man) {
        attackStatsList = player.playerStats.manAttackStats;
        animator = player.humanAnimator;
    }

    public override void Update() {
        if (player.CheckChangeToManRunState()) return;
        if (player.CheckChangeToManJumpOrEmpoweredJumpState()) return;
        if (player.CheckChangeToManDodgeHopDashState()) return;
        if (player.CheckChangeToManDefenseState()) return;
        if (player.CheckChangeToManAttackState()) return;
        if (player.CheckTransformIntoDragonAndBack()) return;
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
        player.ResetEmpowermentAfterTrigger();
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

            CheckAttackHit(player.manAttackCollider);
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
                player.stateMachine.ChangeState(PlayerState.ManIdle);
            }
        }

        if (player.environment == Environment.Ground)
            player.UpdateVelocity(0, 0);
        else
            player.UpdateVelocity(0, player.body.linearVelocity.y);
    }

    public void AfterManAirAttackHit() { }

    public void AfterManWaterAttackHit() { }

    public bool GetAttackAnimationCancellable() {
        var cancelable = attackStatsList[attackMoveCount].cancelable;
        var attackCancelableAfter = attackStatsList[attackMoveCount].attackCancelableAfter;
        return cancelable && Time.time - attackStartTimestamp > attackCancelableAfter;
    }
}