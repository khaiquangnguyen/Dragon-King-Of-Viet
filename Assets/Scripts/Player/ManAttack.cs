using System;
using System.Collections.Generic;
using System.Linq;
using CharacterBehavior;
using UnityEngine;

public class ManAttack : PlayerStateBehavior {
    private int attackMoveCount = 0;
    private SkillState skillState = SkillState.Ready;
    private float newStateStartAt;
    private float attackStartTimestamp;
    public ManAttack(Player player) : base(player, PlayerState.ManAttack, PlayerForm.Man) { }
    public List<GameCharacter> hitCharacters = new List<GameCharacter>();

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

        if (skillState == SkillState.Ready) {
            hitCharacters.Clear();
            player.humanAnimator.Play("Attack_" + (attackMoveCount + 1) + "_Startup");
            player.attackInputBufferCountdown = -1;
            skillState = SkillState.Startup;
            newStateStartAt = Time.time;
            attackStartTimestamp = Time.time;
        }
        else if (skillState == SkillState.Startup) {
            hitCharacters.Clear();
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
                player.playerStateMachine.ChangeState(PlayerState.ManIdle);
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
        var collidedCharacters = Physics2D.OverlapBoxAll(hitboxCollider.bounds.center, hitboxCollider.bounds.size, 0);
        foreach (var character in collidedCharacters) {
            if (character.GetComponent<GameCharacter>() is not null) {
                var gameCharacter = character.GetComponent<GameCharacter>();
                if (gameCharacter == player) continue;
                if (hitCharacters.Contains(gameCharacter)) continue;
                player.OnSkillOrAttackHit(player.playerStats.attackStats[attackMoveCount].damage, gameCharacter);
                hitCharacters.Add(gameCharacter);
            }
            // if (gameCharacter == player) continue;
            // if (gameCharacter.isInvulnerable) continue;
            // if (gameCharacter.isDead) continue;
            // if (gameCharacter.isImmune) continue;
            // if (gameCharacter.isStunned) continue;
            // if (gameCharacter.isKnockedBack) continue;
            // if (gameCharacter.isKnockedUp) continue;
            // if (gameCharacter.isSilenced) continue;
            // if (gameCharacter.isFeared) continue;
            // if (gameCharacter.isCharmed) continue;
            // if (gameCharacter.isTaunted) continue;
            // if (gameCharacter.isRooted) continue;
            // if (gameCharacter.isDisarmed) continue;
            // if (gameCharacter.isBlind) continue;
            // if (gameCharacter.isSlowed) continue;
            // if (gameCharacter.isStasis) continue;
            // if (gameCharacter.isAirborne) continue;
            // if (gameCharacter.isGrounded) continue;
            // if (gameCharacter.isCasting) continue;
            // if (gameCharacter.isChanneling) continue;
            // if (gameCharacter.isDodging) continue;
            // if (gameCharacter.isDashing) continue;
            // if (gameCharacter.isTeleporting) continue;
            // if (gameCharacter.isInvisible) continue;
            // if (gameCharacter.isStealthed) continue;
            // if (gameCharacter.isRevealed) continue;
            // if (gameCharacter.isDisguised) continue;
            // if (gameCharacter.isTransformed) continue;
            // if (gameCharacter.isMounted) continue;
            // if (gameCharacter.isFlying) continue;
            // if (gameCharacter.isHovering) continue;
            // if (gameCharacter.isGrounded) continue;
            // if (gameCharacter.isUnderground) continue;
            // if (gameCharacter.isUnderwater) continue;
            // if (gameCharacter.isSubmerged) continue;
            // if (gameCharacter.isSwimming) continue;
            // if (gameCharacter.isBurning) continue;
            // if (gameCharacter.isBleeding) continue;
            // if (gameCharacter.isPoisoned) continue;
            // if (gameCharacter.isCrippled) continue;
            // if (gameCharacter.isSlowed) continue;
            // if (gameCharacter.isStunned) continue;
            // if (gameCharacter.isSilenced) continue;
        }
    }

    public void End() { }

    public bool GetAttackAnimationCancellable() {
        var cancelable = player.playerStats.attackStats[attackMoveCount].cancelable;
        var attackCancelableAfter = player.playerStats.attackStats[attackMoveCount].attackCancelableAfter;
        return cancelable && Time.time - attackStartTimestamp > attackCancelableAfter;
    }
}