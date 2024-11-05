using System;
using UnityEngine;

public class ManRun : PlayerStateBehavior {
    private int facingDirection => player.facingDirection;

    public ManRun(Player player) : base(player, PlayerState.ManRun, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("Run");
    }

    public override void FixedUpdate() {
        player.humanAnimator.Play("Run");
        if (Mathf.Approximately(player.characterController.velocity.magnitude, 0))
            player.playerStateMachine.ChangeState(PlayerState.ManIdle);
        var acceleration = player.playerStats.manGroundAccel;
        var deceleration = player.playerStats.manGroundDecel;
        var maxSpeed = Mathf.Abs(player.playerStats.manGroundMaxSpeed * player.inputDirectionX);
        if (!player.characterController.isOnGround())
            player.playerStateMachine.ChangeState(PlayerState.ManFall);
        else
            player.characterController.MoveAlongGround(acceleration, deceleration, maxSpeed, facingDirection);
    }

    public override void Update() { }
    public override void OnStateExit() { }
}