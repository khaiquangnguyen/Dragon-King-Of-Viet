using UnityEngine;

public abstract class PlayerStateBehavior {
    public readonly CharacterState name;
    public readonly PlayerForm form;
    protected readonly Player player;

    protected PlayerStateBehavior(Player player, CharacterState name, PlayerForm form) {
        this.player = player;
        this.name = name;
        this.form = form;
    }

    public virtual void OnStateEnter() { }
    public virtual void Update() { }

    public virtual void FixedUpdate() { }
    public virtual void OnStateExit() { }
}
