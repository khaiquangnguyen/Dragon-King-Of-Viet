using UnityEngine;

public class ManEmpoweredDefense:PlayerStateBehavior {
    public SkillState state = SkillState.Ready;
    public float lastStateTimestamp;

    public ManEmpoweredDefense(Player player) : base(player, PlayerState.ManEmpoweredDefense, PlayerForm.Man) { }

    public void Begin() {
        lastStateTimestamp = Time.time;
        player.inputDisabled = true;
        player.ResetEmpowermentAfterTrigger();
    }

    public override void FixedUpdate() {
        if (state == SkillState.Ready) {
            state = SkillState.Startup;
        }
        else if (state == SkillState.Startup) {
            if (Time.time - lastStateTimestamp > player.playerStats.empoweredDefenseStartupDuration) {
                state = SkillState.Active;
                lastStateTimestamp = Time.time;
            }

        }
        else if (state == SkillState.Active) {
            if (player.currentDragonEnergy < 0) {
                state = SkillState.Recovery;
                lastStateTimestamp = Time.time;
            }
            else
                player.currentDragonEnergy -=
                    player.playerStats.empoweredDefenseBaseEnergyDrainRate * Time.fixedDeltaTime;
        }
        else if (state == SkillState.Recovery) {
            if (Time.time - lastStateTimestamp > player.playerStats.empoweredDefenseRecoveryDuration || player.playerStats.empoweredDefenseAnimationCancellable)
                End();
        }
    }

    public void EarlyDefenseEnd() {
        if (state is SkillState.Active or SkillState.Startup) {
            state = SkillState.Recovery;
            lastStateTimestamp = Time.time;
        }
    }

    public void End() {
        player.inputDisabled = false;
    }
}
