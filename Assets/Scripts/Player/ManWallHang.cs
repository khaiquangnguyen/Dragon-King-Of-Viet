public class ManWallHang : PlayerStateBehavior {
    private readonly Player player;

    public ManWallHang(Player player) : base(player, PlayerState.ManWallHang, PlayerForm.Man) { }
}