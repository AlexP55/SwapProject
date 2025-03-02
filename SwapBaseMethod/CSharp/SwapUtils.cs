using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DarkTonic.MasterAudio;
using GameDataEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SwapBaseMethod
{
    public static class SwapUtils
    {
        const float AnimationTime = 0.5f;
        const float ShiftRange = 100f;
        const float DistanceTol = 0.1f;

        public static IEnumerator ChangePosition(Dictionary<BattleChar, int> allyPos, AnimationType type = AnimationType.Shift, float animationTime = AnimationTime, string sound = null)
        {
            if (allyPos.Count <= 0) yield break;
            if (allyPos.Keys.Any(ally => !ally.Info.Ally || ally.transform.parent == null ||
                ally.transform.parent != allyPos.Keys.First().transform.parent))
            {
                yield break;
            }
            var oldPos = BattleSystem.instance.AllyTeam.Chars.ToList();
            var inds = new int[BattleSystem.instance.AllyTeam.Skills_Basic.Length];
            Array.Fill(inds, int.MaxValue);
            for (int i = 0; i < oldPos.Count; i++)
            {
                if (allyPos.ContainsKey(oldPos[i])) inds[i] = allyPos[oldPos[i]];
                else inds[i] = i;
            }
            var newPos = oldPos.OrderBy(ally => inds[oldPos.IndexOf(ally)]).ToList();
            var indArray = new int[BattleSystem.instance.AllyTeam.Skills_Basic.Length];
            inds.CopyTo(indArray, 0);

            var allyChanged = new List<BattleChar>();
            var animatedAlly = (type == AnimationType.Shuffle) ? allyPos.Keys.ToList() : new List<BattleChar>();
            for (int i = 0; i < oldPos.Count; i++)
            {
                if (oldPos[i] != newPos[i])
                {
                    allyChanged.Add(oldPos[i]);
                    if (!animatedAlly.Contains(oldPos[i]))
                    {
                        animatedAlly.Add(oldPos[i]);
                    }
                }
            }
            var directions = animatedAlly.Select(ally => 
                (oldPos[newPos.IndexOf(ally)].transform.localPosition - ally.transform.localPosition).normalized).ToList();

            if (animatedAlly.Count <= 0) yield break;
            var battleText = GetAllyText(allyChanged);

            // play audio if available
            if (!string.IsNullOrEmpty(sound)) MasterAudio.PlaySound(sound);
            switch (type)
            {
                case AnimationType.Shuffle:
                    yield return ShuffleAnimationBegin(animatedAlly, animationTime / 2);
                    break;
                case AnimationType.Shift:
                    yield return ShiftAnimationBegin(animatedAlly, directions, animationTime / 2);
                    break;
            }

            // change gameobject positions
            BattleSystem.instance.AllyTeam.Chars = newPos;
            Array.Sort(indArray, BattleSystem.instance.AllyTeam.Skills_Basic);
            for (int i = 0; i < BattleSystem.instance.AllyTeam.Chars.Count; i++)
            {
                BattleSystem.instance.AllyTeam.Chars[i].transform.SetSiblingIndex(i);
            }

            PosTargetChange(oldPos, newPos);
            foreach (var ally in allyChanged)
            {
                var oldP = oldPos.IndexOf(ally);
                var newP = newPos.IndexOf(ally);
                foreach (var ip_PosChanged in ally.IReturn<IP_PositionChanged>())
                {
                    ip_PosChanged?.PositionChanged(oldP, newP);
                }
                PositionChangeLogs.AddLog(ally, oldP, newP);
            }
            if (allyChanged.Count == 2 && animatedAlly.Count == 2)
            {
                foreach (var ip_Swapped in BattleSystem.instance.IReturn<IP_Swapped>())
                {
                    ip_Swapped?.Swapped(allyChanged[0], allyChanged[1]);
                }
            }
            PositionChangeLogs.NewInd();

            yield return new WaitForEndOfFrame();
            SetAllyText(battleText);

            switch (type)
            {
                case AnimationType.Shuffle:
                    yield return ShuffleAnimationEnd(animatedAlly, animationTime / 2);
                    break;
                case AnimationType.Shift:
                    yield return ShiftAnimationEnd(animatedAlly, directions, animationTime / 2);
                    break;
            }
        }

        public static IEnumerator ChangePositionEnemy(Dictionary<BattleChar, int> enemyPos, float animationTime = AnimationTime, string sound = null)
        {
            if (enemyPos.Count <= 0) yield break;
            if (enemyPos.Keys.Any(enemy => enemy.Info.Ally || enemy.transform.parent == null ||
                enemy.transform.parent != enemyPos.Keys.First().transform.parent))
            {
                yield break;
            }
            var oldPos = BattleSystem.instance.EnemyTeam.AliveChars_Vanish.ToList();
            var enemySorted = oldPos.OrderBy(enemy => enemy.GetLocalPos().x).ToList();
            var inds = new int[BattleSystem.instance.EnemyTeam.Chars.Count];
            for (int i = 0; i < oldPos.Count; i++)
            {
                if (enemyPos.ContainsKey(oldPos[i])) inds[i] = enemyPos[oldPos[i]];
                else inds[i] = i;
            }
            var newPos = oldPos.OrderBy(ally => inds[oldPos.IndexOf(ally)]).ToList();

            var enemyChanged = new List<BattleChar>();
            for (int i = 0; i < oldPos.Count; i++)
            {
                if (oldPos[i] != newPos[i])
                {
                    enemyChanged.Add(oldPos[i]);
                }
            }

            if (enemyChanged.Count <= 0) yield break;
            var originalPos = oldPos.Select(bc => bc.GetLocalPos()).ToList();

            // play audio if available
            if (!string.IsNullOrEmpty(sound)) MasterAudio.PlaySound(sound);

            // change gameobject positions
            enemyChanged.ForEach(bc => bc.MoveTo(originalPos[newPos.IndexOf(bc)]));
            BattleSystem.instance.EnemyTeam.Chars = newPos;
            PosTargetChange(oldPos, newPos);

            foreach (var enemy in enemyChanged)
            {
                var oldP = enemySorted.IndexOf(enemy);
                var newP = enemySorted.IndexOf(oldPos[newPos.IndexOf(enemy)]);
                foreach (var ip_PosChanged in enemy.IReturn<IP_PositionChanged>())
                {
                    ip_PosChanged?.PositionChanged(oldP, newP);
                }
                PositionChangeLogs.AddLog(enemy, oldP, newP);
            }
            if (enemyChanged.Count == 2)
            {
                foreach (var ip_Swapped in BattleSystem.instance.IReturn<IP_Swapped>())
                {
                    ip_Swapped?.Swapped(enemyChanged[0], enemyChanged[1]);
                }
            }
            PositionChangeLogs.NewInd();

            yield return new WaitForSeconds(animationTime);
        }

        private static IEnumerator ShuffleAnimationBegin(List<BattleChar> allies, float animationTime)
        {
            allies.ForEach(ally => ResetPosition(ally, false));
            float time = 0;
            while (time < animationTime)
            {
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
                var scale = Mathf.Cos(Mathf.PI / 2 * time / animationTime);
                allies.ForEach(ally => ally.transform.localScale = new Vector3(scale, 1, 1));
            }
            yield break;
        }

        private static IEnumerator ShuffleAnimationEnd(List<BattleChar> allies, float animationTime)
        {
            float time = 0;
            while (time < animationTime)
            {
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
                var scale = Mathf.Sin(Mathf.PI / 2 * time / animationTime);
                allies.ForEach(ally => ally.transform.localScale = new Vector3(scale, 1, 1));
            }
            allies.ForEach(ally => ResetPosition(ally, true));
            yield break;
        }

        private static IEnumerator ShiftAnimationBegin(List<BattleChar> allies, List<Vector3> directions, float animationTime)
        {
            allies.ForEach(ally => ResetPosition(ally, false));
            var oldPos = allies.Select(ally => ally.transform.localPosition).ToList();
            float time = 0;
            while (time < animationTime)
            {
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
                var scale = Mathf.Cos(Mathf.PI / 2 * time / animationTime);
                for (int i = 0; i < allies.Count; i++)
                {
                    allies[i].transform.localPosition = oldPos[i] + (1 - scale) * ShiftRange * directions[i];
                    allies[i].gameObject.GetOrAddComponent<CanvasGroup>().alpha = scale;
                }
            }
            yield break;
        }

        private static IEnumerator ShiftAnimationEnd(List<BattleChar> allies, List<Vector3> directions, float animationTime)
        {
            float time = 0;
            var oldPos = allies.Select(ally => ally.transform.localPosition).ToList();
            while (time < animationTime)
            {
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
                var scale = Mathf.Sin(Mathf.PI / 2 * time / animationTime);
                for (int i = 0; i < allies.Count; i++)
                {
                    allies[i].transform.localPosition = oldPos[i] + (scale - 1) * ShiftRange * directions[i];
                    allies[i].gameObject.GetOrAddComponent<CanvasGroup>().alpha = scale;
                }
            }
            for (int i = 0; i < allies.Count; i++)
            {
                ResetPosition(allies[i], true);
                allies[i].transform.localPosition = oldPos[i];
            }
            yield break;
        }

        public static IEnumerator SwapChars(BattleChar bc1, BattleChar bc2, AnimationType type = AnimationType.Shift, float animationTime = AnimationTime, string sound = null)
        {
            if (bc1 == null || bc2 == null || bc1.MyTeam != bc2.MyTeam) yield break;
            var team = bc1.MyTeam.Chars;
            var ind1 = team.IndexOf(bc1);
            var ind2 = team.IndexOf(bc2);
            if (ind1 < 0 || ind2 < 0 || ind1 == ind2) yield break;
            if (bc1.Info.Ally)
            {
                yield return ChangePosition(new Dictionary<BattleChar, int> {
                    { bc1, ind2 },
                    { bc2, ind1 }
                }, type, animationTime, sound);
            }
            else
            {
                yield return ChangePositionEnemy(new Dictionary<BattleChar, int> {
                    { bc1, ind2 },
                    { bc2, ind1 }
                }, animationTime, sound);
            }
            yield break;
        }

        public static IEnumerator Shuffle(List<BattleChar> characters, AnimationType type = AnimationType.Shuffle, float animationTime = AnimationTime, string sound = null)
        {
            if (characters.Count <= 1 || characters.Any(bc => bc.MyTeam != characters[0].MyTeam)) yield break;
            var inds = characters.Select(ally => characters[0].MyTeam.Chars.IndexOf(ally)).ToList();
            inds = RandomManager.Shuffle("CharacterShuffle", inds);
            var pos = characters.Zip(inds, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
            if (characters[0].Info.Ally)
            {
                yield return ChangePosition(pos, type, animationTime, sound);
            }
            else
            {
                yield return ChangePositionEnemy(pos, animationTime, sound);
            }
            yield break;
        }

        public static IEnumerator Shift(BattleChar character, int toPos, AnimationType type = AnimationType.Shift, float animationTime = AnimationTime, string sound = null)
        {
            if (character == null) yield break;
            if (toPos < 0) toPos = 0;
            if (character.Info.Ally)
            {
                var team = character.MyTeam.Chars;
                if (toPos >= team.Count) toPos = team.Count - 1;
                int fromPos = team.IndexOf(character);
                if (fromPos < 0 || fromPos == toPos) yield break;
                var pos = new Dictionary<BattleChar, int> { { character, toPos } };
                if (fromPos < toPos)
                {
                    for (int i = fromPos + 1; i <= toPos; i++)
                    {
                        pos.Add(team[i], i - 1);
                    }
                }
                else
                {
                    for (int i = fromPos - 1; i >= toPos; i--)
                    {
                        pos.Add(team[i], i + 1);
                    }
                }
                yield return ChangePosition(pos, type, animationTime, sound);
            }
            else
            {
                var team = character.MyTeam.AliveChars;
                if (toPos >= team.Count) toPos = team.Count - 1;
                var teamSorted = team.OrderBy(enemy => enemy.GetLocalPos().x).ToList();
                var teamOrigin = character.MyTeam.AliveChars_Vanish;
                int fromPos = teamSorted.IndexOf(character);
                if (fromPos < 0 || fromPos == toPos) yield break;
                var pos = new Dictionary<BattleChar, int> { { character, teamOrigin.IndexOf(teamSorted[toPos]) } };
                if (fromPos < toPos)
                {
                    for (int i = fromPos + 1; i <= toPos; i++)
                    {
                        pos.Add(teamSorted[i], teamOrigin.IndexOf(teamSorted[i - 1]));
                    }
                }
                else
                {
                    for (int i = fromPos - 1; i >= toPos; i--)
                    {
                        pos.Add(teamSorted[i], teamOrigin.IndexOf(teamSorted[i + 1]));
                    }
                }
                yield return ChangePositionEnemy(pos, animationTime, sound);
            }
            yield break;
        }

        private static void ResetPosition(BattleChar ally, bool enableShake)
        {
            ally.transform.localScale = Vector3.one;
            ally.gameObject.GetOrAddComponent<CanvasGroup>().alpha = 1;
            ally.UI.CharShake.Stop();
            ally.UI.CharShake.enabled = enableShake;
        }

        private static void PosTargetChange(List<BattleChar> oldPos, List<BattleChar> newPos)
        {
            var castingSkills = new List<CastingSkill>();
            castingSkills.AddRange(BattleSystem.instance.EnemyCastSkills);
            castingSkills.AddRange(BattleSystem.instance.CastSkills);
            castingSkills.AddRange(BattleSystem.instance.SaveSkill);
            castingSkills = castingSkills.Distinct().ToList();
            foreach (var skill in castingSkills)
            {
                if (skill.skill.MySkill.Target.Key == GDEItemKeys.s_targettype_enemy ||
                    skill.skill.MySkill.Target.Key == GDEItemKeys.s_targettype_all_onetarget ||
                    skill.skill.MySkill.Target.Key == GDEItemKeys.s_targettype_ally ||
                    skill.skill.MySkill.Target.Key == GDEItemKeys.s_targettype_otherally ||
                    skill.skill.MySkill.Target.Key == GDEItemKeys.s_targettype_self)
                { // skills with one target
                    var ind = oldPos.IndexOf(skill.Target);
                    if (ind >= 0)
                    {
                        skill.Target = newPos[ind];
                    }
                }
                else if (skill.skill.MySkill.Target.Key == GDEItemKeys.s_targettype_enemy_PlusRandom)
                { // skills with two targets
                    var ind1 = oldPos.IndexOf(skill.Target);
                    if (ind1 >= 0)
                    {
                        skill.Target = newPos[ind1];
                    }
                    var ind2 = oldPos.IndexOf(skill.TargetPlus);
                    if (ind2 >= 0)
                    {
                        skill.TargetPlus = newPos[ind2];
                    }
                }
            }
        }

        public static bool SamePosition(BattleAlly ally, BattleEnemy enemy)
        {
            if (ally == null || ally.IsDead || enemy == null || enemy.IsDead) return false;
            var allyInd = ally.MyTeam.AliveChars.IndexOf(ally);
            enemy.EnemyPosNum(out int enemyInd);
            return allyInd == enemyInd;
        }

        public static void MoveTo(this Buff buff, BattleChar bc, bool StringHide = false)
        {
            if (bc == null || bc.IsDead || bc == buff.BChar) return;
            IEnumerator Coroutine()
            {
                yield return new WaitForEndOfFrame();
                Buff newBuff = null;
                foreach (var stack in buff.StackInfo)
                {
                    var addedBuff = bc.BuffAdd(buff.BuffData.Key, buff.Usestate_L,
                        hide: buff.IsHide, PlusTagPer: 999, RemainTime: buff.LifeTime, StringHide: StringHide);
                    if (addedBuff != null && addedBuff.StackInfo.Count > 0)
                    {
                        addedBuff.StackInfo.Last().RemainTime = stack.RemainTime;
                        newBuff = addedBuff;
                    }
                }
                if (newBuff != null)
                {
                    newBuff.BarrierHP = buff.BarrierHP;
                    if (buff is IP_StatsStable)
                    {
                        newBuff.PlusStat = buff.PlusStat;
                        newBuff.PlusPerStat = buff.PlusPerStat;
                    }
                }
                yield break;
            }
            BattleSystem.instance.StartCoroutine(Coroutine());
            buff.SelfDestroy();
        }

        private static Dictionary<BattleChar, List<BattleText>> GetAllyText(List<BattleChar> allies)
        {
            var result = new Dictionary<BattleChar, List<BattleText>>();
            foreach (var text in BattleSystem.instance.MainUICanvas.GetComponentsInChildren<BattleText>())
            {
                var textPos = text.transform.TransformPoint(Vector3.Scale(new Vector3(150f, 0f, 0f), text.transform.localScale.Invert()));
                foreach (var ally in allies)
                {
                    if (Vector3.Distance(textPos, ally.GetTopPos()) < DistanceTol)
                    {
                        result.TryAdd(ally, new List<BattleText>());
                        result[ally].Add(text);
                        break;
                    }
                }
            }
            return result;
        }

        private static void SetAllyText(Dictionary<BattleChar, List<BattleText>> allyTexts)
        {
            foreach (var entry in allyTexts)
            {
                var ally = entry.Key;
                var texts = entry.Value;
                foreach (var text in texts)
                {
                    if (text == null) continue;
                    text.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                    text.transform.rotation = Quaternion.identity;
                    text.transform.position = ally.GetTopPos();
                    text.transform.GetComponent<RectTransform>().localPosition -= new Vector3(150f, 0f, 0f);
                }
            }
        }

        public static void SetAlliesTransform()
        {
            var allies = BattleSystem.instance.AllyTeam.Chars;
            if (allies.Count < 0) return;
            Transform parent = allies[0].transform.parent;
            var count = parent.Cast<Transform>().Count(child => child.gameObject.activeSelf);
            if (count < 0) return;
            var scale = count <= 4 ? 1f : 4f / count;
            parent.localPosition = new Vector3(269f - 1000f * (1 - scale),
                parent.localPosition.y, parent.localPosition.z);
            parent.localScale = new Vector3(scale, scale, 1f);
        }

        private static IEnumerator DelaySetPos()
        {
            yield return new WaitForEndOfFrame();
            SetAlliesTransform();
            yield break;
        }

        public static Vector3 GetLocalPos(this BattleChar bc)
        {
            if (bc is BattleEnemy enemy)
            {
                var move = enemy.PosObject.GetComponent<SmoothMove>();
                if (move != null && move.enabled && (move.Enable || move.AllowEnable))
                {
                    return move.SavePos;
                }
                return enemy.PosObject.transform.localPosition;
            }
            else
            {
                return bc.transform.localPosition;
            }
        }

        public static void MoveTo(this BattleChar bc, Vector3 pos)
        {
            if (bc is BattleEnemy enemy)
            {
                var move = enemy.PosObject.GetComponent<SmoothMove>();
                move.Move(pos);
                move.enabled = true;
                move.AllowEnable = true;
                IEnumerator DelayStop()
                {
                    var t = 0f;
                    while (t < 2f)
                    {
                        t += Time.deltaTime;
                        move.AllowEnable = true;
                        yield return new WaitForEndOfFrame();
                    }
                    move.AllowEnable = false;
                }
                move.StartCoroutine(DelayStop());
            }
            else
            {
                bc.transform.localPosition = pos;
            }
        }

        public static BattleAlly NewAlly(string charKey, string basicSkill = null)
        {
            Character character = new Character();
            character.Set_AllyData(new GDECharacterData(charKey));
            if (basicSkill != null) character.BasicSkill = new CharInfoSkillData(basicSkill);
            BattleAlly battleAlly = BattleSystem.instance.CreatTempAlly(character);
            if (battleAlly.BasicSkill != null)
            {
                battleAlly.MyBasicSkill.SkillInput(battleAlly.BasicSkill.CloneSkill(true));
                battleAlly.BattleBasicskillRefill = battleAlly.BasicSkill.CloneSkill(true);
            }
            var ind = BattleSystem.instance.AllyTeam.Chars.IndexOf(battleAlly);
            if (ind >= 0)
            {
                BattleSystem.instance.AllyTeam.Skills_Basic =
                    BattleSystem.instance.AllyTeam.Skills_Basic.SetAt(ind, battleAlly.BasicSkill.CloneSkill(true));
            }
            battleAlly.Info.Equip.Clear();
            BattleSystem.instance.StartCoroutine(DelaySetPos());
            return battleAlly;
        }

        public static void RemoveAlly(BattleAlly battleAlly)
        {
            var ind = BattleSystem.instance.AllyTeam.Chars.IndexOf(battleAlly);
            if (ind >= 0)
            {
                BattleSystem.instance.AllyTeam.Chars.RemoveAt(ind);
                BattleSystem.instance.AllyTeam.Skills_Basic.RemoveAt(ind);
            }
            Object.Destroy(battleAlly.gameObject);
            BattleSystem.instance.StartCoroutine(DelaySetPos());
        }

        public static Vector3 Invert(this Vector3 vec)
        {
            return new Vector3(1 / vec.x, 1 / vec.y, 1 / vec.z);
        }

        public static void RemoveAt<T>(this T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                arr[a] = arr[a + 1];
            }
            arr[arr.Length - 1] = default;
        }

        public static T[] SetAt<T>(this T[] arr, int index, T value)
        {
            if (index < 0) return arr;
            if (index < arr.Length - 1)
            {
                arr[index] = value;
                return arr;
            }
            var result = new T[index + 1];
            Array.Copy(arr, result, arr.Length);
            result[index] = value;
            return result;
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            return go.GetComponent<T>() ?? go.AddComponent<T>();
        }

        public static T GetOrAddBattleValue<T>(this BattleSystem ins) where T : class
        {
            var bv = ins.GetBattleValue<T>();
            if (bv == null)
            {
                bv = Activator.CreateInstance<T>();
                ins.BattleValues.Add(bv);
            }
            return bv;
        }

        public static List<BattleChar> SortedEnemies(this BattleSystem ins)
        {
            return ins.EnemyTeam.AliveChars_Vanish.OrderBy(enemy => enemy.GetLocalPos().x).ToList();
        }

    }

    public enum AnimationType
    {
        Shuffle,
        Shift
    }
}
