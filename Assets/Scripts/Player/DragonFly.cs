using UnityEngine;

public class DragonFly : PlayerStateBehavior {
    public DragonFly(Player player) : base(player, PlayerState.DragonFly, PlayerForm.Dragon) { }
    public float dragonHoverBufferCountdown;
    private Vector2 dragonMoveDirection;
    public float dragonMaxSpeed;

    public override void OnStateEnter() {
        player.dragonAnimator.Play("spin");
        player.dragonMaxSpeed = player.playerStats.dragonMaxSpeed;
        dragonMaxSpeed = player.playerStats.dragonMaxSpeed;
    }

    public override void Update() { }

    public override void FixedUpdate() {
        var input = Input.GetButton("Jump");
        if (!input) {
            player.stateMachine.ChangeState(PlayerState.DragonHover);
            return;
        }
        if (Camera.main == null) return;
        var currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragonMoveDirection = currentMousePosition - player.transform.position;
        var angle = Mathf.Atan2(dragonMoveDirection.y, dragonMoveDirection.x) * Mathf.Rad2Deg;
        // rotate along the axis by angle
        player.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        var movementAcceleration = Mathf.Abs(player.characterController.velocity.magnitude) < dragonMaxSpeed
            ? player.playerStats.dragonAccel
            : player.playerStats.dragonDecel;
        var dragonSpeed = Mathf.MoveTowards(player.characterController.velocity.magnitude, player.playerStats.dragonMaxSpeed, movementAcceleration * Time.fixedDeltaTime);
        player.characterController.velocity = dragonMoveDirection.normalized * dragonSpeed;
    }

    public override void OnStateExit() {
        player.transform.rotation = Quaternion.identity;
    }
}
