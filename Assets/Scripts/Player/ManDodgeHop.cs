using System;
using UnityEngine;

public class ManDodge : PlayerStateBehavior {
    private float speedBeforeDodgeHop;
    private float hopTimestamp;
    private float hopeDuration;
    private float hopDistance;
    private float hopSpeed;
    private AnimationCurve dodgeHopDistanceCurve => player.playerStats.dodgeHopDistanceCurve;
    private Vector2 hopStartingPosition;
    private int direction;


    private Environment environment => player.environment;
    private PlayerStats playerStats => player.playerStats;

    public ManDodge(Player player) : base(player, PlayerState.ManDodgeHop, PlayerForm.Man) { }

    public override void OnStateEnter() {
        hopStartingPosition = player.characterController.body.position;
        player.inputDisabled = true;
        speedBeforeDodgeHop = player.characterController.velocity.x;
        // get parameters
        hopTimestamp = Time.time;
        hopeDuration = player.playerStats.dodgeHopDuration;
        hopDistance = player.playerStats.dodgeHopDistance;
        hopSpeed = hopDistance / hopeDuration;
        player.characterController.Move(hopSpeed * direction, 0);
    }

    public void SetDodgeDirection(int dodgeDirection) {
        this.direction = dodgeDirection;
    }

    public override void Update() { }

    public override void FixedUpdate() {
        if (Time.time - hopTimestamp <= hopeDuration) {
            var currentHopDistance = dodgeHopDistanceCurve.Evaluate((Time.time - hopTimestamp) / hopeDuration) * hopDistance;
            player.characterController.MoveToX(currentHopDistance * direction + hopStartingPosition.x);
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
        hopTimestamp = 0;
        var environmentMaxSpeed = environment switch {
            Environment.Ground => playerStats.manGroundMaxSpeed,
            Environment.Air => playerStats.manAirMaxSpeed,
            Environment.Water => playerStats.manWaterMaxSpeed,
        };
        var hopEndSpeed = Mathf.Max(environmentMaxSpeed, Mathf.Abs(speedBeforeDodgeHop)) * player.inputDirectionX;
        player.characterController.MoveX(hopEndSpeed);
    }
}
