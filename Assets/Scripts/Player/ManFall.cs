using UnityEngine;

public class ManFall : PlayerStateBehavior {
    public ManFall(Player player) : base(player, PlayerState.ManFall, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("JumpFall");
    }

    public override void Update() {
        if (player.CheckChangeToManJumpOrEmpoweredJumpState()) return;
        if (player.CheckChangeToManDodgeHopDashState()) return;
        if (player.CheckChangeToManDefenseState()) return;
        if (player.CheckChangeToManAttackState()) return;
        if (player.CheckTransformIntoDragonAndBack()) return;
    }
    public override void FixedUpdate() {
        var acceleration = player.playerStats.manAirAccel;
        var deceleration = player.playerStats.manAirDecel;
        var maxSpeedX = player.playerStats.manAirMaxSpeed * Mathf.Abs(player.inputDirectionX);
        var gravityMult = player.GetGravityMult();
        player.characterController.MoveOnNonGroundHorizontal(acceleration, deceleration, maxSpeedX,
            player.playerStats.gravity, gravityMult, player.facingDirection, player.playerStats.maxFallSpeed);
        if (player.characterController.isOnWalkableGround()) {
            if (player.inputDirectionX == 0) {
                player.characterController.Move(0, 0);
                player.stateMachine.ChangeState(PlayerState.ManIdle);
            }
            else {
                player.characterController.Move(maxSpeedX, 0);
                player.stateMachine.ChangeState(PlayerState.ManRun);
            }
        }
    }

    public override void OnStateExit() {
        base.OnStateExit();
    }
}
