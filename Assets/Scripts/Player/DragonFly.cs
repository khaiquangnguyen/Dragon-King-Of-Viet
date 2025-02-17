using UnityEngine;

public class DragonFly : PlayerStateBehavior {
    public DragonFly(Player player) : base(player, PlayerState.DragonFly, PlayerForm.Dragon) { }
    public float dragonHoverBufferCountdown;
    private Vector2 dragonMoveDirection;
    public float dragonMaxSpeed;
    public bool isFlying = true;

    public override void OnStateEnter() {
        player.dragonMaxSpeed = player.playerStats.dragonMaxSpeed;
        dragonMaxSpeed = player.playerStats.dragonMaxSpeed;
        player.dragonAnimator.Play(player.playerStats.dragonFlyAnimation.name);
        isFlying = true;
    }

    public override void Update() {
        var input = Input.GetButton("Jump");
        isFlying = input;
    }

    public override void FixedUpdate() {
        if (!isFlying) {
            player.stateMachine.ChangeState(PlayerState.DragonIdle);
            return;
        }
        if (Camera.main == null) return;
        var currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragonMoveDirection = currentMousePosition - player.transform.position;
        if (dragonMoveDirection.magnitude < 0.1f) {
            player.characterController.velocity = Vector2.zero;
            return;
        }
        var angle = Mathf.Atan2(dragonMoveDirection.y, dragonMoveDirection.x) * Mathf.Rad2Deg;
        // rotate along the axis by angle
        player.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        player.characterController.MoveOnNonGroundAnyDirectionNoGravity(player.playerStats.dragonAccel,
            player.playerStats.dragonDecel, player.playerStats.dragonMaxSpeed, dragonMoveDirection.normalized);
    }

    public override void OnStateExit() {
        player.transform.rotation = Quaternion.identity;
    }
}
