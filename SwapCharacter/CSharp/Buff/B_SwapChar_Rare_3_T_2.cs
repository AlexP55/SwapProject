using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Force Shield
    /// Position Buff
    /// </summary>
    public class B_SwapChar_Rare_3_T_2:B_SwapStable
    {
        public override void Init()
        {
            base.Init();
            PlusStat.Strength = true;
            PlusStat.def = 20;
        }
    }
}