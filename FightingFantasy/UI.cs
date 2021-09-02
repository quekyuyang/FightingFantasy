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

            if (Game.State == Game.StateEnum.Normal)
            {
                DisplayCharacterStats();

                Console.WriteLine(Game.GetStory());
                Console.WriteLine();

                DisplayChoices();
                Console.WriteLine();
                DisplayMessages();
                Console.WriteLine();
            }
            else if (Game.State == Game.StateEnum.Items)
            {
                Dictionary<Item, int> inventory = Game.GetProtagInventory();
                String s = "";
                foreach (KeyValuePair<Item, int> slot in inventory)
                    s += String.Format("{0,-30} {1,-5}\n", slot.Key, slot.Value);
                Console.WriteLine(s);
            }
            else if (Game.State == Game.StateEnum.Battle)
            {
                DisplayBattleState();
                Console.WriteLine();
                DisplayMessages();
                Console.WriteLine();
            }
            else if (Game.State == Game.StateEnum.GameOver)
                Console.WriteLine("Game Over!");
            PromptResponse();
        }

        static private void DisplayMessages()
        {
            List<string> messages = Game.GetMessages();
            foreach (string message in messages)
                Console.WriteLine(message);
            messages.Clear();
        }

        static private void DisplayChoices()
        {
            List<string> choices = Game.GetChoices();
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

        private static readonly string battle_format = "{0,8}{1,3}{2,6}{3,12}{4,3}";

        static private void DisplayBattleState()
        {
            var (protag_stamina, protag_skill, protag_luck) = Game.GetProtagStats();
            var (enemy_name, enemy_stamina, enemy_skill) = Game.GetEnemyStats();
            Console.WriteLine(battle_format, "You", "", "", enemy_name, "");
            Console.WriteLine(battle_format, "Stamina:", protag_stamina, "", "Stamina:", enemy_stamina);
            Console.WriteLine(battle_format, "Skill:", protag_skill, "vs", "Skill:", enemy_skill);
            Console.WriteLine(battle_format, "Luck:", protag_luck, "", "", "");
        }

        static private void PromptResponse()
        {
            Console.Write("->");
        }
    }
}
