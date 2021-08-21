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
            JsonChapter chapter_data = chapters[chapter_n];
            if (chapter_data.type == "choices")
            {
                int n_choices = chapter_data.choices.Length;
                var choices = new List<(string, int)>();
                for (int i = 0; i < n_choices; i++)
                    choices.Add(((string)chapter_data.choices[i][0],(int)(long)chapter_data.choices[i][1]));
                current_chapter = new ChoiceChapter(chapter_data.story, protag, choices, chapter_data.stat_changes);
            }
                
            else if (chapter_data.type == "story_only")
                current_chapter = new StoryOnlyChapter(chapter_data.story, protag, chapter_data.next_chapter, chapter_data.stat_changes);
            else if (chapter_data.type == "prebattle_choices")
                current_chapter = new BattleChapter(chapter_data.story, protag, chapter_data.enemies, chapter_data.next_chapter, chapter_data.stat_changes);
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
        static public List<string> GetChoices()
        {
            return current_chapter.GetChoices();
        }

        static public List<string> GetMessages()
        {
            return current_chapter.GetMessages();
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

        public List<string> GetChoices()
        {
            if (ExpectInput)
                return new List<string>{ "Yes", "No" };
            else
                return new List<string>();
        }

        private bool EnemyDead()
        {
            return enemy.stamina <= 0;
        }
    }
}
