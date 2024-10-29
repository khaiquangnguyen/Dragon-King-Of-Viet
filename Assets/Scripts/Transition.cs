using System.Linq.Expressions;

public abstract class Transition {
    public CharacterState from;
    public CharacterState to;
    public LambdaExpression condition;

    public Transition(CharacterState from, CharacterState to, LambdaExpression condition) {
        this.from = from;
        this.to = to;
        this.condition = condition;
    }

    public bool ShouldTransition() {
        return (bool) condition.Compile().DynamicInvoke();
    }

    public abstract void OnTransition();

}
