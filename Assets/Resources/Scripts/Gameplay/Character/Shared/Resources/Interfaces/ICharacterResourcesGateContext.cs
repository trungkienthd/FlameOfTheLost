namespace FlameOfTheLost
{
    // Minimal interface the gate needs.
    public interface ICharacterResourcesGateContext
    {
        public HPController HP
        {
            get;
        }
        public FPController FP
        {
            get;
        }
        public StaminaController Stamina
        {
            get;
        }
    }
}
