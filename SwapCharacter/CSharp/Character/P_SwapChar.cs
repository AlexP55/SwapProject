using System.Linq;
using System.Collections;
using SwapBaseMethod;
using System.Collections.Generic;

namespace SwapCharacter
{
    /// <summary>
    /// Quick Deploy
    /// Passive:
    /// Obtain Eye of Phalanx (active item, 2 charges, gain one charge when a battle ends).
    /// 
    /// Use out of battle: Adjust positions of allies at the start of a battle(do not consume charges).
    /// 
    /// Use in battle: swap two allies' positions and protect healing gauge.
    /// </summary>
    public class P_SwapChar : Passive_Char, IP_BattleStart_Ones, IP_LevelUp, IP_BattleEnd
    {
        public bool ItemTake = false;

        public override void Init()
        {
            base.Init();
            OnePassive = true;
        }

        public void BattleStart(BattleSystem Ins)
        {
            var allies = Ins.AllyTeam.Chars.ToList();
            if (allies.Count >= 2)
            {
                var formation = FormationCV.Instance.order;
                Dictionary<BattleChar, int> alliesPos = new Dictionary<BattleChar, int>();
                for (int i = 0; i < formation.Count; i++)
                {
                    var bc = allies.Find(c => c.Info == PlayData.TSavedata.Party[i]);
                    alliesPos.Add(bc, formation[i]);
                }
                BattleSystem.DelayInput(SwapUtils.ChangePosition(alliesPos, AnimationType.Shuffle));
            }
        }

        public void LevelUp()
        {
            FieldSystem.DelayInput(Delay());
        }

        private IEnumerator Delay()
        {
            if (!ItemTake)
            {
                ItemTake = true;
                InventoryManager.Reward(ItemBase.GetItem(ModItemKeys.Item_Active_Active_SwapChar));
            }
            yield break;
        }

        public void BattleEnd()
        {
            ChargeActive();
        }

        public static Item_Active GetActive()
        {
            return PartyInventory.InvenM.ReturnItem(ModItemKeys.Item_Active_Active_SwapChar) as Item_Active;
        }

        public static void ChargeActive()
        {
            var item = GetActive();
            if (item != null) item.ChargeNow++;
        }
    }
}