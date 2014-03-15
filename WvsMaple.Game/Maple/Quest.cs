using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Data;

namespace WvsGame.Maple
{
    public class Quest
    {
        public ushort ID { get; set; }
        public ushort NextQuestID { get; set; }
        public sbyte Area { get; set; }
        public byte MinimumLevel { get; set; }
        public byte MaximumLevel { get; set; }
        public short PetCloseness { get; set; }
        public sbyte TamingMobLevel { get; set; }
        public int RepeatWait { get; set; }
        public short Fame { get; set; }
        public int TimeLimit { get; set; }
        public bool AutoStart { get; set; }

        public Quest NextQuest
        {
            get
            {
                return MapleData.CachedQuests[this.NextQuestID];
            }
        }

        public List<ushort> PreRequiredQuests { get; private set; }
        public List<ushort> PostRequiredQuests { get; private set; }
        public Dictionary<int, short> PreRequiredItems { get; private set; }
        public Dictionary<int, short> PostRequiredItems { get; private set; }
        public Dictionary<int, short> PostRequiredKills { get; private set; }
        public List<short> ValidJobs { get; private set; }

        // Rewards (Start, End)
        public int[] ExperienceReward { get; set; }
        public int[] MesoReward { get; set; }
        public int[] PetClosenessReward { get; set; }
        public bool[] PetSpeedReward { get; set; }
        public int[] FameReward { get; set; }
        public int[] PetSkillReward { get; set; }
        public Dictionary<int, short> PreItemRewards { get; private set; }
        public Dictionary<int, short> PostItemRewards { get; private set; }
        //public Dictionary<Skill, Job> PreSkillRewards { get; set; }
        //public Dictionary<Skill, Job> PostSkillRewards { get; set; }


        public Quest()
        {
            this.PreRequiredQuests = new List<ushort>();
            this.PostRequiredQuests = new List<ushort>();
            this.PreRequiredItems = new Dictionary<int, short>();
            this.PostRequiredItems = new Dictionary<int, short>();
            this.PostRequiredKills = new Dictionary<int, short>();

            this.ExperienceReward = new int[2];
            this.MesoReward = new int[2];
            this.PetClosenessReward = new int[2];
            this.PetSpeedReward = new bool[2];
            this.FameReward = new int[2];
            this.PetSkillReward = new int[2];

            this.PreItemRewards = new Dictionary<int, short>();
            this.PostItemRewards = new Dictionary<int, short>();
            //this.PreSkillRewards = new Dictionary<Skill, Job>();
            //this.PostSkillRewards = new Dictionary<Skill, Job>();

            this.ValidJobs = new List<short>();
        }
    }
}
