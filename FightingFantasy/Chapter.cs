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
        public object[][] stat_changes;
    }

    class Chapter
    {
        public string Story { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public int NextChapter { get; set; }
        public Protagonist protag;

        public Chapter(string story, Protagonist protag, object[][] stat_changes)
        {
            Story = story;
            Message = "";
            IsActive = true;
            this.protag = protag;

            if (stat_changes != null)
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
        }

        public virtual void Continue(string input){}

        public virtual string[] GetChoices()
        {
            return new string[0];
        }
    }

    class StoryOnlyChapter : Chapter
    {
        public StoryOnlyChapter(string story, Protagonist protag, int next_chapter, object[][] stat_changes)
            : base(story,protag,stat_changes)
        {
            NextChapter = next_chapter;
        }

        public override void Continue(string input)
        {
            IsActive = false;
        }
    }

    class ChoiceChapter : Chapter
    {
        public object[][] Choices { get; set; }
        
        public ChoiceChapter(string story, Protagonist protag, object[][] choices, object[][] stat_changes)
            : base(story,protag,stat_changes)
        {
            Choices = choices;
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
        
        public object[][] choices { get; set; }
        private bool paused;
        private Battle battle;

        public BattleChapter(string story, Protagonist protag, List<Enemy> enemies, int next_chapter, object[][] stat_changes)
            : base(story,protag,stat_changes)
        {
            this.enemies = enemies;
            NextChapter = next_chapter;
            paused = true;

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
