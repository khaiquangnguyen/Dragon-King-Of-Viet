using UnityEngine;

public class DragonCastSpell : BaseSpellCast {
    public DragonCastSpell(Player player) : base(player, PlayerState.DragonCastSpell, PlayerForm.Dragon) {
        animator = player.dragonAnimator;
    }

    public override void OnRecoveryEnd() {
        player.stateMachine.ChangeState(PlayerState.DragonIdle);
    }
}