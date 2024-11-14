using UnityEngine;

public class ManDefense : PlayerStateBehavior {
    private float newStateStartAt;
    private float defenseStartTimestamp;
    private DefenseState defenseState = DefenseState.Ready;

    public ManDefense(Player player) : base(player, PlayerState.ManDefense, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.UpdateVelocity(0, 0);
        defenseState = DefenseState.Ready;
        newStateStartAt = Time.time;
    }

    public override void Update() {
        if (Input.GetButtonUp("Defense")) {
            newStateStartAt = Time.time;
            defenseState = DefenseState.Recovery;
        }
    }

    public override void FixedUpdate() {
        if (defenseState == DefenseState.Ready) {
            defenseState = DefenseState.Startup;
            defenseStartTimestamp = Time.time;
            newStateStartAt = Time.time;
        }
        else if (defenseState == DefenseState.Startup) {
            player.humanAnimator.Play(player.playerStats.defenseStartupAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseStartupDuration) {
                defenseState = DefenseState.ActivePreCounter;
                newStateStartAt = Time.time;
            }
        }
        else if (defenseState == DefenseState.ActivePreCounter) {
            player.humanAnimator.Play(player.playerStats.defenseActivePreCounterAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseActivePreCounterDuration) {
                defenseState = DefenseState.ActiveDuringCounter;
                newStateStartAt = Time.time;
            }
        }
        else if (defenseState == DefenseState.ActiveDuringCounter) {
            player.humanAnimator.Play(player.playerStats.defenseActiveDuringCounterAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseActiveDuringCounterDuration) {
                defenseState = DefenseState.ActivePostCounter;
                newStateStartAt = Time.time;
            }
        }
        else if (defenseState == DefenseState.ActivePostCounter) {
            player.humanAnimator.Play(player.playerStats.defenseActivePostCounterAnimation.name);
        }
        else if (defenseState == DefenseState.Recovery) {
            player.humanAnimator.Play(player.playerStats.defenseRecoveryAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseRecoveryDuration)
                player.stateMachine.ChangeState(PlayerState.ManIdle);
        }
    }
}
