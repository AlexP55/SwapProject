
namespace SwapCharacter
{
    /// <summary>
    /// Distracted
    /// </summary>
    public class B_SwapChar_2_T:Buff, IP_TurnEnd
    {
        public override void Init()
        {
            base.Init();
            PlusStat.Weak = true;
        }

        public void TurnEnd()
        {
            SelfDestroy();
        }
    }
}