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

        static public void Continue()
        {
            if (current_chapter is StoryOnlyChapter story_chapter)
                GoToChapter(story_chapter.next_chapter);
            else
                Console.WriteLine("Attempt to continue in non story only chapter");
        }

        static public void Choose(int choice_n)
        {
            if (current_chapter is ChoiceChapter choice_chapter)
            {
                long chapter_n = (long)choice_chapter.choices[choice_n - 1][1];
                GoToChapter((int)chapter_n);
            }
            else
                Console.WriteLine("Attempt to choose in non choice chapter");
        }

        static private void GoToChapter(int chapter_n)
        {
            if (chapters[chapter_n].type == "choices")
                current_chapter = new ChoiceChapter(chapters[chapter_n].story, chapters[chapter_n].choices);
            else if (chapters[chapter_n].type == "story_only")
                current_chapter = new StoryOnlyChapter(chapters[chapter_n].story, chapters[chapter_n].next_chapter);
            else if (chapters[chapter_n].type == "prebattle_choices")
            {
                current_chapter = new BattleChapter(chapters[chapter_n].story, protag, chapters[chapter_n].enemies, chapters[chapter_n].next_chapter);
                battle = new Battle(protag, chapters[chapter_n].enemies);
            }
                
        }

        static public Type GetChapterType() => current_chapter.GetType();
        static public string GetStory() => current_chapter.story;
        static public (int,int,int) GetProtagStats() => (protag.stamina,protag.skill,protag.luck);
        static public string[] GetChoices()
        {
            if (current_chapter is StoryOnlyChapter story_chapter)
            {
                return new string[0];
            }
            else if (current_chapter is ChoiceChapter choice_chapter)
            {
                int n_choices = choice_chapter.choices.Length;
                string[] choices = new string[n_choices];
                for (int i = 0; i < n_choices; i++)
                    choices[i] = (string)choice_chapter.choices[i][0];
                return choices;
            }
            else
                throw new InvalidOperationException("Attempt to get choices when current chapter has none");
        }

        static public bool BattleOngoing()
        {
            return (battle != null);
        }
    }

    class Battle
    {
        public bool BattleOngoing { get; set; }
        private Protagonist protag;
        private Enemy current_enemy;
        private Enemy[] enemies;
        public string State { get; set; } // How to make getter only from outside but retain setter for internal use?
        private string[] choices;

        public Battle(Protagonist protag, Enemy[] enemies)
        {
            this.BattleOngoing = true;
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

        public void EndBattle()
        {
            BattleOngoing = false;
        }
    }
}
