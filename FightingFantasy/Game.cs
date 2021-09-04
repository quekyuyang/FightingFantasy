using System;
using System.Collections.Generic;
using System.Text;

namespace FightingFantasy
{
    class Game
    {
        static private ChapterFactory chapter_factory;
        static private Chapter current_chapter;
        static private Protagonist protag;
        static public StateEnum State
        {
            get;
            private set;
        }

        public enum StateEnum
        {
            Normal,
            Items,
            Battle,
            GameOver
        }

        static public void Start(ChapterFactory chapter_factory)
        {
            Game.chapter_factory = chapter_factory;
            protag = new Protagonist();
            State = StateEnum.Normal;
            int chapter_n = 1;
            GoToChapter(chapter_n);
        }

        static public void Continue(string input)
        {
            if (State == StateEnum.Normal || State == StateEnum.Battle)
            {
                if (protag.stamina <= 0)
                {
                    State = StateEnum.GameOver;
                    return;
                }

                int input_num = 0;
                if (Int32.TryParse(input, out input_num))
                    current_chapter.Continue(input_num);
                else if (input == "i")
                {
                    State = StateEnum.Items;
                    return;
                }
                else if (input == "q")
                    System.Environment.Exit(1);

                if (current_chapter.Ended)
                    GoToChapter(current_chapter.NextChapter);

                if (current_chapter.InBattle)
                    State = StateEnum.Battle;
                else
                    State = StateEnum.Normal;
            }
            else if (State == StateEnum.Items)
            {
                State = StateEnum.Normal;
            }
            else if (State == StateEnum.GameOver)
                System.Environment.Exit(1);
            else
                throw new Exception("Invalid game state!");
        }

        static private void GoToChapter(int chapter_n)
        {
            current_chapter = chapter_factory.CreateChapter(chapter_n, protag);
        }

        static public string GetStory() => current_chapter.GetStory();
        static public (int,int,int) GetProtagStats() => (protag.stamina,protag.skill,protag.luck);
        static public Dictionary<Item, int> GetProtagInventory() => protag.Inventory;
        static public (string,int,int) GetEnemyStats()
        {
            if (current_chapter.CurrentEvent is BattleEvent battle_event)
                return battle_event.GetEnemyStats();
            else
                throw new Exception("Attempt to get enemy stats from non-battle event");
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
}
