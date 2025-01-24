using UnityEngine;

public class ManToDragonTransform : PlayerStateBehavior {
    private float manToDragonCountdown;

    public ManToDragonTransform(Player player) : base(player, PlayerState.ManToDragon, PlayerForm.Dragon) { }

    public override void OnStateEnter() {
        player.UpdateVelocity(0, 0);
        manToDragonCountdown = player.playerStats.manToDragonTransformDuration;
        player.dragonBody.gameObject.SetActive(true);
        player.dragonAnimator.Play(player.playerStats.manToDragonTransformAnimation.name);
        player.humanBody.gameObject.SetActive(false);
    }

    public override void FixedUpdate() {
        // the transformation is a two part transform
        // first part is 1 third of the animation
        // second part is the rest of the animation
        // in the first part, the human scales down and fades out

        player.UpdateVelocity(0, 0);
        manToDragonCountdown = Mathf.Max(manToDragonCountdown - Time.fixedDeltaTime, 0f);
        var firstTransformDuration = player.playerStats.manToDragonTransformDuration * 0.5f;
        var secondTransformDuration = player.playerStats.manToDragonTransformDuration * 0.5f;
        var timePassSinceTransformedRatio = player.playerStats.manToDragonTransformDuration - manToDragonCountdown;
        if (timePassSinceTransformedRatio > firstTransformDuration) {
            player.dragonAnimator.Play(player.playerStats.manToDragonTransformRoarAnimation.name);
        }
        if (manToDragonCountdown <= 0) player.stateMachine.ChangeState(PlayerState.DragonIdle);
    }

    public override void OnStateExit() {
        player.dragonBody.gameObject.transform.localScale = Vector3.one;
    }
}
