using UnityEngine;

public class ManEmpoweredDefense : PlayerStateBehavior {
    public DefenseState defenseState = DefenseState.Ready;
    private float newStateStartAt;

    public ManEmpoweredDefense(Player player) : base(player, PlayerState.ManEmpoweredDefense, PlayerForm.Man) { }

    public override void OnStateEnter() {
        defenseState = DefenseState.Ready;
        newStateStartAt = Time.time;
        player.ResetEmpowermentAfterTrigger();
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
            newStateStartAt = Time.time;
        }
        else if (defenseState == DefenseState.Startup) {
            player.humanAnimator.Play(player.playerStats.empoweredDefenseStartupAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseStartupDuration) {
                defenseState = DefenseState.ActiveCounter;
                newStateStartAt = Time.time;
            }
        }
        else if (defenseState == DefenseState.ActiveCounter) {
            player.humanAnimator.Play(player.playerStats.empoweredDefenseActiveCounterAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseActiveCounterDuration) {
                defenseState = DefenseState.ActiveNoCounter;
                newStateStartAt = Time.time;
            }
        }
        else if (defenseState == DefenseState.ActiveNoCounter) {
            player.humanAnimator.Play(player.playerStats.empoweredDefenseActiveNoCounterAnimation.name);
        }
        else if (defenseState == DefenseState.Recovery) {
            player.humanAnimator.Play(player.playerStats.empoweredDefenseRecoveryAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseRecoveryDuration)
                player.stateMachine.ChangeState(PlayerState.ManIdle);
        }
    }

}
