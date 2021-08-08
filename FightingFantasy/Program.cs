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
                string input = Console.ReadLine();
                Console.WriteLine();
                Game.Continue(input);
            }
            
        }
    }
}
