using System;
using System.Collections.Generic;
using System.Text;

namespace FightingFantasy
{
    class JsonChapter
    {
        public string type;
        public string story;
        public string story2;
        public int next_chapter;
        public object[][] choices;
        public List<Enemy> enemies;
        public object[][] stat_changes;
        public object[][] items;
    }

    class ChapterData
    {
        public string type;
        public string story;
        public string story2;
        public int next_chapter;
        public List<(string, int)> choices;
        public List<Enemy> enemies;
        public List<(string, int)> stat_changes;
        public List<(string, int)> items;

        public ChapterData(JsonChapter json_chapter)
        {
            type = json_chapter.type;
            story = json_chapter.story;
            story2 = json_chapter.story2;
            next_chapter = json_chapter.next_chapter;
            enemies = json_chapter.enemies;

            if (json_chapter.choices != null)
            {
                int n_choices = json_chapter.choices.Length;
                choices = new List<(string, int)>();
                for (int i = 0; i < n_choices; i++)
                    choices.Add(((string)json_chapter.choices[i][0], (int)(long)json_chapter.choices[i][1]));
            }

            stat_changes = new List<(string, int)>();
            if (json_chapter.stat_changes != null)
            {
                int n_changes = json_chapter.stat_changes.Length;
                for (int i = 0; i < n_changes; i++)
                    stat_changes.Add(((string)json_chapter.stat_changes[i][0], (int)(long)json_chapter.stat_changes[i][1]));
            }

            items = new List<(string, int)>();
            if (json_chapter.items != null)
            {
                int n_changes = json_chapter.items.Length;
                for (int i = 0; i < n_changes; i++)
                    items.Add(((string)json_chapter.items[i][0], (int)(long)json_chapter.items[i][1]));
            }
        }
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
            var chapter_data = new ChapterData(chapters[chapter_n]);

            if (chapter_data.type == "choices")
                return new ChoiceChapter(chapter_data.story, protag, chapter_data.choices);
            else if (chapter_data.type == "story_only")
                return new StoryOnlyChapter(chapter_data.story, protag, chapter_data.next_chapter, chapter_data.stat_changes, chapter_data.items);
            else if (chapter_data.type == "battle")
                return new BattleChapter(chapter_data.story, protag, chapter_data.enemies, chapter_data.next_chapter);
            else
                return CreateSpecialChapter(chapter_n, protag);
        }

        private Chapter CreateSpecialChapter(int chapter_n, Protagonist protag)
        {
            var chapter_data = new ChapterData(chapters[chapter_n]);
            switch (chapter_n)
            {
                case 71:
                    return new Chapter71(chapter_data.story, protag, chapter_data.enemies, chapter_data.next_chapter);
                case 220:
                    return new Chapter220(chapter_data, protag);
                case 235:
                    return new Chapter235(chapter_data, protag);
                case 383:
                    return new Chapter383(chapter_data, protag);
                default:
                    throw new Exception($"Attempt to create unimplemented chapter {chapter_n}");
            }
        }
    }

    class Chapter
    {
        public List<string> Messages { get; set; }
        public bool Ended { get; set; }
        protected Queue<Event> events;
        protected Event current_event;
        public Event CurrentEvent { get => current_event; }

        public int NextChapter
        {
            get => current_event.NextChapter;
        }
        public bool InBattle
        {
            get => current_event is BattleEvent;
        }

        public Chapter()
        {
            events = new Queue<Event>();
        }

        public virtual void Continue(int input)
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
        public StoryOnlyChapter(string story, Protagonist protag, int next_chapter, List<(string, int)> stat_changes, List<(string, int)> items)
            : base()
        {
            current_event = new StoryEvent(story, protag, next_chapter, stat_changes, items);
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
        public BattleChapter() { }

        public BattleChapter(string story, Protagonist protag, List<Enemy> enemies, int next_chapter)
            : base()
        {
            events.Enqueue(new StoryEvent(story, protag));
            events.Enqueue(new BattleEvent(story, protag, enemies, next_chapter));

            current_event = events.Dequeue();
            current_event.Start();
        }
    }

    class Chapter71: BattleChapter
    {
        public Chapter71(string story, Protagonist protag, List<Enemy> enemies, int next_chapter)
        {
            events.Enqueue(new StoryEvent(story, protag));
            events.Enqueue(new BattleEvent71(story, protag, enemies, next_chapter));

            current_event = events.Dequeue();
            current_event.Start();
        }
    }

    class Chapter220: Chapter
    {
        public Chapter220(ChapterData chapter_data, Protagonist protag)
        {
            events.Enqueue(new StoryEvent(chapter_data.story, protag));
            events.Enqueue(new BattleEvent(chapter_data.story, protag, chapter_data.enemies));
            events.Enqueue(new ChoiceEvent(chapter_data.story2, protag, chapter_data.choices));

            current_event = events.Dequeue();
            current_event.Start();
        }
    }

    class Chapter235 : Chapter
    {
        public Chapter235(ChapterData chapter_data, Protagonist protag)
        {
            events.Enqueue(new ItemEvent235(chapter_data.story, protag));
            events.Enqueue(new StoryEvent(chapter_data.story2, protag, 0, chapter_data.stat_changes, chapter_data.items));
            events.Enqueue(new ChoiceEvent("What will you do next?", protag, chapter_data.choices));

            current_event = events.Dequeue();
            current_event.Start();
        }
    }

    class Chapter383 : Chapter
    {
        public Chapter383(ChapterData chapter_data, Protagonist protag)
        {
            List<(string, int)> stat_changes = chapter_data.stat_changes;
            events.Enqueue(new StoryEvent(chapter_data.story, protag, 0, stat_changes, chapter_data.items));
            events.Enqueue(new ChoiceEvent("What will you do?", protag, chapter_data.choices));

            current_event = events.Dequeue();
            current_event.Start();
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
