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
        if (Mathf.Approximately(player.humanController.velocity.magnitude, 0))
            player.stateMachine.ChangeState(PlayerState.ManIdle);
        var acceleration = player.playerStats.manGroundAccel;
        var deceleration = player.playerStats.manGroundDecel;
        var maxSpeed = Mathf.Abs(player.playerStats.manGroundMaxSpeed * player.inputDirectionX);
        // greater than max speed and moving in the same direction, then decelerate
        var accelerationFactor = body.linearVelocity.magnitude > maxSpeed
            ? deceleration
            : acceleration;
        var maxCurrentVelocity = Mathf.MoveTowards(body.linearVelocity.magnitude, maxSpeed,
            accelerationFactor) * facingDirection;
        if (player.environment == Environment.Air) player.stateMachine.ChangeState(PlayerState.ManFall);
        // human x movement is dependent on the environment
        player.humanController.MoveAlongGround(maxCurrentVelocity);
        // simulated gravity on slope
    }

    public override void Update() { }
    public override void OnStateExit() { }
}
