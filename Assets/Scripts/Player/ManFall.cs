using UnityEngine;
using System;

public class ManFall : PlayerStateBehavior {
    public ManFall(Player player) : base(player, PlayerState.ManFall, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("JumpFall");
    }

    public override void FixedUpdate() {
        var acceleration = player.playerStats.manAirAccel;
        var deceleration = player.playerStats.manAirDecel;
        var maxSpeedX = player.playerStats.manAirMaxSpeed;
        var gravityMult = player.GetGravityMult();
        player.characterBaseMovement.Fall(acceleration, deceleration, maxSpeedX, player.playerStats.gravity,
            gravityMult);
    }

    public override void OnStateExit() {
        base.OnStateExit();
    }
}
