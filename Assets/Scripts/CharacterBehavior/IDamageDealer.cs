namespace CharacterBehavior {
    public interface IDamageDealer {
        public void OnDealDamage(int damageDealt, IDamageTaker gameCharacter);
    }
}
