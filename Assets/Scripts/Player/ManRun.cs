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
        if (Mathf.Approximately(player.characterController.velocity.magnitude, 0))
            player.stateMachine.ChangeState(PlayerState.ManIdle);
        var acceleration = player.playerStats.manGroundAccel;
        var deceleration = player.playerStats.manGroundDecel;
        var maxSpeed = Mathf.Abs(player.playerStats.manGroundMaxSpeed * player.inputDirectionX);
        if (player.environment == Environment.Air) player.stateMachine.ChangeState(PlayerState.ManFall);
        // human x movement is dependent on the environment
        player.characterBaseMovement.RunOnGround(acceleration, deceleration, maxSpeed, facingDirection);
    }

    public override void Update() { }
    public override void OnStateExit() { }
}
