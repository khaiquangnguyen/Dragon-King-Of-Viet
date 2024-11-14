using System.Collections.Generic;

public class PlayerStateMachine {
    public PlayerState currentPlayerState;

    public readonly Dictionary<PlayerState, PlayerStateBehavior> states = new();
    public PlayerStateBehavior currentStateBehavior => states[currentPlayerState];

    public void ChangeState(PlayerState newPlayerState) {
        if (newPlayerState == currentPlayerState) return;
        currentStateBehavior.OnStateExit();
        currentPlayerState = newPlayerState;
        var newState = states[newPlayerState];
        newState.OnStateEnter();
    }

    public void Update() {
        currentStateBehavior.Update();
    }

    public void FixedUpdate() {
        currentStateBehavior.FixedUpdate();
    }

    public void LateUpdate() { }

    public void AddState(PlayerStateBehavior state) {
        states.Add(state.name, state);
    }
}