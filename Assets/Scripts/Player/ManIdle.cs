public class ManIdle : PlayerStateBehavior {
    public ManIdle(Player player) : base(player, PlayerState.ManIdle, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play(player.playerStats.manIdleAnimation.name);
    }

    public override void Update() {
        player.CheckTransformIntoDragonAndBack();
    }

    public override void FixedUpdate() {
        player.characterController.Move(0, 0);
        var isOnGround = player.characterController.isOnWalkableGround();
        if (!isOnGround) player.stateMachine.ChangeState(PlayerState.ManFall);
    }

    public override void OnStateExit() { }
}
