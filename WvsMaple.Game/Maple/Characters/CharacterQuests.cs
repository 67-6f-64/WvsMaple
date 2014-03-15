using Common;
using Common.Net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Data;
using WvsGame.Net;

namespace WvsGame.Maple.Characters
{
    public class CharacterQuests
    {
        public Character Parent { get; private set; }

        public Dictionary<ushort, Dictionary<int, short>> Started { get; private set; }
        public Dictionary<ushort, DateTime> Completed { get; private set; }

        public CharacterQuests(Character parent)
        {
            this.Parent = parent;

            this.Started = new Dictionary<ushort, Dictionary<int, short>>();
            this.Completed = new Dictionary<ushort, DateTime>();
        }

        public void Operate(Packet inPacket)
        {
            QuestAction action = (QuestAction)inPacket.ReadByte();
            Quest quest = MapleData.CachedQuests[inPacket.ReadUShort()];

            int npcId = -1;

            switch (action)
            {
                case QuestAction.Start:

                    npcId = inPacket.ReadInt();
                    this.Start(quest, npcId);

                    break;

                case QuestAction.Complete:

                    npcId = inPacket.ReadInt();
                    inPacket.ReadInt();
                    int itemChoiceId = 0;

                    if (inPacket.Remaining >= 4)
                    {
                        itemChoiceId = inPacket.ReadInt();
                    }

                    this.Finish(quest, itemChoiceId);

                    break;
            }
        }

        public void Append(Packet outPacket)
        {
            outPacket.WriteShort((short)this.Started.Count);

            foreach (KeyValuePair<ushort, Dictionary<int, short>> quest in this.Started)
            {
                outPacket.WriteUShort(quest.Key);

                string kills = string.Empty;

                foreach (int kill in quest.Value.Values)
                {
                    kills += kill.ToString().PadLeft(3, '\u0030');
                }

                outPacket.WriteString(kills);
            }

            outPacket.WriteShort((short)this.Completed.Count);

            foreach (KeyValuePair<ushort, DateTime> quest in this.Completed)
            {
                outPacket.WriteUShort(quest.Key);
                outPacket.WriteDateTime(quest.Value);
            }
        }

        public void Start(Quest quest, int npcId)
        {
            this.Started.Add(quest.ID, new Dictionary<int, short>());

            foreach (KeyValuePair<int, short> requiredKills in MapleData.CachedQuests[quest.ID].PostRequiredKills)
            {
                this.Started[quest.ID].Add(requiredKills.Key, 0);
            }

            // TODO: Add checks for this rewards?
            //this.Parent.GainExperience(quest.ExperienceReward[0], true);
            //this.Parent.Fame += (short)quest.FameReward[0];
            //this.Parent.Meso += quest.MesoReward[0] * Constants.MesoRate;

            // TODO: Skill rewards and pet related rewards.

            foreach (KeyValuePair<int, short> item in quest.PreItemRewards)
            {
                if (item.Value > 0)
                {
                    this.Parent.Inventory.Add(new Item(item.Key, item.Value), inChat: true);
                }
                else if (item.Value < 0)
                {
                    this.Parent.Inventory.Remove(item.Key, Math.Abs(item.Value));
                }
            }

            using (Packet outPacket = new Packet(ServerMessages.Message))
            {
                outPacket.WriteByte(1);
                outPacket.WriteUShort(quest.ID);
                outPacket.WriteByte(1);
                outPacket.WriteString("");

                this.Parent.Client.Send(outPacket);
            }

            this.Update(quest.ID, npcId, 10);
        }

        public void Finish(Quest quest, int itemChoiseId)
        {

            foreach (KeyValuePair<int, short> item in quest.PostRequiredItems)
            {
                this.Parent.Inventory.Remove(item.Key, item.Value);
            }

            //TODO: Add checks for this rewards?
            //this.Parent.GainExperience(quest.ExperienceReward[1], true);
            //this.Parent.Fame += (short)quest.FameReward[1];
            //this.Parent.Meso += quest.MesoReward[1] * Constants.MesoRate;

            // TODO: Skill rewards and pet related rewards

            foreach (KeyValuePair<int, short> item in quest.PostItemRewards)
            {
                if (item.Value > 0)
                {
                    this.Parent.Inventory.Add(new Item(item.Key, item.Value), inChat: true);
                }
                else if (item.Value < 0)
                {
                    this.Parent.Inventory.Remove(item.Key, Math.Abs(item.Value));
                }
            }

            if (itemChoiseId != 0)
            {
                //this.Parent.Items.Add(new Item(itemChoiceId, quest.SelectibleItemRewards[itemChoiceId]));
            }

            using (Packet outPacket = new Packet(ServerMessages.Message))
            {
                outPacket.WriteByte(1);
                outPacket.WriteUShort(quest.ID);
                outPacket.WriteByte(2);
                outPacket.WriteDateTime(DateTime.UtcNow);

                this.Parent.Client.Send(outPacket);
            }

            this.Delete(quest.ID);

            this.Completed.Add(quest.ID, DateTime.UtcNow);

            //using (Packet outPacket = new Packet(MapleServerOperationCode.ShowItemGainInChat))
            //{
            //    outPacket.WriteByte(12);

            //    this.Parent.Client.Send(outPacket);
            //}

            using (Packet outPacket = new Packet(ServerMessages.QuestClear))
            {
                this.Parent.Client.Send(outPacket);
            }
        }

