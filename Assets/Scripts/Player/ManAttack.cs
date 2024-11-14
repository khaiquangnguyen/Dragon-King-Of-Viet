using System.Collections.Generic;
using CharacterBehavior;
using UnityEngine;

public class ManAttack : PlayerStateBehavior {
    private int attackMoveCount = 0;
    private SkillState skillState = SkillState.Ready;
    private float newStateStartAt;
    private float attackStartTimestamp;
    public ManAttack(Player player) : base(player, PlayerState.ManAttack, PlayerForm.Man) { }
    private readonly List<GameCharacter> hitCharacters = new();

    public override void OnStateEnter() {
        // the attack count is only reset after a certain time has passed, to create a bit of a buffer even
        // when player enter attack after previous attack has ended
        if (Time.time - newStateStartAt >= player.playerStats.attackInputBufferPostAttackDuration)
            attackMoveCount = 0;
        else
            attackMoveCount = (attackMoveCount + 1) % player.playerStats.attackStats.Count;
        newStateStartAt = 0;
        skillState = SkillState.Ready;
        player.ResetEmpowermentAfterTrigger();
        player.UpdateVelocity(0, 0);
    }

    public override void FixedUpdate() {
        var attackStartupTime = player.playerStats.attackStats[attackMoveCount].startupDuration;
        var attackActiveTime = player.playerStats.attackStats[attackMoveCount].activeDuration;
        var attackRecoveryTime = player.playerStats.attackStats[attackMoveCount].recoveryDuration;
        var startupAnimation = player.playerStats.attackStats[attackMoveCount].startupAnimation;
        var activeAnimation = player.playerStats.attackStats[attackMoveCount].activeAnimation;
        var recoveryAnimation = player.playerStats.attackStats[attackMoveCount].recoveryAnimation;

        if (skillState == SkillState.Ready) {
            hitCharacters.Clear();
            player.humanAnimator.Play(startupAnimation.name);
            player.attackInputBufferCountdown = -1;
            skillState = SkillState.Startup;
            newStateStartAt = Time.time;
            attackStartTimestamp = Time.time;
        }
        else if (skillState == SkillState.Startup) {
            hitCharacters.Clear();
            if (Time.time - newStateStartAt > attackStartupTime) {
                player.humanAnimator.Play(activeAnimation.name);
                skillState = SkillState.Active;
                newStateStartAt = Time.time;
            }
        }
        else if (skillState == SkillState.Active) {
            if (Time.time - newStateStartAt > attackActiveTime) {
                player.humanAnimator.Play(recoveryAnimation.name);
                skillState = SkillState.Recovery;
                newStateStartAt = Time.time;
            }
            CheckAttackHit();
        }
        else if (skillState == SkillState.Recovery) {
            hitCharacters.Clear();
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

    public void CheckAttackHit() {
        var hitboxCollider = player.manAttackCollider;
        if (hitboxCollider == null) return;
        //get all colliders that are in the hitbox
        var collidedCharacters = Physics2D.OverlapCircleAll(hitboxCollider.bounds.center, hitboxCollider.radius);
        foreach (var character in collidedCharacters) {
            if (character.GetComponent<GameCharacter>() is not null) {
                var gameCharacter = character.GetComponent<GameCharacter>();
                if (gameCharacter == player) continue;
                if (hitCharacters.Contains(gameCharacter)) continue;
                player.OnSkillOrAttackHit(player.playerStats.attackStats[attackMoveCount].damage, gameCharacter);
                hitCharacters.Add(gameCharacter);
            }
        }
    }

    public void End() { }

    public bool GetAttackAnimationCancellable() {
        var cancelable = player.playerStats.attackStats[attackMoveCount].cancelable;
        var attackCancelableAfter = player.playerStats.attackStats[attackMoveCount].attackCancelableAfter;
        return cancelable && Time.time - attackStartTimestamp > attackCancelableAfter;
    }
}
