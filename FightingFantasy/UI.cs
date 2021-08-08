using System;
using System.Collections.Generic;
using System.Text;

namespace FightingFantasy
{
    class UI
    {
        static public void Update()
        {
            Console.Clear();
            DisplayCharacterStats();

            Console.WriteLine(Game.GetStory());
            Console.WriteLine();

            DisplayEnemyStats();

            DisplayChoices();
            Console.WriteLine(Game.GetMessage());
            PromptResponse();
        }

        static private void DisplayChoices()
        {
            string[] choices = Game.GetChoices();
            int i = 1;
            foreach (string choice in choices)
            {
                Console.WriteLine("{0}. {1}", i, choice);
                i++;
            }
        }

        static private void DisplayCharacterStats()
        {
            var (stamina, skill, luck) = Game.GetProtagStats();
            Console.WriteLine("Your character stats:");
            Console.WriteLine("Stamina: {0}", stamina);
            Console.WriteLine("Skill: {0}", skill);
            Console.WriteLine("Luck: {0}", luck);
            Console.WriteLine();
        }

        static private void DisplayEnemyStats()
        {
            var (name, stamina, skill) = Game.GetEnemyStats();
            if (name.Length > 0)
            {
                Console.WriteLine(name);
                Console.WriteLine($"Stamina: {stamina}");
                Console.WriteLine($"Skill: {skill}");
                Console.WriteLine();
            }
        }

        static private void PromptResponse()
        {
            Console.Write("->");
        }
    }
}
