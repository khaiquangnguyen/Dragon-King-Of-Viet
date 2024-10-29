public class ManWallHang: PlayerStateBehavior {
    private readonly Player player;

    public ManWallHang(Player player) : base(player, CharacterState.ManWallHang, PlayerForm.Man) { }
}
