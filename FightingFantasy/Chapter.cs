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
        public List<string> Messages { get; set; }
        public bool IsActive { get; set; }
        public int NextChapter { get; set; }
        public Protagonist protag;

        public Chapter(string story, Protagonist protag, object[][] stat_changes)
        {
            Story = story;
            IsActive = true;
            this.protag = protag;
            Messages = new List<string>();

            if (stat_changes != null)
            {
                foreach (object[] stat_change in stat_changes)
                {
                    string stat_name = (string)stat_change[0];
                    int change = (int)(long)stat_change[1];
                    if (stat_name == "stamina")
                    {
                        protag.stamina += change;
                        if (change < 0)
                            Messages.Add($"You took {-change} damage!");
                        else
                            Messages.Add($"You recovered {change} stamina!");
                    }
                    else if (stat_name == "skill")
                    {
                        protag.skill += change;
                        if (change < 0)
                            Messages.Add($"Your skill decreased by {-change}!");
                        else
                            Messages.Add($"Your skill increased by {change}!");
                    }
                    else if (stat_name == "luck")
                    {
                        protag.luck += change;
                        if (change < 0)
                            Messages.Add($"Your luck decreased by {-change}!");
                        else
                            Messages.Add($"Your luck increased by {change}!");
                    }
                        
                }
            }
        }

        public virtual void Continue(string input){}

        public virtual List<string> GetChoices()
        {
            return new List<string>();
        }

        public List<string> GetMessages()
        {
            return Messages;
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
        public List<string> Choices { get; set; }
        private List<int> next_chapters;

        public ChoiceChapter(string story, Protagonist protag, List<(string,int)> choices, object[][] stat_changes)
            : base(story,protag,stat_changes)
        {
            Choices = new List<string>();
            next_chapters = new List<int>();

            foreach ((string,int) choice in choices)
            {
                Choices.Add(choice.Item1);
                next_chapters.Add(choice.Item2);
            }
        }

        public override void Continue(string input)
        {
            int player_choice;
            if (Int32.TryParse(input, out player_choice) && player_choice <= next_chapters.Count && player_choice >= 1)
            {
                NextChapter = next_chapters[player_choice - 1];
                IsActive = false;
            }
            else
            {
                Messages.Add("Invalid choice. Please enter a number corresponding to one of the choices above.");
            }
        }

        public override List<string> GetChoices()
        {
            return Choices;
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

        public override List<string> GetChoices()
        {
            return battle.GetChoices();
        }

        public override void Continue(string input)
        {
            if (battle.BattleEnded)
            {
                ReadyNextEnemy();
                return;
            }
                
            int player_choice;
            if (!Int32.TryParse(input, out player_choice) || player_choice > 2 || player_choice < 1)
            {
                Messages.Add("Invalid choice. Please enter a number corresponding to one of the choices above.");
                return;
            }

            battle.RunNextRound(player_choice);
            Messages.Add(battle.Message);
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
