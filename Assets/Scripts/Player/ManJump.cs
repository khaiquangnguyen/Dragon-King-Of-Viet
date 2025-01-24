using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum JumpModifiers {
    JumpPlatform
}

public class ManJump : PlayerStateBehavior {
    private readonly Dictionary<JumpModifiers, bool> jumpModifiers = new(Enum.GetValues(typeof(JumpModifiers))
        .Cast<JumpModifiers>()
        .ToDictionary(state => state, _ => false));

    public float jumpStartTimestamp;
    public float jumpMaxHeight;
    public float jumpSpeed;
    public float jumpPeakHangDuration;
    public float jumpPeakHangThreshold;
    public bool bumpHead;
    public float jumpMoveY;
    protected bool jumpCutStarted;
    public float jumpCutTimestamp;

    public ManJump(Player player, PlayerState state = PlayerState.ManJump) : base(player, state, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.characterController.LeaveGround();
        player.humanAnimator.Play("JumpRise");
        jumpCutStarted = false;
        jumpMaxHeight = player.playerStats.jumpMaxHeight;
        jumpSpeed = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(player.playerStats.gravity));
        // begin jump
        jumpMoveY = jumpSpeed;
        jumpStartTimestamp = Time.time;
    }

    public override void Update() {
        if (player.CheckChangeToManJumpOrEmpoweredJumpState()) return;
        if (player.CheckChangeToManShortDashState()) return;
        if (player.CheckChangeToManDefenseState()) return;
        if (player.CheckChangeToManAttackState()) return;
        if (player.CheckChangeToManDragonCastSpell()) return;
        if (player.CheckTransformIntoDragonAndBack()) return;
    }

    public void UpdateYDuringJumpCut(float jumpCutDuration, float jumpCutHeight, AnimationCurve heightCurve) {
        if (!jumpCutStarted) {
            jumpCutStarted = true;
            jumpCutTimestamp = Time.time;
            player.humanAnimator.Play("JumpMid");
        }

        var jumpHeightCurrentPercentage =
            heightCurve.Evaluate((Time.time - jumpCutTimestamp) / jumpCutDuration);
        var jumpHeightNextPercentage =
            heightCurve.Evaluate((Time.time - jumpStartTimestamp + Time.fixedDeltaTime) / jumpCutDuration);
        var jumpHeightDelta = (jumpHeightNextPercentage - jumpHeightCurrentPercentage) * jumpCutHeight;
        jumpMoveY = jumpHeightDelta / Time.fixedDeltaTime;
        if (Time.time - jumpCutTimestamp > jumpCutDuration)
            player.stateMachine.ChangeState(PlayerState.ManFall);
    }

    public void UpdateYDuringJump(float jumpDuration, float jumpHeight, AnimationCurve heightCurve) {
        var jumpHeightCurrentPercentage =
            heightCurve.Evaluate((Time.time - jumpStartTimestamp) / jumpDuration);
        var jumpHeightNextPercentage = heightCurve.Evaluate((Time.time - jumpStartTimestamp + Time.fixedDeltaTime) /
                                                            jumpDuration);
        var jumpHeightDelta = (jumpHeightNextPercentage - jumpHeightCurrentPercentage) * jumpHeight;
        jumpMoveY = jumpHeightDelta / Time.fixedDeltaTime;
        if (jumpHeightCurrentPercentage > 0.65) player.humanAnimator.Play("JumpMid");
        if (Time.time - jumpStartTimestamp > jumpDuration)
            player.stateMachine.ChangeState(PlayerState.ManFall);
    }

    public override void FixedUpdate() {
        var acceleration = player.playerStats.manAirAccel;
        var deceleration = player.playerStats.manAirDecel;
        var maxSpeedX = player.playerStats.manAirMaxHSpeed * Mathf.Abs(player.inputDirectionX);
        var accelerationFactor = Mathf.Abs(player.characterController.velocity.x) > maxSpeedX
            ? acceleration
            : deceleration;
        var jumpMoveX = Mathf.MoveTowards(Mathf.Abs(player.characterController.velocity.x), maxSpeedX,
            accelerationFactor * Time.fixedDeltaTime) * player.facingDirection; // jump cut
        if (player.isJumpCut) {
            UpdateYDuringJumpCut(player.playerStats.jumpCutDuration, player.playerStats.jumpCutHeight,
                player.playerStats.jumpCutHeightCurve);
        }
        else {
            UpdateYDuringJump(player.playerStats.jumpDuration, jumpMaxHeight, player.playerStats.jumpHeightCurve);
        }

        player.characterController.Move(jumpMoveX, jumpMoveY);
    }

    public override void OnStateExit() { }

    public bool CanJumpCut() {
        var jumpHeightCurrentPercentage =
            player.playerStats.jumpHeightCurve.Evaluate((Time.time - jumpStartTimestamp) /
                                                        player.playerStats.jumpDuration);
        return jumpHeightCurrentPercentage < player.playerStats.jumpPeakHangThreshold;
    }
}
