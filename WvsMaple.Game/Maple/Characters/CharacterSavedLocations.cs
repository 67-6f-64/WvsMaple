using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WvsGame.Net;

namespace WvsGame.Maple.Characters
{
    public class CharacterSavedLocations : Dictionary<string, int>
    {
        public Character Parent { get; private set; }

        public CharacterSavedLocations(Character parent)
            : base()
        {
            this.Parent = parent;
        }

        public void Insert(string label, int destinationId)
        {
            if (this.ContainsKey(label))
            {
                this[label] = destinationId;
            }
            else
            {
                this.Add(label, destinationId);
            }
        }

        public int Get(string label)
        {
            if (!this.ContainsKey(label))
            {
                return 0;
            }

            return this[label];
        }

        public void Delete(string label)
        {
            if (!this.ContainsKey(label))
            {
                return;
            }

            this.Remove(label);
            GameServer.Database.RunQuery("DELETE FROM saved_locations WHERE CharacterID = '{0}' && Label = '{1}'", this.Parent.ID, label);
        }

        public void Load()
        {
            using (MySqlDataReader reader = GameServer.Database.RunQuery("SELECT * FROM saved_locations WHERE CharacterID = '{0}'", this.Parent.ID))
            {
                while (reader.Read())
                {
                    this.Add(reader.GetString("Label"), reader.GetInt32("DestinationID"));
                }
            }
        }

        public void Save()
        {
            foreach (KeyValuePair<string, int> savedLocation in this)
            {
                StringBuilder query = new StringBuilder();

                if (GameServer.Database.Exists("saved_locations", "CharacterID = '{0}' && Label = '{1}'", this.Parent.ID, savedLocation.Key))
                {
                    query.Append("UPDATE saved_locations SET ");
                    query.AppendFormat("{0} = '{1}' ", "DestinationID", savedLocation.Value);
                    query.AppendFormat("WHERE CharacterID = '{0}' && Label = '{1}'", this.Parent.ID, savedLocation.Key);
                }
                else
                {
                    query.Append("INSERT INTO saved_locations (CharacterID, Label, DestionationID) VALUES (");
                    query.AppendFormat("'{0}', ", this.Parent.ID);
                    query.AppendFormat("'{0}', ", savedLocation.Key);
                    query.AppendFormat("'{0}')", savedLocation.Value);
                }

                GameServer.Database.RunQuery(query.ToString());
            }
        }
    }
}
