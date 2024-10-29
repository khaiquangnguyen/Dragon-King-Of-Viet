using UnityEngine;

public class DragonFloat : PlayerStateBehavior {
    public DragonFloat(Player player) : base(player, CharacterState.DragonFloat, PlayerForm.Dragon) { }
    public float dragonHoverBufferCountdown;

    public override void OnStateEnter() {
        player.dragonMaxSpeed = player.playerStats.dragonMaxSpeed;
        // player.dragonAnimator.Play("run");

    }

    public override void FixedUpdate() {
        // human x movement is dependent on the environment

        var acceleration = player.playerStats.manGroundAccel;
        var deceleration = player.playerStats.manGroundDecel;
        var maxSpeedX = player.playerStats.dragonMaxSpeed;
        player.MoveX(acceleration, deceleration, maxSpeedX);
        if (Mathf.Approximately(player.body.linearVelocity.x, 0)) {
            player.stateMachine.ChangeState(CharacterState.DragonHover);
        }
        float y = Mathf.PingPong(Time.time, 1f) - 0.5f;
        player.dragonBody.transform.localPosition = new Vector3(0, y, 0);
        player.UpdateVelocityY(0);
    }
}
