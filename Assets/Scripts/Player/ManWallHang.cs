public class ManWallHang : PlayerStateBehavior {

    public ManWallHang(Player player) : base(player, PlayerState.ManWallHang, PlayerForm.Man) { }

    public override void Update() {
        player.CheckChangeToManJumpOrEmpoweredJumpState();
    }

    public override void FixedUpdate() {
        var canWallHang = player.characterController.CheckCanWallHang(player.facingDirection);
        // fall down slowly
        if (canWallHang) {
            var vy = player.playerStats.manWallHangFallSpeed;
            player.characterController.Move(0, vy);
        }
        else {
            player.stateMachine.ChangeState(PlayerState.ManFall);
        }
    }
}
