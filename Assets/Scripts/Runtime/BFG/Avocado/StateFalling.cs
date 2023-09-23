namespace BFG.Avocado {
internal class StateFalling : AvocadoState {
    public StateFalling(AvocadoState[] states) : base(states) {
    }

    public override void Update(ref AvocadoController avocado) {
        // if (avocado._isGrounded) {}
    }

    public override void OnGrounded(ref AvocadoController avocado) {
        SwitchState(ref avocado, AvocadoStateIndex.Movement);
    }
}
}
