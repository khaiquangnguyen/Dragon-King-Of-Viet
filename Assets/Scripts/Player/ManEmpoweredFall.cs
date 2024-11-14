using UnityEngine;

public class ManEmpoweredFall : PlayerStateBehavior {
    private SkillState state = SkillState.Ready;
    private float lastStateTimestamp;

    public ManEmpoweredFall(Player player) : base(player, PlayerState.ManEmpoweredFall, PlayerForm.Man) { }

    public void Begin() {
        player.inputDisabled = true;
        player.ResetEmpowermentAfterTrigger();
        lastStateTimestamp = Time.time;
    }

    public void FixedUpdate() {
        if (state == SkillState.Ready) {
            state = SkillState.Startup;
        }
        else if (state == SkillState.Startup) {
            if (Time.time - lastStateTimestamp > player.playerStats.empoweredFallStartupDuration) {
                state = SkillState.Active;
                lastStateTimestamp = Time.time;
            }
            else if (state == SkillState.Active) {
                state = SkillState.Recovery;
                lastStateTimestamp = Time.time;
            }
            else if (state == SkillState.Recovery) {
                if (Time.time - lastStateTimestamp > player.playerStats.empoweredFallRecoveryDuration)
                    End();
            }
        }
    }

    public void End() {
        player.stateMachine.ChangeState(PlayerState.ManIdle);
    }
}