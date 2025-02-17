using UnityEngine;

public class ManDefense : PlayerStateBehavior {
    private float newStateStartAt;
    private DefenseState defenseState = DefenseState.Ready;

    public ManDefense(Player player) : base(player, PlayerState.ManDefense, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.UpdateVelocity(0, 0);
        defenseState = DefenseState.Ready;
        newStateStartAt = Time.time;
    }

    public override void Update() {
        if (player.CheckChangeToManDragonCastSpell()) {
            newStateStartAt = Time.time;
            defenseState = DefenseState.Recovery;
            return;
        }

        if (Input.GetButtonUp("Defense")) {
            newStateStartAt = Time.time;
            defenseState = DefenseState.Recovery;
        }
    }

    public override void FixedUpdate() {
        if (defenseState == DefenseState.Ready) {
            defenseState = DefenseState.Startup;
            newStateStartAt = Time.time;
        }
        else if (defenseState == DefenseState.Startup) {
            player.humanAnimator.Play(player.playerStats.defenseStartupAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseStartupDuration) {
                defenseState = DefenseState.ActiveCounter;
                newStateStartAt = Time.time;
            }
        }
        else if (defenseState == DefenseState.ActiveCounter) {
            player.humanAnimator.Play(player.playerStats.defenseActiveCounterAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseActiveCounterDuration) {
                defenseState = DefenseState.ActiveNoCounter;
                newStateStartAt = Time.time;
            }
        }
        else if (defenseState == DefenseState.ActiveNoCounter) {
            player.humanAnimator.Play(player.playerStats.defenseActiveNoCounterAnimation.name);
        }
        else if (defenseState == DefenseState.Recovery) {
            player.humanAnimator.Play(player.playerStats.defenseRecoveryAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseRecoveryDuration)
                player.stateMachine.ChangeState(PlayerState.ManIdle);
        }
    }
}