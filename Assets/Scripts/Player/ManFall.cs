using UnityEngine;
using System;

public class ManFall : PlayerStateBehavior {
    public ManFall(Player player) : base(player, CharacterState.ManFall, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("JumpFall");
    }

    public override void FixedUpdate() {
        // moveX
        var acceleration = player.playerStats.manAirAccel;
        var deceleration = player.playerStats.manAirDecel;
        var maxSpeedX = player.playerStats.manAirMaxSpeed;
        player.MoveX(acceleration, deceleration, maxSpeedX);
        // vY
        var moveY = player.body.linearVelocity.y;
        var gravityMult = player.GetGravityMult();
        moveY += player.playerStats.gravity * gravityMult * Time.fixedDeltaTime;
        player.UpdateVelocityY(moveY);
        if (player.environment == Environment.Ground) {
            if (player.inputDirectionX == 0)
                player.stateMachine.ChangeState(CharacterState.ManIdle);
            else
                player.stateMachine.ChangeState(CharacterState.ManRun);
        }
    }

    public override void OnStateExit() {
        base.OnStateExit();
    }
}
