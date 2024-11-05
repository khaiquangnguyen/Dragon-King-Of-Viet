using UnityEngine;
using System;

public class ManFall : PlayerStateBehavior {
    public ManFall(Player player) : base(player, PlayerState.ManFall, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("JumpFall");
    }

    public override void FixedUpdate() {
        // moveX
        var acceleration = player.playerStats.manAirAccel;
        var deceleration = player.playerStats.manAirDecel;
        var maxSpeedX = player.playerStats.manAirMaxSpeed;
        // player.MoveX(acceleration, deceleration, maxSpeedX);
        // vY
        var velocityY = player.humanController.velocity.y;
        var gravityMult = player.GetGravityMult();
        velocityY += player.playerStats.gravity * gravityMult * Time.fixedDeltaTime;
        player.humanController.MoveOnNonGround(0, velocityY);
        if (player.humanController.isOnWalkableGround()) {
            if (player.inputDirectionX == 0)
                player.stateMachine.ChangeState(PlayerState.ManIdle);
            else
                player.stateMachine.ChangeState(PlayerState.ManRun);
        }
        // if (player.environment == Environment.Ground) {
        //     if (player.inputDirectionX == 0) {
        //         player.humanController.Move(0,0);
        //         player.stateMachine.ChangeState(PlayerState.ManIdle);
        //     }
        //     else
        //         player.stateMachine.ChangeState(PlayerState.ManRun);
        // }
    }

    public override void OnStateExit() {
        base.OnStateExit();
    }
}
