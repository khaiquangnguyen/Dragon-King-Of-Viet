using System;
using UnityEngine;

public class ManDash : PlayerStateBehavior {
    private float speedBeforeDash;
    private float dashTimestamp;
    private float dashDuration;
    private float dashDistance;
    private Vector2 dashStartingPosition;
    private AnimationCurve dashDistanceCurve => player.playerStats.dashDistanceCurve;
    private int direction;

    private Environment environment => player.environment;
    private PlayerStats playerStats => player.playerStats;

    public ManDash(Player player) : base(player, PlayerState.ManDash, PlayerForm.Man) { }

    public override void OnStateEnter() {
        dashStartingPosition = player.body.position;
        player.inputDisabled = true;
        player.ResetEmpowermentAfterTrigger();
        speedBeforeDash = player.body.linearVelocity.x;
        // get parameters
        dashTimestamp = Time.time;
        dashDuration = player.playerStats.dashDuration;
        dashDistance = player.playerStats.dashDistance;
    }

    public void SetDirection(int newDirection) {
        this.direction = newDirection;
    }

    public override void Update() { }

    public override void FixedUpdate() {
        if (Time.time - dashTimestamp <= dashDuration) {
            var currentDashDistance =
                dashDistanceCurve.Evaluate((Time.time - dashTimestamp) / dashDuration) * dashDistance;
            player.characterController.MoveToX(currentDashDistance * direction + dashStartingPosition.x);
        }
        else {
            player.SetStateAfterMovement();
        }
    }

    public override void OnStateExit() {
        player.inputDisabled = false;
        player.dashCooldownCountdown = player.dashCooldown;
        dashTimestamp = 0;
        var environmentMaxSpeed = environment switch {
            Environment.Ground => playerStats.manGroundMaxSpeed,
            Environment.Air => playerStats.manAirMaxHSpeed,
            Environment.Water => playerStats.manWaterMaxSpeed,
            _ => throw new ArgumentOutOfRangeException()
        };
        var dashEndSpeed = Mathf.Max(environmentMaxSpeed, Mathf.Abs(speedBeforeDash)) * player.inputDirectionX;
        player.UpdateVelocityX(dashEndSpeed);
    }
}