using UnityEngine;

public class ManToDragonTransform: PlayerStateBehavior {
    private float manToDragonCountdown;

    public ManToDragonTransform(Player player) : base(player, PlayerState.ManToDragon, PlayerForm.Dragon) { }

    public override void OnStateEnter() {
        player.UpdateVelocity(0,0);
        manToDragonCountdown = player.playerStats.manToDragonTransformDuration;
        player.dragonBody.gameObject.SetActive(true);
        player.ChangeAlphaOfHumanAnimator(1);
        player.ChangeAlphaOfDragonAnimator(0);
        player.dragonBody.gameObject.transform.localScale = Vector3.zero;
    }

    public override void FixedUpdate() {
        player.UpdateVelocity(0,0);
        manToDragonCountdown = Mathf.Max(manToDragonCountdown - Time.fixedDeltaTime, 0f);
        var transformRate = 1 - manToDragonCountdown / player.playerStats.manToDragonTransformDuration;
        if (transformRate > 0.25f) {
            player.dragonBody.gameObject.transform.localScale = Vector3.one * transformRate;
            // scale down and fade human slowly
            player.ChangeAlphaOfHumanAnimator(1- transformRate);
            player.ChangeAlphaOfDragonAnimator(transformRate);
        }
        if (transformRate >= 1) {
            player.stateMachine.ChangeState(PlayerState.DragonHover);
        }
    }

    public override void OnStateExit() {
        player.humanBody.gameObject.SetActive(false);
        player.dragonBody.gameObject.transform.localScale = Vector3.one;
        player.ChangeAlphaOfDragonAnimator(1);
    }
}