        public void Update(ushort questId, int npcId, byte progress)
        {
            //using (Packet outPacket = new Packet(ServerMessages.UpdateQuestInfo))
            //{
            //    outPacket.WriteByte(progress);
            //    outPacket.WriteUShort(questId);
            //    outPacket.WriteInt(npcId);
            //    outPacket.WriteInt();

            //    this.Parent.Client.Send(outPacket);
            //}
        }

        public void Load()
        {
            using (MySqlDataReader reader = GameServer.Database.RunQuery("SELECT * FROM quests_started WHERE CharacterID = '{0}'", this.Parent.ID))
            {
                while (reader.Read())
                {
                    ushort questId = reader.GetUInt16("QuestID");

                    if (!this.Started.ContainsKey(questId))
                    {
                        this.Started.Add(questId, new Dictionary<int, short>());
                    }
                }
            }

            using (MySqlDataReader reader = GameServer.Database.RunQuery("SELECT * FROM quests_completed WHERE CharacterID = '{0}'", this.Parent.ID))
            {
                while (reader.Read())
                {
                    this.Completed.Add(reader.GetUInt16("QuestID"), reader.GetDateTime("CompletionTime"));
                }
            }
        }

        public void Save()
        {
            foreach (KeyValuePair<ushort, Dictionary<int, short>> quest in this.Started)
            {
                if (quest.Value == null || quest.Value.Count == 0)
                {
                    if (GameServer.Database.Exists("quests_started", "CharacterID = '{0}' && QuestID = '{1}'", this.Parent.ID, quest.Key))
                    {
                        return;
                    }

                    StringBuilder query = new StringBuilder();

                    query.Append("INSERT INTO quests_started (CharacterID, QuestID) VALUES (");
                    query.AppendFormat("'{0}', ", this.Parent.ID);
                    query.AppendFormat("'{0}')", quest.Key);

                    GameServer.Database.RunQuery(query.ToString());
                }
                else
                {
                    foreach (KeyValuePair<int, short> mobKills in quest.Value)
                    {
                        StringBuilder query = new StringBuilder();

                        if (GameServer.Database.Exists("quests_started", "CharacterID = '{0}' && QuestID = '{1}' && MobID = '{2}'", this.Parent.ID, quest.Key, mobKills.Key))
                        {
                            query.Append("UPDATE quests_started SET ");
                            query.AppendFormat("{0} = '{1}', ", "CharacterID", this.Parent.ID);
                            query.AppendFormat("{0} = '{1}', ", "QuestID", quest.Key);
                            query.AppendFormat("{0} = '{1}', ", "MobID", mobKills.Key);
                            query.AppendFormat("{0} = '{1}' ", "Killed", mobKills.Value);
                            query.AppendFormat("WHERE CharacterID = '{0}' && QuestID = '{1}' && MobID = '{2}'", this.Parent.ID, quest.Key, mobKills.Key);
                        }
                        else
                        {
                            query.Append("INSERT INTO quests_started VALUES (");
                            query.AppendFormat("'{0}', ", this.Parent.ID);
                            query.AppendFormat("'{0}', ", quest.Key);
                            query.AppendFormat("'{0}', ", mobKills.Key);
                            query.AppendFormat("'{0}')", mobKills.Value);
                        }

                        GameServer.Database.RunQuery(query.ToString());
                    }
                }
            }

            foreach (KeyValuePair<ushort, DateTime> quest in this.Completed)
            {
                StringBuilder query = new StringBuilder();

                if (GameServer.Database.Exists("quests_completed", "CharacterID = '{0}' && QuestID = '{1}'", this.Parent.ID, quest.Key))
                {
                    query.Append("UPDATE quests_completed SET ");
                    query.AppendFormat("{0} = '{1}', ", "CharacterID", this.Parent.ID);
                    query.AppendFormat("{0} = '{1}', ", "QuestID", quest.Key);
                    query.AppendFormat("{0} = '{1}' ", "CompletionTime", quest.Value);
                    query.AppendFormat("WHERE CharacterID = '{0}' && QuestID = '{1}'", this.Parent.ID, quest.Key);
                }
                else
                {
                    query.Append("INSERT INTO quests_completed VALUES (");
                    query.AppendFormat("'{0}', ", this.Parent.ID);
                    query.AppendFormat("'{0}', ", quest.Key);
                    query.AppendFormat("'{0}', ", quest.Value);
                }

                GameServer.Database.RunQuery(query.ToString());
            }
        }

        public void Delete(ushort questId)
        {
            if (this.Started.ContainsKey(questId))
            {
                this.Started.Remove(questId);
            }

            if (GameServer.Database.Exists("quests_started", "QuestID = '{0}'", questId))
            {
                GameServer.Database.Delete("quests_started", "QuestID = '{0}'", questId);
            }
        }
    }
}
