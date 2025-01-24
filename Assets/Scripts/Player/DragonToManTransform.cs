using UnityEngine;

public class DragonToManTransform : PlayerStateBehavior {
    private float dragonToManCountdown;

    public DragonToManTransform(Player player) : base(player, PlayerState.DragonToMan, PlayerForm.Dragon) { }

    public override void OnStateEnter() {
        player.dragonRotationSpeed = player.playerStats.dragonToManRotationSpeed;
        player.inputDisabled = true;
        player.ChangeAlphaOfHumanAnimator(1);
        player.humanAnimator.Play("Idle");
        player.dragonMaxSpeed = player.playerStats.dragonMaxSpeed;
        dragonToManCountdown = player.playerStats.dragonToManTransformDuration;
        player.humanBody.gameObject.SetActive(true);
        player.dragonAnimator.Play(player.playerStats.manToDragonTransformAnimation.name);
    }

    public override void FixedUpdate() {
        dragonToManCountdown = Mathf.Max(dragonToManCountdown - Time.fixedDeltaTime, 0f);
        var transformRate = 1 - dragonToManCountdown / player.playerStats.dragonToManTransformDuration;
        // player.dragonBody.gameObject.transform.localScale = Vector3.one * (1 - transformRate);
        if (Mathf.Approximately(dragonToManCountdown, 0)) {
            player.inputDisabled = false;
            player.dragonBody.gameObject.SetActive(false);
            player.humanBody.gameObject.SetActive(true);
            player.humanBody.gameObject.transform.localScale = new Vector3(1, 1, 1);
            player.transform.rotation = Quaternion.identity;
            player.CheckGround();
            if (player.environment == Environment.Ground) {
                if (player.inputDirectionX != 0) player.stateMachine.ChangeState(PlayerState.ManRun);
                else
                    player.stateMachine.ChangeState(PlayerState.ManIdle);
            }
            else {
                player.stateMachine.ChangeState(PlayerState.ManFall);
            }
        }
    }

    public override void OnStateExit() {
        player.humanBody.gameObject.SetActive(true);
    }
}
