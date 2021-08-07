using System;
using System.Collections.Generic;
using System.Text;

namespace FightingFantasy
{
    class Character
    {
        public int stamina;
        public int skill;

        public int AttackStrength()
        {
            return Dice.SumRoll(2) + skill;
        }

        public virtual int DealDmg()
        {
            return 2;
        }

        public virtual void TakeDmg(int dmg)
        {
            stamina -= dmg;
        }
    }

    class Protagonist:Character
    {
        public int luck;

        public Protagonist()
        {
            stamina = Dice.SumRoll(2) + 12;
            skill = Dice.SumRoll(1) + 6;
            luck = Dice.SumRoll(1) + 6;
        }

        public bool TestLuck()
        {
            bool is_lucky = Dice.SumRoll(2) <= luck;
            luck -= 1;
            return is_lucky;
        }

        public override int DealDmg()
        {
            char user_input = PromptYesNo("You wounded the enemy! Test your luck to increase damage? (y/n)");

            int dmg = 0;
            if (user_input == 'y')
            {
                bool is_lucky = TestLuck();
                if (is_lucky)
                    dmg = 4;
                else
                    dmg = 1;
            }
            else if (user_input == 'n')
                dmg = 2;

            return dmg;
        }

        public override void TakeDmg(int dmg)
        {
            char user_input = PromptYesNo("The enemy wounded you! Test your luck to decrease damage? (y/n)");

            if (user_input == 'y')
            {
                bool is_lucky = TestLuck();
                if (is_lucky)
                    stamina -= dmg - 1;
                else
                    stamina -= dmg + 1;
            }
            else if (user_input == 'n')
                stamina -= dmg;
        }

        static char PromptYesNo(string msg)
        {
            Console.WriteLine(msg);
            string user_input = Console.ReadLine();
            while (user_input != "y" && user_input != "n")
            {
                Console.WriteLine("Invalid command, please enter 'y' or 'n'.");
                user_input = Console.ReadLine();
            }
            return user_input[0];
        }
    }

    class Enemy : Character
    {
        public string name;
        public Enemy(string name, int stamina, int skill)
        {
            this.name = name;
            this.stamina = stamina;
            this.skill = skill;
        }
    }
}
