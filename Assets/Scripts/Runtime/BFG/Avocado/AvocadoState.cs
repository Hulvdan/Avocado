namespace BFG.Avocado {
internal abstract class AvocadoState {
    readonly AvocadoState[] _states;

    public AvocadoState(AvocadoState[] states) {
        _states = states;
    }

    public virtual void OnEnter(ref AvocadoController avocado) {
    }

    public virtual void OnExit(ref AvocadoController avocado) {
    }

    public virtual void OnJumpStarted(ref AvocadoController avocado) {
    }

    public virtual void OnJumpEnded(ref AvocadoController avocado) {
    }

    public virtual void OnGravity(ref AvocadoController avocado, float gravity) {
    }

    public virtual void OnMove(ref AvocadoController avocado, float moveAxisValue) {
    }

    public virtual void Update(ref AvocadoController avocado) {
    }

    public virtual void OnThrow(ref AvocadoController avocado) {
    }

    public virtual void OnThrowEnded(ref AvocadoController avocado) {
    }

    protected void SwitchState(ref AvocadoController avocado, AvocadoStateIndex index) {
        avocado.SwitchState(_states[(int)index]);
    }
}
}
