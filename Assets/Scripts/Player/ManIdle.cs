public class ManIdle : PlayerStateBehavior {
    public ManIdle(Player player) : base(player, CharacterState.ManIdle, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("Idle");
        player.UpdateVelocity(0,0);
    }

    public override void Update() {
    }

    public override void FixedUpdate() {
        if (player.environment == Environment.Air) {
            player.stateMachine.ChangeState(CharacterState.ManFall);
        }
    }

    public override void OnStateExit() {
    }
}
