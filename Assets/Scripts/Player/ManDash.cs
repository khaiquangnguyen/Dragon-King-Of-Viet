using System;
using UnityEngine;

public class ManDash : PlayerStateBehavior {
    private float speedBeforeDash;
    private float dashTimestamp;
    private float dashDuration;
    private float dashSpeed;

    private Environment environment => player.environment;
    private PlayerStats playerStats => player.playerStats;

    public ManDash(Player player) : base(player, PlayerState.ManDash, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("Dash");
        player.inputDisabled = true;
        player.ResetEmpowermentAfterTrigger();
        speedBeforeDash = player.body.linearVelocity.x;
        // get parameters
        dashTimestamp = Time.time;
        dashDuration = player.playerStats.dashDuration;
        dashSpeed = player.playerStats.dashSpeed;
        player.UpdateVelocity(dashSpeed * player.facingDirection, 0);
    }

    public override void Update() { }

    public override void FixedUpdate() {
        if (Time.time - dashTimestamp < dashDuration) {
            player.UpdateVelocity(dashSpeed * player.facingDirection, 0);
        }
        // out of dash duration, set to false
        else {
            if (player.environment == Environment.Ground) {
                if (player.inputDirectionX == 0)
                    player.stateMachine.ChangeState(PlayerState.ManIdle);
                else
                    player.stateMachine.ChangeState(PlayerState.ManRun);
            }
            else if (player.environment == Environment.Air) {
                player.stateMachine.ChangeState(PlayerState.ManFall);
            }
        }
    }

    public override void OnStateExit() {
        player.inputDisabled = false;
        player.dashCooldownCountdown = player.dashCooldown;
        dashTimestamp = 0;
        var environmentMaxSpeed = environment switch {
            Environment.Ground => playerStats.manGroundMaxSpeed,
            Environment.Air => playerStats.manAirMaxSpeed,
            Environment.Water => playerStats.manWaterMaxSpeed,
            _ => throw new ArgumentOutOfRangeException()
        };
        var dashEndSpeed = Mathf.Max(environmentMaxSpeed, Mathf.Abs(speedBeforeDash)) * player.inputDirectionX;
        player.UpdateVelocityX(dashEndSpeed);
    }
}