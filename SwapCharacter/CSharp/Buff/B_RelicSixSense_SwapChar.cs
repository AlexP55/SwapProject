using System.Collections.Generic;
using GameDataEditor;

namespace SwapCharacter
{
    /// <summary>
    /// 6th Sense
    /// Apply nonvanishing "Identified!" to the unit. When defeated, transfer "6th Sense" to another unit.
    /// </summary>
    public class B_RelicSixSense_SwapChar:Buff, IP_Dead, IP_Awake, IP_BuffRemove
    {
        public override void Init()
        {
            base.Init();
            PlusStat.crihit = 10;
        }

        public void Dead()
        {
            List<BattleChar> list = new List<BattleChar>(BChar.MyTeam.AliveChars);
            list.Remove(BChar);
            if (list.Count > 0) list.Random(BChar.GetRandomClass().Main).BuffAdd(BuffData.Key, Usestate_L);
        }

        public void Awake()
        {
            var identified = BChar.BuffAdd(GDEItemKeys.Buff_B_Control_P, Usestate_L);
            identified.CantDisable = true;
        }

        public void BuffRemove(BattleChar buffMaster, Buff buff)
        {
            if (buff.BuffData.Key == GDEItemKeys.Buff_B_Control_P && buffMaster == BChar)
            {
                Buff identified = null;
                for (int i = 0; i < buff.StackNum; i++)
                {
                    identified = BChar.BuffAdd(GDEItemKeys.Buff_B_Control_P, Usestate_L);
                }
                if (identified != null) identified.CantDisable = true;
            }
        }
    }
}