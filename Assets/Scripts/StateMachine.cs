using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class StateMachine {
    public CharacterState currentCharacterState;

    private readonly Dictionary<CharacterState, PlayerStateBehavior> states = new();
    private List<Transition> transitions = new();
    public PlayerStateBehavior currentStateBehavior => states[currentCharacterState];

    public void ChangeState(CharacterState newCharacterState) {
        if (newCharacterState == currentCharacterState) return;
        currentStateBehavior.OnStateExit();
        currentCharacterState = newCharacterState;
        var newState = states[newCharacterState];
        newState.OnStateEnter();
    }

    public void Update() {
        currentStateBehavior.Update();
    }

    public void FixedUpdate() {
        currentStateBehavior.FixedUpdate();
    }

    public void LateUpdate() {
    }

    public void AddState(PlayerStateBehavior state) {
        this.states.Add(state.name, state);
    }

    public void AddTransition(Transition transition) {
        var existingTransition = transitions.FindIndex(existingTransition =>
            existingTransition.from == transition.from && existingTransition.to == transition.to);
        if (existingTransition != -1)
            transitions[existingTransition] = transition;
        else
            transitions.Add(transition);
    }
}
