using UnityEngine;

public class ManCastSpell : BaseSpellCast {
    public ManCastSpell(Player player) : base(player, PlayerState.ManCastSpell, PlayerForm.Man) {
        animator = player.humanAnimator;
    }

    public override void OnRecoveryEnd() {
        player.stateMachine.ChangeState(PlayerState.ManIdle);
    }
}
