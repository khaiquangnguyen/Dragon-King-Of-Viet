public class DragonIdle : PlayerStateBehavior {
    public DragonIdle(Player player) : base(player, PlayerState.DragonIdle, PlayerForm.Dragon) { }
    private float amplitude = 1f;

    public override void OnStateEnter() {
        player.UpdateVelocity(0, 0);
    }

    public override void Update() {
        player.CheckTransformIntoDragonAndBack();
        player.CheckChangeToDragonFly();
        player.CheckChangeToDragonFloat();
    }

    public override void FixedUpdate() {
        // make the dragon float up and down in sine way
    }

    public override void OnStateExit() { }
}

// public class DragonHover : PlayerStateBehavior {
//     public DragonHover(Player player) : base(player, CharacterState.DragonHover, PlayerForm.Dragon) { }
//     private float dragonMaxSpeed;
//     private Vector3 dragonHoverTargetPosition;
//
//     public override void OnStateEnter() {
//         player.StartCoroutine(RandomDragonHoverPosition());
//         dragonMaxSpeed = player.playerStats.dragonHoverMaxSpeed;
//     }
//
//     private IEnumerator<WaitForSeconds> RandomDragonHoverPosition() {
//         for (;;) {
//             var dragonHoverRandomFactorX = player.playerStats.dragonHoverRandomFactorX;
//             var dragonHoverRandomFactorY = player.playerStats.dragonHoverRandomFactorY;
//             dragonHoverTargetPosition = new Vector3(
//                 player.dragonFlyTowardPosition.x + Random.Range(-dragonHoverRandomFactorX, dragonHoverRandomFactorX),
//                 player.dragonFlyTowardPosition.y + Random.Range(-dragonHoverRandomFactorY, dragonHoverRandomFactorY),
//                 player.dragonFlyTowardPosition.z
//             );
//             yield return new WaitForSeconds(player.playerStats.dragonHoverRandomPeriod);
//         }
//     }
//
//     public override void FixedUpdate() {
//         MonoBehaviour.print("Hovering");
//         player.CheckDragonForcedHover();
//         if (!player.dragonForcedHoverInPlace) {
//             if (Camera.main == null) return;
//             var currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//             var isDragonNearMouse = Vector2.Distance(player.body.position, currentMousePosition) <
//                                     player.playerStats.dragonHoverAllowedDistanceToMouse;
//             var isMouseRelativelyStable = Vector2.Distance(currentMousePosition, player.dragonFlyTowardPosition) <
//                                           player.playerStats.dragonHoverPointerMovementRange;
//             if (!isMouseRelativelyStable || !isDragonNearMouse)
//                 player.stateMachine.ChangeState(CharacterState.DragonFly);
//         }
//
//         player.DragonMove(dragonHoverTargetPosition);
//     }
//
//     public override void OnStateExit() {
//         player.StopCoroutine(RandomDragonHoverPosition());
//         dragonMaxSpeed = player.playerStats.dragonMaxSpeed;
//     }
// }
