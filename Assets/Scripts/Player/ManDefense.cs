using UnityEngine;

public class ManDefense : PlayerStateBehavior {
    private float newStateStartAt;
    private float defenseStartTimestamp;
    private DefenseState defenseState = DefenseState.Ready;

    public ManDefense(Player player) : base(player, CharacterState.ManDefense, PlayerForm.Man) { }

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
            player.humanAnimator.Play("DefenseStartup");
            if (Time.time - newStateStartAt > player.playerStats.defenseStartupDuration) {
                defenseState = DefenseState.ActivePreCounter;
                newStateStartAt = Time.time;
            }
        }
        else if (defenseState == DefenseState.ActivePreCounter) {
            player.humanAnimator.Play("Defense");
            if (Time.time - newStateStartAt > player.playerStats.defenseActivePreCounterDuration) {
                defenseState = DefenseState.ActiveDuringCounter;
                newStateStartAt = Time.time;
            }
        }
        else if (defenseState == DefenseState.ActiveDuringCounter) {
            player.humanAnimator.Play("Counter");
            if (Time.time - newStateStartAt > player.playerStats.defenseActiveDuringCounterDuration) {
                defenseState = DefenseState.ActivePostCounter;
                newStateStartAt = Time.time;
            }
        }
        else if (defenseState == DefenseState.ActivePostCounter) {
            player.humanAnimator.Play("Defense");
        }
        else if (defenseState == DefenseState.Recovery) {
            player.humanAnimator.Play("IdleTransition");
            if (Time.time - newStateStartAt > player.playerStats.defenseRecoveryDuration)
                player.stateMachine.ChangeState(CharacterState.ManIdle);
        }
    }

}
