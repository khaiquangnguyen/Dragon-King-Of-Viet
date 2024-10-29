using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum JumpModifiers {
    DragonEmpowered,
    JumpPlatform
}

public class ManJump : PlayerStateBehavior{
    private readonly Dictionary<JumpModifiers, bool> jumpModifiers = new(Enum.GetValues(typeof(JumpModifiers))
        .Cast<JumpModifiers>()
        .ToDictionary(state => state, _ => false));

    private float jumpTimestamp;
    private float jumpMaxHeight;
    private float jumpSpeed;
    private float jumpPeakHangDuration;
    private float jumpPeakHangThreshold;
    private bool bumpHead;
    private float jumpMoveY;
    private bool jumpCutStarted;
    private float jumpCutTimestamp;
    private AnimationCurve jumpHeightCurve => player.playerStats.jumpHeightCurve;
    private AnimationCurve jumpCutHeightCurve => player.playerStats.jumpCutHeightCurve;


    public ManJump(Player player) : base(player, PlayerState.ManJump, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("JumpRise");
        // calculate jump parameters
        // consume the empowered effect if any is available
        if (player.isEmpowering) {
            jumpModifiers[JumpModifiers.DragonEmpowered] = true;
            player.ResetEmpowermentAfterTrigger();
        }
        jumpCutStarted = false;
        jumpMaxHeight = jumpModifiers[JumpModifiers.DragonEmpowered]
            ? player.playerStats.powerJumpMaxHeight
            : player.playerStats.jumpMaxHeight;
        jumpSpeed = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(player.playerStats.gravity));
        // begin jump
        jumpModifiers[JumpModifiers.DragonEmpowered] = false;
        jumpMoveY = jumpSpeed;
        jumpTimestamp = Time.time;
    }

    public override void Update() {
    }

    public override void FixedUpdate() {
        // moveX
        var acceleration = player.playerStats.manAirAccel;
        var deceleration = player.playerStats.manAirDecel;
        var maxSpeedX = player.playerStats.manAirMaxSpeed;
        player.MoveX(acceleration, deceleration, maxSpeedX);
        // jump cut
        if (player.isJumpCut) {
            if (!jumpCutStarted) {
                jumpCutStarted = true;
                jumpCutTimestamp = Time.time;
                player.humanAnimator.Play("JumpMid");
            }
            var jumpHeightCurrentPercentage = jumpCutHeightCurve.Evaluate((Time.time - jumpCutTimestamp) / player.playerStats.jumpCutDuration);
            var jumpHeightNextPercentage = jumpCutHeightCurve.Evaluate((Time.time - jumpTimestamp + Time.fixedDeltaTime) / player.playerStats.jumpCutDuration);
            var jumpHeightDelta = (jumpHeightNextPercentage - jumpHeightCurrentPercentage) * player.playerStats.jumpCutHeight;
            jumpMoveY = jumpHeightDelta / Time.fixedDeltaTime;
            if (Time.time - jumpCutTimestamp > player.playerStats.jumpCutDuration) player.stateMachine.ChangeState(PlayerState.ManFall);
        }
        else {
            var jumpHeightCurrentPercentage = jumpHeightCurve.Evaluate((Time.time - jumpTimestamp) / player.playerStats.jumpDuration);
            var jumpHeightNextPercentage = jumpHeightCurve.Evaluate((Time.time - jumpTimestamp + Time.fixedDeltaTime) / player.playerStats.jumpDuration);
            var jumpHeightDelta = (jumpHeightNextPercentage - jumpHeightCurrentPercentage) * jumpMaxHeight;
            jumpMoveY = jumpHeightDelta / Time.fixedDeltaTime;
            if (jumpHeightCurrentPercentage > 0.65) {
                player.humanAnimator.Play("JumpMid");
            }
            if (Time.time - jumpTimestamp > player.playerStats.jumpDuration) {
                player.stateMachine.ChangeState(PlayerState.ManFall);
            }

        }
        player.UpdateVelocityY(jumpMoveY);
    }

    public override void OnStateExit() {
    }

    public bool CanJumpCut() {
        var jumpHeightCurrentPercentage = jumpHeightCurve.Evaluate((Time.time - jumpTimestamp) / player.playerStats.jumpDuration);
        return jumpHeightCurrentPercentage < player.playerStats.jumpPeakHangThreshold;
    }
}
