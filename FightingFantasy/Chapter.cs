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
        public Enemy current_enemy;
        public string State { get; set; }
        public Protagonist protag;
        public object[][] choices { get; set; }
        private bool paused;

        public BattleChapter(string story, Protagonist protag, List<Enemy> enemies, int next_chapter)
        {
            Story = story;
            Message = "";
            this.current_enemy = enemies[0];
            this.enemies = enemies;
            this.protag = protag;
            NextChapter = next_chapter;
            paused = true;
            IsActive = true;
        }

        public override string[] GetChoices()
        {
            if (!paused)
                return new string[] { "Yes", "No" };
            else
                return new string[0];
        }

        public override void Continue(string input)
        {
            Message = "";
            if (!paused)
            {
                int player_choice;
                if (!Int32.TryParse(input, out player_choice) || player_choice > 2 || player_choice < 1)
                {
                    Message = "Invalid choice. Please enter a number corresponding to one of the choices above.";
                    return;
                }

                if (player_choice == 1)
                {
                    bool is_lucky = protag.TestLuck();
                    if (State == "wounding")
                    {
                        if (is_lucky)
                        {
                            current_enemy.stamina -= 2;
                            Message = "You dealt a critical blow!"; // These messages are never shown because overwritten by RunNextRound
                        }
                        else
                        {
                            current_enemy.stamina += 1;
                            Message = "The wound was a mere graze..";
                        }
                            
                    }
                    else if (State == "wounded")
                    {
                        if (is_lucky)
                        {
                            protag.stamina += 1;
                            Message = "You managed to avoid full damage!";
                        }
                        else
                        {
                            protag.stamina -= 1;
                            Message = "You suffered a critical blow!";
                        }
                            
                    }
                }

                if (EnemyDead())
                {
                    Message = $"You defeated {current_enemy.name}!";
                    ReadyNextEnemy();
                    paused = true;
                }
            }
            else
                paused = false;

            if (!paused)
                RunNextRound();
        }

        private void RunNextRound()
        {
            if (protag.AttackStrength() > current_enemy.AttackStrength())
            {
                current_enemy.stamina -= 2;
                State = "wounding";
                Message = "You have wounded the enemy! Test your luck?";
            }
            else
            {
                protag.stamina -= 2;
                State = "wounded";
                Message = "You have been wounded! Test your luck?";
            }

            if (EnemyDead())
            {
                Message = $"You defeated {current_enemy.name}!";
                ReadyNextEnemy();
                paused = true;
            }
        }

        private bool EnemyDead()
        {
            return current_enemy.stamina <= 0;
        }

        private void ReadyNextEnemy()
        {
            enemies.RemoveAt(0);
            if (enemies.Count == 0)
                IsActive = false;
            else
                current_enemy = enemies[0];
        }

        public (string,int,int) GetEnemyStats()
        {
            return (current_enemy.name,current_enemy.stamina, current_enemy.skill);
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
