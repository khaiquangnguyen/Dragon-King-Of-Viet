using UnityEngine;

public class ManIdle : PlayerStateBehavior {
    public ManIdle(Player player) : base(player, PlayerState.ManIdle, PlayerForm.Man) { }

    public override void OnStateEnter() {
        player.humanAnimator.Play("Idle");
        MonoBehaviour.print("wha hahah");
    }

    public override void Update() { }

    public override void FixedUpdate() {
        player.humanController.MoveOnNonGround(0, 0);
        if (player.environment == Environment.Air) player.stateMachine.ChangeState(PlayerState.ManFall);
    }

    public override void OnStateExit() { }
}