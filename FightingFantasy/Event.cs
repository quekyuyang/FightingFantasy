﻿using System;
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

        public virtual void Continue(string input) { }
    }

    class StoryEvent : Event
    {
        private List<(string, int)> stat_changes;
        
        public StoryEvent(string story, Protagonist protag, int next_chapter, List<(string, int)> stat_changes)
            : base(story, protag)
        {
            NextChapter = next_chapter;
            this.stat_changes = stat_changes;
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
        }

        public override void Continue(string input)
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

        public override void Continue(string input)
        {
            int player_choice;
            if (Int32.TryParse(input, out player_choice) && player_choice <= next_chapters.Count && player_choice >= 1)
            {
                NextChapter = next_chapters[player_choice - 1];
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
        }

        public override void Continue(string input)
        {
            if (battle.BattleEnded)
            {
                ReadyNextEnemy();
                return;
            }

            int player_choice;
            if (!Int32.TryParse(input, out player_choice) || player_choice > 2 || player_choice < 1)
            {
                Messages.Add("Invalid choice. Please enter a number corresponding to one of the choices above.");
                return;
            }

            battle.RunNextRound(player_choice);
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
}