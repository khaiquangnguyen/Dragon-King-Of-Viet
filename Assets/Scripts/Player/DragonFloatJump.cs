using UnityEngine;

public class DragonFloatJump : PlayerStateBehavior {
    public DragonFloatJump(Player player) : base(player, PlayerState.DragonFloatJump, PlayerForm.Dragon) { }
    public float dragonHoverBufferCountdown;
    private float jumpStartTimestamp;
    private float jumpMaxHeight;
    private float jumpSpeed;
    private float jumpPeakHangDuration;
    private float jumpPeakHangThreshold;
    private bool bumpHead;
    private float jumpMoveY;
    private bool jumpCutStarted;
    private float jumpCutTimestamp;
    private float startingJumpHeight;

    public override void OnStateEnter() {
        startingJumpHeight = player.characterController.body.position.y;
        player.dragonMaxSpeed = player.playerStats.dragonMaxSpeed;
        player.dragonAnimator.Play(player.playerStats.dragonFloatMoveAnimation.name);
        player.dragonAnimator.speed = 1;
        jumpCutStarted = false;
        jumpMaxHeight = player.playerStats.jumpMaxHeight;
        jumpSpeed = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(player.playerStats.gravity));
        // begin jump
        jumpMoveY = jumpSpeed;
        jumpStartTimestamp = Time.time;
    }

    public void UpdateYDuringJumpUpDown(float jumpDuration, float jumpHeight, AnimationCurve heightCurve) {

    }

    public void RecoverOriginalPosition() {
        var currentHeight = player.characterController.body.position.y;
        var acceleration = player.playerStats.manAirAccel;
        var deceleration = player.playerStats.manAirDecel;
        var maxSpeedX = player.playerStats.manAirMaxHSpeed * Mathf.Abs(player.inputDirectionX);
        var accelerationFactor = Mathf.Abs(player.characterController.velocity.x) > maxSpeedX
            ? acceleration
            : deceleration;
        var jumpMoveX = Mathf.MoveTowards(Mathf.Abs(player.characterController.velocity.x), maxSpeedX,
            accelerationFactor * Time.fixedDeltaTime) * player.facingDirection;
        var gravityMult = currentHeight > startingJumpHeight ? 1 : -1;
        player.characterController.MoveOnNonGroundHorizontalWithGravity(acceleration, deceleration, maxSpeedX,
            player.playerStats.gravity, gravityMult, player.facingDirection, player.playerStats.maxVerticalSpeed);
    }

    public override void FixedUpdate() {
        var acceleration = player.playerStats.manAirAccel;
        var deceleration = player.playerStats.manAirDecel;
        var maxSpeedX = player.playerStats.manAirMaxHSpeed * Mathf.Abs(player.inputDirectionX);
        var accelerationFactor = Mathf.Abs(player.characterController.velocity.x) > maxSpeedX
            ? acceleration
            : deceleration;
        var jumpMoveX = Mathf.MoveTowards(Mathf.Abs(player.characterController.velocity.x), maxSpeedX,
            accelerationFactor * Time.fixedDeltaTime) * player.facingDirection;
        if (Time.time - jumpStartTimestamp <= player.playerStats.jumpDuration) {
            var jumpHeightCurrentPercentage =
                player.playerStats.dragonJumpHeightCurve.Evaluate((Time.time - jumpStartTimestamp) /
                                                                  player.playerStats.jumpDuration);
            var jumpHeightNextPercentage = player.playerStats.jumpHeightCurve.Evaluate(
                (Time.time - jumpStartTimestamp + Time.fixedDeltaTime) /
                player.playerStats.jumpDuration);
            var jumpHeightDelta = (jumpHeightNextPercentage - jumpHeightCurrentPercentage) *
                                  player.playerStats.jumpMaxHeight;
            jumpMoveY = jumpHeightDelta / Time.fixedDeltaTime;
        }
        else {
            var currentHeight = player.characterController.body.position.y;
            jumpMoveY = player.characterController.velocity.y;
            var gravity = player.playerStats.gravity;
            var gravityMult = currentHeight > startingJumpHeight ? 1 : -1;
            jumpMoveY +=  gravity * gravityMult * Time.fixedDeltaTime;
            var distanceToOriginalPosition = currentHeight - startingJumpHeight;
            // jumpMoveY has to be less than the distance to the original position
            if (distanceToOriginalPosition < 0) {
                jumpMoveY = Mathf.Max(jumpMoveY, distanceToOriginalPosition);
            }
            else {
                jumpMoveY = Mathf.Min(jumpMoveY, distanceToOriginalPosition);
            }
            jumpMoveY = Mathf.Clamp(jumpMoveY, -player.playerStats.maxVerticalSpeed, player.playerStats.maxVerticalSpeed);
        }
        player.characterController.Move(jumpMoveX, jumpMoveY);
        // near the starting jump point, change to idle
        if (Mathf.Abs(player.characterController.body.position.y - startingJumpHeight) < 0.1f && Time.time - jumpStartTimestamp > 0.1f) {
            player.stateMachine.ChangeState(PlayerState.DragonIdle);
        }
    }
}
