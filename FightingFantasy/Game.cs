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
        static private Battle battle;

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
        private Enemy current_enemy;
        private Enemy[] enemies;
        public string State { get; set; } // How to make getter only from outside but retain setter for internal use?
        private string[] choices;

        public Battle(Protagonist protag, Enemy[] enemies)
        {
            this.protag = protag;
            this.enemies = enemies;
            this.current_enemy = enemies[0];
        }

        public void Continue()
        {
            if (protag.AttackStrength() > current_enemy.AttackStrength())
            {
                current_enemy.TakeDmg(protag.DealDmg());
                State = "wounding";
            }
            else
            {
                protag.TakeDmg(current_enemy.DealDmg());
                State = "wounded";
            }
        }

        public string[] GetChoices()
        {
            return choices;
        }
    }
}
