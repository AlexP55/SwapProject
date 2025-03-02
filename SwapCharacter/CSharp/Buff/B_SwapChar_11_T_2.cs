using SwapBaseMethod;

namespace SwapCharacter
{
    /// <summary>
    /// Fleet-Footed
    /// When swap with an ally, draw 1 skill and remove one stack.
    /// </summary>
    public class B_SwapChar_11_T_2 : Buff, IP_Swapped
    {
        public void Swapped(BattleChar ally1, BattleChar ally2)
        {
            if (ally1 == BChar || ally2 == BChar)
            {
                BattleSystem.instance.AllyTeam.Draw(1);
                SelfStackDestroy();
            }
        }
    }
}