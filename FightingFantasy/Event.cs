using System;
using System.Collections.Generic;
using System.Text;

namespace FightingFantasy
{
    class Event
    {
        public string Story { get; set; }
        public virtual List<string> Choices { get; set; }
        public List<string> Messages { get; set; }
        public bool Ended { get; set; }
        public int NextChapter { get; set; }
        protected Protagonist protag;

        public Event(string story, Protagonist protag)
        {
            Story = story;
            Messages = new List<string>();
            Ended = false;
            this.protag = protag;
        }

        public virtual void Start() { }

        public virtual void Continue(int input) { }
    }

    class StoryEvent : Event
    {
        private List<(string, int)> stat_changes;
        private List<(string, int)> items;

        public StoryEvent(string story, Protagonist protag, int next_chapter, List<(string, int)> stat_changes = null, List<(string, int)> items = null)
            : base(story, protag)
        {
            NextChapter = next_chapter;
            this.stat_changes = stat_changes ?? new List<(string,int)>();
            this.items = items ?? new List<(string,int)>();
            Choices = new List<string>();
        }

        public override void Start()
        {
            foreach ((string, int) stat_change in stat_changes)
            {
                string stat_name = stat_change.Item1;
                int change = stat_change.Item2;
                if (stat_name == "stamina")
                {
                    protag.stamina += change;
                    if (change < 0)
                        Messages.Add($"You took {-change} damage!");
                    else
                        Messages.Add($"You recovered {change} stamina!");
                }
                else if (stat_name == "skill")
                {
                    protag.skill += change;
                    if (change < 0)
                        Messages.Add($"Your skill decreased by {-change}!");
                    else
                        Messages.Add($"Your skill increased by {change}!");
                }
                else if (stat_name == "luck")
                {
                    protag.luck += change;
                    if (change < 0)
                        Messages.Add($"Your luck decreased by {-change}!");
                    else
                        Messages.Add($"Your luck increased by {change}!");
                }
            }

            foreach ((string, int) item in items)
            {
                protag.AddToInventory(Item.CreateItem(item.Item1), item.Item2);
                Messages.Add($"You obtained {item.Item2} {item.Item1}!");
            }
        }

        public override void Continue(int input)
        {
            Ended = true;
        }
    }

    class ChoiceEvent : Event
    {
        private List<int> next_chapters;
        public ChoiceEvent(string story, Protagonist protag, List<(string, int)> choices)
            : base(story, protag)
        {
            Choices = new List<string>();
            next_chapters = new List<int>();

            foreach ((string, int) choice in choices)
            {
                Choices.Add(choice.Item1);
                next_chapters.Add(choice.Item2);
            }
        }

        public override void Continue(int input)
        {
            if (input <= next_chapters.Count && input >= 1)
            {
                NextChapter = next_chapters[input - 1];
                Ended = true;
            }
            else
            {
                Messages.Add("Invalid choice. Please enter a number corresponding to one of the choices above.");
            }
        }
    }

    class BattleEvent : Event
    {
        List<Enemy> enemies;
        private Battle battle;
        public override List<string> Choices
        {
            get => battle.GetChoices();
        }

        public BattleEvent(string story, Protagonist protag, List<Enemy> enemies, int next_chapter)
            : base(story, protag)
        {
            this.enemies = enemies;
            NextChapter = next_chapter;
        }

        public override void Start()
        {
            battle = new Battle(protag, enemies[0]);
            Messages.Add("Battle!");
        }

        public override void Continue(int input)
        {
            if (battle.BattleEnded)
            {
                ReadyNextEnemy();
                return;
            }

            if (input > 2 || input < 1)
            {
                Messages.Add("Invalid choice. Please enter a number corresponding to one of the choices above.");
                return;
            }

            battle.RunNextRound(input);
            Messages.Add(battle.Message);
        }

        private void ReadyNextEnemy()
        {
            enemies.RemoveAt(0);
            if (enemies.Count == 0)
                Ended = true;
            else
                battle = new Battle(protag, enemies[0]);
        }

        public (string, int, int) GetEnemyStats()
        {
            Enemy enemy = enemies[0];
            return (enemy.name, enemy.stamina, enemy.skill);
        }
    }

    class ItemEvent235 : Event
    {
        public ItemEvent235(string story, Protagonist protag)
            : base(story,protag)
        {
            string item1 = "gold pieces";
            string item2 = "copper-colored key";
            string item3 = "ointment";

            Choices = new List<string>()
            {
                $"{item1} and {item2}",
                $"{item2} and {item3}",
                $"{item1} and {item3}"
            };
        }

        public override void Continue(int input)
        {
            if (input <= Choices.Count && input >= 1)
            {
                Item gold = Item.CreateItem("gold pieces");
                Item copper_key = Item.CreateItem("copper-colored key");
                Item ointment = Item.CreateItem("ointment");

                switch (input)
                {
                    case 1:
                        protag.AddToInventory(gold, 8);
                        protag.AddToInventory(copper_key, 1);
                        break;
                    case 2:
                        protag.AddToInventory(copper_key, 1);
                        protag.AddToInventory(ointment, 1);
                        break;
                    case 3:
                        protag.AddToInventory(gold, 8);
                        protag.AddToInventory(ointment, 1);
                        break;
                }
                Ended = true;
            }
            else
            {
                Messages.Add("Invalid choice. Please enter a number corresponding to one of the choices above.");
            }
        }
    }
}
