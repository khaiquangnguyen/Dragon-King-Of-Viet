using UnityEngine;

public class ManFall : PlayerStateBehavior {
    public ManFall(Player player) : base(player, PlayerState.ManFall, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("JumpFall");
    }

    public override void Update() {
        if (player.CheckChangeToWallHang()) return;
        if (player.CheckChangeToManJumpOrEmpoweredJumpState()) return;
        if (player.CheckChangeToManShortDashState()) return;
        if (player.CheckChangeToManDefenseState()) return;
        if (player.CheckChangeToManAttackState()) return;
        if (player.CheckTransformIntoDragonAndBack()) return;
    }

    public override void FixedUpdate() {
        // when falling, can't stick to ground
        player.characterController.shouldStickToGround = false;
        var acceleration = player.playerStats.manAirAccel;
        var deceleration = player.playerStats.manAirDecel;
        var maxSpeedX = player.playerStats.manAirMaxHSpeed * Mathf.Abs(player.inputDirectionX);
        var gravityMult = player.GetGravityMult();
        player.characterController.MoveOnNonGroundHorizontalWithGravity(acceleration, deceleration, maxSpeedX,
            player.playerStats.gravity, gravityMult, player.facingDirection, player.playerStats.maxVerticalSpeed);
        if (player.characterController.CheckIsOnGround()) {
            player.characterController.shouldStickToGround = true;
            if (player.inputDirectionX == 0) {
                player.stateMachine.ChangeState(PlayerState.ManIdle);
            }
            else {
                player.stateMachine.ChangeState(PlayerState.ManRun);
            }
        }
    }

    public override void OnStateExit() {
        base.OnStateExit();
    }
}
