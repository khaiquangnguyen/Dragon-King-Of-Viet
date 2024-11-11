namespace CharacterBehavior {
    public interface IDamageTaker {
        public DamageResult OnTakeDamage(int damage, IDamageDealer damageDealer);
    }
}