using UnityEngine;

public class DragonFloatMove : PlayerStateBehavior {
    public DragonFloatMove(Player player) : base(player, PlayerState.DragonFloatMove, PlayerForm.Dragon) { }
    public float dragonHoverBufferCountdown;

    public override void OnStateEnter() {
        player.dragonMaxSpeed = player.playerStats.dragonMaxSpeed;
        player.dragonAnimator.Play(player.playerStats.dragonFloatMoveAnimation.name);
        player.dragonAnimator.speed = 1;
    }

    public override void FixedUpdate() {
        // human x movement is dependent on the environment
        var acceleration = player.playerStats.dragonAccel;
        var deceleration = player.playerStats.dragonDecel;
        var inputDirection = new Vector2(player.inputDirectionX, player.inputDirectionY);
        var normalizedInputDirection = inputDirection.normalized;
        var direction = normalizedInputDirection;
        // rotate current direction toward input direction at a certain rate
        player.characterController.MoveOnNonGroundAnyDirectionNoGravity(acceleration, deceleration,
            player.playerStats.dragonMaxSpeed, direction);
        if (player.characterController.velocity.magnitude < 0.05f) {
            player.stateMachine.ChangeState(PlayerState.DragonIdle);
        }
    }
}
