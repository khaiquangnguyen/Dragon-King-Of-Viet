using UnityEngine;

public class ManExecution : PlayerStateBehavior {
    public float startedAt;
    public ManExecution(Player player) : base(player, PlayerState.ManExecution, PlayerForm.Man) { }

    public override void OnStateEnter() {
        startedAt = Time.time;
        Utils.PlayAnimationMatchingDuration(player.humanAnimator, player.playerStats.executionAnimation,
            player.playerStats.executionDuration);
    }

    public override void FixedUpdate() {
        if (Time.time - startedAt >= player.playerStats.executionDuration) {
            player.SetStateAfterMovement();
        }
    }
}