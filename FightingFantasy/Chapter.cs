using System;
using System.Collections.Generic;
using System.Text;

namespace FightingFantasy
{
    class JsonChapter
    {
        public string type;
        public string story;
        public int next_chapter;
        public object[][] choices;
        public List<Enemy> enemies;
    }

    class Chapter
    {
        public string Story { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public int NextChapter { get; set; }

        public virtual void Continue(string input){}

        public virtual string[] GetChoices()
        {
            return new string[0];
        }
    }

    class StoryOnlyChapter : Chapter
    {
        public StoryOnlyChapter(string story, int next_chapter)
        {
            Story = story;
            Message = "";
            NextChapter = next_chapter;
            IsActive = true;
        }

        public override void Continue(string input)
        {
            IsActive = false;
        }
    }

    class ChoiceChapter : Chapter
    {
        public object[][] Choices { get; set; }
        
        public ChoiceChapter(string story, object[][] choices)
        {
            Story = story;
            Message = "";
            Choices = choices;
            IsActive = true;
        }

        public override void Continue(string input)
        {
            int player_choice;
            if (Int32.TryParse(input, out player_choice) && player_choice <= Choices.Length && player_choice >= 1)
            {
                long temp = (long)Choices[player_choice - 1][1];
                NextChapter = (int)temp;
                IsActive = false;
            }
            else
            {
                Message = "Invalid choice. Please enter a number corresponding to one of the choices above.";
            }
        }

        public override string[] GetChoices()
        {
            int n_choices = Choices.Length;
            string[] choices = new string[n_choices];
            for (int i = 0; i < n_choices; i++)
                choices[i] = (string)Choices[i][0];
            return choices;
        }
    }

    class BattleChapter : Chapter
    {
        public List<Enemy> enemies;
        public string State { get; set; }
        public Protagonist protag;
        public object[][] choices { get; set; }
        private bool paused;
        private Battle battle;

        public BattleChapter(string story, Protagonist protag, List<Enemy> enemies, int next_chapter)
        {
            Story = story;
            Message = "";
            this.enemies = enemies;
            this.protag = protag;
            NextChapter = next_chapter;
            paused = true;
            IsActive = true;

            battle = new Battle(protag, enemies[0]);
        }

        public override string[] GetChoices()
        {
            return battle.GetChoices();
        }

        public override void Continue(string input)
        {
            Message = "";

            if (battle.BattleEnded)
            {
                ReadyNextEnemy();
                return;
            }
                
            int player_choice;
            if (!Int32.TryParse(input, out player_choice) || player_choice > 2 || player_choice < 1)
            {
                Message = "Invalid choice. Please enter a number corresponding to one of the choices above.";
                return;
            }

            battle.RunNextRound(player_choice);
            Message = battle.Message;
        }

        private void ReadyNextEnemy()
        {
            enemies.RemoveAt(0);
            if (enemies.Count == 0)
                IsActive = false;
            else
                battle = new Battle(protag,enemies[0]);
        }

        public (string,int,int) GetEnemyStats()
        {
            Enemy enemy = enemies[0];
            return (enemy.name,enemy.stamina, enemy.skill);
        }
    }

    class Dice
    {
        public static Random random = new Random();
        public static List<int> Roll(int n_dice)
        {
            List<int> dice_rolls = new List<int>();
            for (int i = 0; i < n_dice; i++)
                dice_rolls.Add(random.Next(1, 7));
            return dice_rolls;
        }

        public static int SumRoll(int n_dice)
        {
            int dice_sum = 0;
            for (int i = 0; i < n_dice; i++)
                dice_sum += random.Next(1, 7);
            return dice_sum;
        }
    }
}
