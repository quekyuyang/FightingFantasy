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

    class ChapterFactory
    {
        private Dictionary<int,JsonChapter> chapters;
        public ChapterFactory(Dictionary<int, JsonChapter> chapters)
        {
            this.chapters = chapters;
        }

        public Chapter CreateChapter(int chapter_n, Protagonist protag)
        {
            JsonChapter chapter_data = chapters[chapter_n];

            var stat_changes = new List<(string, int)>();
            if (chapter_data.stat_changes != null)
            {
                int n_changes = chapter_data.stat_changes.Length;
                for (int i = 0; i < n_changes; i++)
                    stat_changes.Add(((string)chapter_data.stat_changes[i][0], (int)(long)chapter_data.stat_changes[i][1]));
            }

            if (chapter_data.type == "choices")
            {
                int n_choices = chapter_data.choices.Length;
                var choices = new List<(string, int)>();
                for (int i = 0; i < n_choices; i++)
                    choices.Add(((string)chapter_data.choices[i][0], (int)(long)chapter_data.choices[i][1]));
                return new ChoiceChapter(chapter_data.story, protag, choices);
            }
            else if (chapter_data.type == "story_only")
                return new StoryOnlyChapter(chapter_data.story, protag, chapter_data.next_chapter, stat_changes);
            else
                return new BattleChapter(chapter_data.story, protag, chapter_data.enemies, chapter_data.next_chapter);
        }
    }

    class Chapter
    {
        public List<string> Messages { get; set; }
        public bool Ended { get; set; }
        protected Queue<Event> events;
        protected Event current_event;
        public int NextChapter
        {
            get => current_event.NextChapter;
        }

        public Chapter()
        {
            events = new Queue<Event>();
        }

        public virtual void Continue(string input)
        {
            current_event.Continue(input);
            if (current_event.Ended)
            {
                if (events.Count > 0)
                {
                    current_event = events.Dequeue();
                    current_event.Start();
                }
                else
                    Ended = true;
            }
        }

        public string GetStory()
        {
            return current_event.Story;
        }

        public List<string> GetChoices()
        {
            return current_event.Choices;
        }

        public List<string> GetMessages()
        {
            return current_event.Messages;
        }
    }

    class StoryOnlyChapter : Chapter
    {
        public StoryOnlyChapter(string story, Protagonist protag, int next_chapter, List<(string, int)> stat_changes)
            : base()
        {
            current_event = new StoryEvent(story, protag, next_chapter, stat_changes);
            current_event.Start();
        }
    }

    class ChoiceChapter : Chapter
    {
        public ChoiceChapter(string story, Protagonist protag, List<(string,int)> choices)
            : base()
        {
            current_event = new ChoiceEvent(story, protag, choices);
            current_event.Start();
        }
    }

    class BattleChapter : Chapter
    {        
        public BattleChapter(string story, Protagonist protag, List<Enemy> enemies, int next_chapter)
            : base()
        {
            current_event = new BattleEvent(story, protag, enemies, next_chapter);
            current_event.Start();
        }

        public (string,int,int) GetEnemyStats()
        {
            return (current_event as BattleEvent).GetEnemyStats();
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
