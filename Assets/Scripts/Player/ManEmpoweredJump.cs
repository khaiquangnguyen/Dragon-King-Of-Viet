using UnityEngine;


public class ManEmpoweredJump : ManJump {
    public ManEmpoweredJump(Player player) : base(player, PlayerState.ManEmpoweredJump) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("JumpRise");
        player.ResetEmpowermentAfterTrigger();
        jumpCutStarted = false;
        jumpMaxHeight = player.playerStats.empoweredJumpMaxHeight;
        jumpSpeed = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(player.playerStats.gravity));
        // begin jump
        jumpMoveY = jumpSpeed;
        jumpStartTimestamp = Time.time;
    }

    public override void Update() {
        // don't allow input change things during input lock
        if (Time.time - jumpStartTimestamp >= player.playerStats.empoweredJumpInputLockDuration) {
            if (player.CheckChangeToManJumpOrEmpoweredJumpState()) return;
            if (player.CheckChangeToManDodgeHopDashState()) return;
            if (player.CheckChangeToManDefenseState()) return;
            if (player.CheckChangeToManAttackState()) return;
            if (player.CheckTransformIntoDragonAndBack()) return;
        }
    }

    public override void FixedUpdate() {
        if (player.isJumpCut) {
            UpdateYDuringJumpCut(player.playerStats.empoweredJumpCutDuration, player.playerStats.empoweredJumpCutHeight,player.playerStats.empoweredJumpCutHeightCurve);
        }
        else {
            UpdateYDuringJump(player.playerStats.empoweredJumpDuration, jumpMaxHeight, player.playerStats.empoweredJumpHeightCurve);
        }
        player.characterController.Move(0, jumpMoveY);
    }

    public override void OnStateExit() { }

    public bool CanEmpoweredJumpCut() {
        var jumpHeightCurrentPercentage =
            player.playerStats.empoweredJumpHeightCurve.Evaluate((Time.time - jumpStartTimestamp) / player.playerStats.empoweredJumpDuration);
        return jumpHeightCurrentPercentage < player.playerStats.jumpPeakHangThreshold;
    }
}
