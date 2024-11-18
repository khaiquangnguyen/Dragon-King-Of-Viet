using UnityEngine;

public class DragonDefense : BasePlayerAction {
    private DefenseState defenseState = DefenseState.Ready;
    private float newStateStartAt;

    public DragonDefense(Player player) : base(player, PlayerState.DragonDefense, PlayerForm.Dragon) { }

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
            newStateStartAt = Time.time;
        }
        else if (defenseState == DefenseState.Startup) {
            player.dragonAnimator.Play(player.playerStats.defenseStartupAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseStartupDuration) {
                defenseState = DefenseState.ActiveCounter;
                newStateStartAt = Time.time;
            }
        }
        else if (defenseState == DefenseState.ActiveCounter) {
            player.dragonAnimator.Play(player.playerStats.defenseActiveCounterAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseActiveCounterDuration) {
                defenseState = DefenseState.ActiveNoCounter;
                newStateStartAt = Time.time;
            }
        }
        else if (defenseState == DefenseState.ActiveNoCounter) {
            player.dragonAnimator.Play(player.playerStats.defenseActiveNoCounterAnimation.name);
        }
        else if (defenseState == DefenseState.Recovery) {
            player.dragonAnimator.Play(player.playerStats.defenseRecoveryAnimation.name);
            if (Time.time - newStateStartAt > player.playerStats.defenseRecoveryDuration)
                player.stateMachine.ChangeState(PlayerState.DragonIdle);
        }
    }
}
