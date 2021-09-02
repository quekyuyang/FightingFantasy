using System;
using System.Collections.Generic;
using System.Text;

namespace FightingFantasy
{
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
                if (protag.AttackStrength() > enemy.AttackStrength())
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
                return new List<string> { "Yes", "No" };
            else
                return new List<string>();
        }

        private bool EnemyDead()
        {
            return enemy.stamina <= 0;
        }
    }
}
