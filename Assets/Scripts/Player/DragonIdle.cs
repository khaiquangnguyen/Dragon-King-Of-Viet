public class DragonIdle : PlayerStateBehavior {
    public DragonIdle(Player player) : base(player, PlayerState.DragonIdle, PlayerForm.Dragon) { }
    private float amplitude = 1f;

    public override void OnStateEnter() {
        player.UpdateVelocity(0, 0);
    }

    public override void Update() {
        if (player.CheckTransformIntoDragonAndBack()) return;
        if (player.CheckChangeToDragonFly()) return;
        if (player.CheckChangeToDragonFloat()) return;
        if (player.CheckChangeToDragonAttackState()) return;
        if (player.CheckChangeToDragonDefenseState()) return;
        if (player.CheckChangeToDragonOrManCastSpell()) return;
    }

    public override void FixedUpdate() {
        // make the dragon float up and down in sine way
    }

    public override void OnStateExit() { }
}
