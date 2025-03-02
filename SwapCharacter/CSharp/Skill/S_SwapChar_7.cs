using System.Collections.Generic;

namespace SwapCharacter
{
    /// <summary>
    /// Distraction Hologram
    /// Select: Create a hologram on an ally, or create a new hologram character.
    /// </summary>
    public class S_SwapChar_7:Skill_Extended
    {

        public override void Init()
        {
            base.Init();
            ChoiceSkillList = new List<string>
            {
                ModItemKeys.Skill_S_SwapChar_7_1,
                ModItemKeys.Skill_S_SwapChar_7_2
            };
        }
    }
}