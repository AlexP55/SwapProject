
namespace SwapCharacter
{
    /// <summary>
    /// Enhanced Perception
    /// </summary>
    public class B_SwapChar_6_T:Buff
    {
        public override void Init()
        {
            base.Init();
            PlusPerStat.Damage = 10;
            PlusStat.cri = 20;
            PlusStat.PlusCriDmg = 30;
            PlusStat.Strength = true;
        }
    }
}