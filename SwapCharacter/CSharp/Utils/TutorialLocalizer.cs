using TMPro;
using UnityEngine;

namespace SwapCharacter
{
    /// <summary>
    /// Localize the tutorial object based on LangSystemDB of mod
    /// </summary>
    public class TutorialLocalizer : MonoBehaviour
    {
        private TutorialObject Tut;
        private int NowSetNum;

        public void Init(TutorialObject tut)
        {
            Tut = tut;
            NowSetNum = -1;
        }

        private void Update()
        {
            if (Tut != null)
            {
                var num = Tut.GetField<int>("SetNum") - 1;
                if (NowSetNum != num)
                {
                    NowSetNum = num;
                    var tutorial = Tut.GetField<Tutorial>("Setting");
                    if (tutorial != null && num >= 0 && num < tutorial.Contents.Count)
                    {
                        var content = tutorial.Contents[num];
                        var key = content.Text_I2.mTerm;
                        var text = Tut.GetField<TextMeshProUGUI>("TextUI");
                        text.text = SwapExt.GetTranslation(key);
                    }
                }
            }
        }
    }
}
