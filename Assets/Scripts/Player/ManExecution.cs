using UnityEngine;

public class ManExecution : PlayerStateBehavior {
    public float StartedAt;
    public ManExecution(Player player) : base(player, PlayerState.ManExecution, PlayerForm.Man) { }

    public override void OnStateEnter() {
        StartedAt = Time.time;
        Utils.PlayAnimationMatchingDuration(player.humanAnimator, player.playerStats.executionAnimation,
            player.playerStats.executionDuration);
    }

    public override void FixedUpdate() {
        if (Time.time - StartedAt >= player.playerStats.executionDuration) {
            player.SetStateAfterMovement();
        }
    }
}