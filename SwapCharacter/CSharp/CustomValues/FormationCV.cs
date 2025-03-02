using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SwapCharacter
{
    /// <summary>
    /// Custom value for saving starting formation infomation of the party
    /// </summary>
    public class FormationCV : CustomValue
    {
        public List<int> _order; // record the formation, ith number is the position of the ith character in party

        [XmlIgnore]
        public List<int> order
        {
            get
            {
                int partyNum = PlayData.TSavedata.Party.Count;
                var avalablePos = Enumerable.Range(0, partyNum).ToList();
                var list = new List<int>();
                for (int i = 0; i < partyNum; i++)
                {
                    if (i < _order.Count && avalablePos.Remove(_order[i]))
                    {
                        list.Add(_order[i]);
                    }
                    else
                    {
                        list.Add(-1);
                    }
                }
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j] < 0 && avalablePos.Count > 0) 
                    {
                        list[j] = avalablePos[0];
                        avalablePos.RemoveAt(0);
                    }
                }
                return list;
            }
        }

        public static FormationCV Instance
        {
            get
            {
                if (PlayData.TSavedata == null)
                {
                    return new FormationCV();
                }
                return PlayData.TSavedata.GetOrAddCustomValue<FormationCV>();
            }
        }

        public FormationCV()
        {
            _order = new List<int>();
        }

        public void SetOrder(List<int> od)
        {
            _order = od.ToList();
        }
    }
}
