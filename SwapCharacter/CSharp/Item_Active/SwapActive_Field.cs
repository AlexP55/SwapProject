using System.Collections.Generic;
using GameDataEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SwapCharacter
{
    /// <summary>
    /// Eye of Phalanx
    /// Use out of battle: Adjust positions of allies at the start of a battle (do not consume charges).
    /// </summary>
    public class SwapActive_Field : UseitemBase
    {
        public override bool Use(Character CharInfo)
        {
            Effect(CharInfo);
            return false;
        }

        public override void Effect(Character CharInfo)
        {
            base.Effect(CharInfo);

            var FormationUI = UIManager.InstantiateActive(UIManager.inst.UnlockUI);
            var window = FormationUI.GetComponent<UnlockWindow>();
            window.Text.text = SwapExt.GetTranslation("Formation");
            window.NameText.text = SwapExt.GetTranslation("FormationDes");
            window.FadeUp = true;
            window.unlockimage.gameObject.SetActive(false);
            window.NameText.gameObject.SetActive(true);
            var FormationBase = SwapExt.GetAssets<GameObject>("Assets/ModAssets/FormationBase.prefab");
            var FormationSorter = FormationBase.transform.Find("FormationSorter").gameObject;
            FormationSorter = Object.Instantiate(FormationSorter, FormationUI.transform);
            FormationSorter.transform.SetSiblingIndex(window.unlockimage.transform.GetSiblingIndex());

            var confirmButton = window.transform.Find("ButtonV2").GetComponent<Button>();
            confirmButton.onClick.AddListener(() => SaveFormation(FormationSorter));

            var characterButton = FormationSorter.transform.Find("Character0").gameObject;
            var buttons = new List<GameObject>();

            for (int i = 0; i < PlayData.TSavedata.Party.Count; i++)
            {
                var button = i == 0 ? characterButton : Object.Instantiate(characterButton, FormationSorter.transform);
                button.name = "Character" + i.ToString();
                var img = button.transform.Find("Icon").GetComponent<Image>();
                GetCharacterSprite(PlayData.TSavedata.Party[i].KeyData, img);
                buttons.Add(button);
            }
            var order = FormationCV.Instance.order;
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].transform.SetSiblingIndex(order[i]);
            }
        }

        private void SaveFormation(GameObject formationSorter)
        {
            List<int> formation = new List<int>();
            for (int i = 0; i < PlayData.TSavedata.Party.Count; i++)
            {
                var button = formationSorter.transform.Find("Character" + i.ToString());
                var ind = button != null ? button.GetSiblingIndex() : -1;
                formation.Add(ind);
            }
            FormationCV.Instance.SetOrder(formation);
        }

        public static void GetCharacterSprite(string key, Image img)
        {
            try
            {
                var data = new GDECharacterData(key);
                if (string.IsNullOrEmpty(data.CollectionSprite_SkillFace_Path)) return;
                AddressableLoadManager.LoadAsyncAction(data.CollectionSprite_SkillFace_Path,
                    AddressableLoadManager.ManageType.Character, img);
            }
            catch { }
        }
    }
}
