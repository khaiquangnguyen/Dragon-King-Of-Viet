using UnityEngine;

public class ManRun : PlayerStateBehavior {
    private int facingDirection => player.facingDirection;

    public ManRun(Player player) : base(player, PlayerState.ManRun, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("Run");
    }

    public override void Update() {
        if (player.CheckChangeToManJumpOrEmpoweredJumpState()) return;
        if (player.CheckChangeToManShortDashState()) return;
        if (player.CheckChangeToManDefenseState()) return;
        if (player.CheckChangeToManAttackState()) return;
        if (player.CheckTransformIntoDragonAndBack()) return;
        if (player.CheckChangeToManDragonCastSpell()) return;
    }

    public override void FixedUpdate() {
        // if (!player.characterController.CheckIsOnGround()) return;
        player.humanAnimator.Play("Run");
        var acceleration = player.playerStats.manGroundAccel;
        var deceleration = player.playerStats.manGroundDecel;
        var maxSpeed = Mathf.Abs(player.playerStats.manGroundMaxSpeed * player.inputDirectionX);
        player.characterController.MoveAlongGround(acceleration, deceleration, maxSpeed, facingDirection);
        if (player.CheckChangeToManIdle()) return;
        if (player.CheckChangeToManFall()) return;
    }

    public override void OnStateExit() { }
}
