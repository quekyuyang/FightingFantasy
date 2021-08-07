using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FightingFantasy
{
    class Program
    {
        static void Main(string[] args)
        {
            string jstring = File.ReadAllText(@"c:\Users\queky\Downloads\story2.json");
            Dictionary<int, JsonChapter> chapters = JsonConvert.DeserializeObject<Dictionary<int, JsonChapter>>(jstring);
            Game.Start(chapters);
            
            while(true)
            {
                UI.Update();

                Type chapter_type = Game.GetChapterType();

                if (chapter_type.Equals(typeof(StoryOnlyChapter)))
                {
                    Console.ReadLine();
                    Game.Continue();
                }
                else if (chapter_type.Equals(typeof(ChoiceChapter)))
                {
                    int user_choice;
                    string[] choices = Game.GetChoices();
                    while (!Int32.TryParse(Console.ReadLine(), out user_choice) && user_choice > choices.Length && user_choice < 1)
                        Console.WriteLine("Invalid input, please enter again.");

                    Game.Choose(user_choice);
                }
            }
            
        }
    }
}
