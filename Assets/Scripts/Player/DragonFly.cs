using UnityEngine;

public class DragonFly : PlayerStateBehavior {
    public DragonFly(Player player) : base(player, PlayerState.DragonFly, PlayerForm.Dragon) { }
    public float dragonHoverBufferCountdown;

    public override void OnStateEnter() {
        player.dragonAnimator.Play("spin");
        player.dragonMaxSpeed = player.playerStats.dragonMaxSpeed;
    }

    public override void Update() { }

    public override void FixedUpdate() {
        if (Camera.main == null) return;
        var currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        player.dragonFlyTowardPosition = currentMousePosition;
        player.DragonMove(player.dragonFlyTowardPosition);
    }

    public override void OnStateExit() {
        player.transform.localRotation = Quaternion.identity;
    }
}
