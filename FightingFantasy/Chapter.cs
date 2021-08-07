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
        public Enemy[] enemies;
    }

    class Chapter
    {
        public string story;

        public virtual int Play(){ return -1; }
    }

    class StoryOnlyChapter : Chapter
    {
        public int next_chapter;

        public StoryOnlyChapter(string story, int next_chapter)
        {
            this.story = story;
            this.next_chapter = next_chapter;
        }

        public override int Play()
        {
            Console.WriteLine(story);
            Console.ReadLine();
            return next_chapter;
        }
    }

    class ChoiceChapter : Chapter
    {
        public object[][] choices { get; set; }
        
        public ChoiceChapter(string story, object[][] choices)
        {
            this.story = story;
            this.choices = choices;
        }

        public override int Play()
        {
            Console.WriteLine(story);
            Console.WriteLine();
            for (int i = 0; i < choices.Length; i++)
                Console.WriteLine("{0}. {1}", i + 1, (string)choices[i][0]);

            int user_choice;
            while (!Int32.TryParse(Console.ReadLine(), out user_choice) && user_choice > choices.Length && user_choice < 1) 
                Console.WriteLine("Invalid input, please enter again.");

            long chapter_n = (long)choices[user_choice - 1][1];
            return (int)chapter_n;
        }
    }

    class BattleChapter : Chapter
    {
        public Enemy[] enemies;
        public Protagonist protag;
        public int next_chapter;

        public BattleChapter(string story, Protagonist protag, Enemy[] enemies, int next_chapter)
        {
            this.story = story;
            this.enemies = enemies;
            this.protag = protag;
            this.next_chapter = next_chapter;
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
