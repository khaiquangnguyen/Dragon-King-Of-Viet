using UnityEngine;

public class ManDodge : PlayerStateBehavior {
    private float speedBeforeShortDash;
    private float hopTimestamp;
    private float hopeDuration;
    private float hopDistance;
    private float hopSpeed;
    private AnimationCurve shortDashDistanceCurve => player.playerStats.shortDashDistanceCurve;
    private Vector2 hopStartingPosition;
    private int direction;
    private Environment environment => player.environment;
    private PlayerStats playerStats => player.playerStats;

    public ManDodge(Player player) : base(player, PlayerState.ManShortDash, PlayerForm.Man) { }

    public override void OnStateEnter() {
        hopStartingPosition = player.characterController.body.position;
        player.inputDisabled = true;
        speedBeforeShortDash = player.characterController.velocity.x;
        // get parameters
        hopTimestamp = Time.time;
        hopeDuration = player.playerStats.shortDashDuration;
        hopDistance = player.playerStats.shortDashDistance;
    }

    public void SetDirection(int dodgeDirection) {
        this.direction = dodgeDirection;
    }

    public override void Update() { }

    public override void FixedUpdate() {
        if (Time.time - hopTimestamp <= hopeDuration) {
            var currentHopDistance =
                shortDashDistanceCurve.Evaluate((Time.time - hopTimestamp) / hopeDuration) * hopDistance;
            MonoBehaviour.print(currentHopDistance);
            player.characterController.MoveToX(currentHopDistance * direction + hopStartingPosition.x);
        }
        else {
            player.SetStateAfterMovement();
        }
    }

    public override void OnStateExit() {
        player.inputDisabled = false;
        player.dashCooldownCountdown = player.dashCooldown;
        hopTimestamp = 0;
        var environmentMaxSpeed = environment switch {
            Environment.Ground => playerStats.manGroundMaxSpeed,
            Environment.Air => playerStats.manAirMaxHSpeed,
            Environment.Water => playerStats.manWaterMaxSpeed,
        };
        var hopEndSpeed = Mathf.Max(environmentMaxSpeed, Mathf.Abs(speedBeforeShortDash)) * player.inputDirectionX;
        player.characterController.MoveX(hopEndSpeed);
    }
}