using System;
using System.Collections.Generic;
using System.Text;

namespace FightingFantasy
{
    class Game
    {
        static private Dictionary<int, JsonChapter> chapters;
        static private Chapter current_chapter;
        static private Protagonist protag;

        static public void Start(Dictionary<int, JsonChapter> chapters)
        {
            Game.chapters = chapters;
            protag = new Protagonist();
            int chapter_n = 1;
            GoToChapter(chapter_n);
        }

        static public void Continue(string input)
        {
            current_chapter.Continue(input);
            if (!current_chapter.IsActive)
                GoToChapter(current_chapter.NextChapter);
        }

        static private void GoToChapter(int chapter_n)
        {
            if (chapters[chapter_n].type == "choices")
                current_chapter = new ChoiceChapter(chapters[chapter_n].story, chapters[chapter_n].choices);
            else if (chapters[chapter_n].type == "story_only")
                current_chapter = new StoryOnlyChapter(chapters[chapter_n].story, chapters[chapter_n].next_chapter);
            else if (chapters[chapter_n].type == "prebattle_choices")
                current_chapter = new BattleChapter(chapters[chapter_n].story, protag, chapters[chapter_n].enemies, chapters[chapter_n].next_chapter);

            if (chapters[chapter_n].stat_changes != null)
                ApplyStatChanges(chapters[chapter_n].stat_changes);
        }

        static private void ApplyStatChanges(object[][] stat_changes)
        {
            foreach (object[] stat_change in stat_changes)
            {
                if ((string)stat_change[0] == "stamina")
                    protag.stamina += (int)(long)stat_change[1];
                else if ((string)stat_change[0] == "skill")
                    protag.skill += (int)(long)stat_change[1];
                else if ((string)stat_change[0] == "luck")
                    protag.luck += (int)(long)stat_change[1];
            }
        }

        static public Type GetChapterType() => current_chapter.GetType();
        static public string GetStory() => current_chapter.Story;
        static public (int,int,int) GetProtagStats() => (protag.stamina,protag.skill,protag.luck);
        static public (string,int,int) GetEnemyStats()
        {
            if (current_chapter is BattleChapter battle_chapter)
                return battle_chapter.GetEnemyStats();
            else
                return ("", -1, -1);
        }
        static public string[] GetChoices()
        {
            return current_chapter.GetChoices();
        }

        static public string GetMessage()
        {
            return current_chapter.Message;
        }
    }

    class Battle
    {
        private Protagonist protag;
        private Enemy enemy;
        public string State { get; set; }
        private bool ExpectInput { get; set; }
        public string Message { get; set; }
        public bool BattleEnded { get; set; }

        public Battle(Protagonist protag, Enemy enemy)
        {
            this.protag = protag;
            this.enemy = enemy;

            State = "";
            ExpectInput = false;
            Message = "";
            BattleEnded = false;
        }

        public void RunNextRound(int choice_n)
        {
            Message = "";
            if (!ExpectInput)
            {
                if(protag.AttackStrength() > enemy.AttackStrength())
                {
                    enemy.stamina -= 2;
                    State = "wounding";
                    Message = "You have wounded the enemy! Test your luck?";
                }
                else
                {
                    protag.stamina -= 2;
                    State = "wounded";
                    Message = "You have been wounded! Test your luck?";
                }

                ExpectInput = true;
            }
            else
            {
                if (choice_n == 1)
                    ApplyLuck(protag.TestLuck());
                ExpectInput = false;
            }

            if (EnemyDead())
            {
                Message = $"You defeated {enemy.name}!";
                ExpectInput = false;
                BattleEnded = true;
            }
        }

        private void ApplyLuck(bool is_lucky)
        {
            if (State == "wounding")
            {
                if (is_lucky)
                {
                    enemy.stamina -= 2;
                    Message = "You dealt a critical blow!";
                }
                else
                {
                    enemy.stamina += 1;
                    Message = "The enemy's wound was a mere graze..";
                }

            }
            else if (State == "wounded")
            {
                if (is_lucky)
                {
                    protag.stamina += 1;
                    Message = "You managed to avoid the full damage!";
                }
                else
                {
                    protag.stamina -= 1;
                    Message = "You suffered a critical blow!";
                }

            }
        }

        public string[] GetChoices()
        {
            if (ExpectInput)
                return new string[] { "Yes", "No" };
            else
                return new string[0];
        }

        private bool EnemyDead()
        {
            return enemy.stamina <= 0;
        }
    }
}
