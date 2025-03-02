
namespace SwapCharacter
{
	/// <summary>
	/// Barrier
	/// </summary>
    public class B_SwapChar_10_S:Buff
    {
        public const float ratio = 35f;

        public override void Init()
        {
            base.Init();
            BarrierHP += (int)Misc.PerToNum(Usestate_L.GetStat.maxhp, ratio);
        }
    }
}