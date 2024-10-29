using System;
using UnityEngine;

public class ManRun : PlayerStateBehavior {
    private Rigidbody2D body => player.body;
    private int facingDirection => player.facingDirection;

    public ManRun(Player player) : base(player, PlayerState.ManRun, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("Run");
    }

    public override void FixedUpdate() {
        if (player.environment == Environment.Air) {
            player.stateMachine.ChangeState(PlayerState.ManFall);
        }
        // human x movement is dependent on the environment
        var acceleration = player.playerStats.manGroundAccel;
        var deceleration = player.playerStats.manGroundDecel;
        var maxSpeedX = player.playerStats.manGroundMaxSpeed;
        player.MoveX(acceleration, deceleration, maxSpeedX);
        if (Mathf.Approximately(player.body.linearVelocity.x, 0)) {
            player.stateMachine.ChangeState(PlayerState.ManIdle);
        }

    }

    public override void Update() { }
    public override void OnStateExit() { }
}
