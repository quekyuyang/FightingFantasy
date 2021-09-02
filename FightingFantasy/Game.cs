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
                new Exception("Invalid game state!");
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
