using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsLogin.Net;

namespace WvsLogin.Maple
{
    public class Account
    {
        public MapleClient Client { get; private set; }

        public int ID { get; private set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Pin { get; set; }
        public string Pic { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime Creation { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool IsBanned { get; set; }
        public bool IsMaster { get; set; }
        public bool LicenceAgreement { get; set; }

        private bool IsAssigned { get; set; }

        public Account(MapleClient client)
        {
            this.Client = client;
        }

        public void Load(string username)
        {
            using (MySqlDataReader reader = LoginServer.Database.RunQuery("SELECT * FROM accounts WHERE Username = '{0}'", username))
            {
                if (reader.Read())
                {
                    this.IsAssigned = true;

                    this.ID = reader.GetInt32("ID");
                    this.Username = reader.GetString("Username");
                    this.Password = reader.GetString("Password");
                    this.Pin = reader.GetString("Pin");
                    this.Pic = reader.GetString("Pic");
                    this.Birthday = reader.GetDateTime("Birthday");
                    this.Creation = reader.GetDateTime("Creation");
                    this.IsLoggedIn = reader.GetBoolean("IsLoggedIn");
                    this.IsBanned = reader.GetBoolean("IsBanned");
                    this.IsMaster = reader.GetBoolean("IsMaster");
                    this.LicenceAgreement = reader.GetBoolean("LicenceAgreement");
                }
                else
                {
                    throw new NoAccountException();
                }
            }
        }

        public void Save()
        {
            if (this.IsAssigned)
            {
                StringBuilder query = new StringBuilder();

                query.Append("UPDATE accounts SET ");
                query.AppendFormat("{0} = '{1}', ", "Pin", this.Pin);
                query.AppendFormat("{0} = '{1}', ", "IsLoggedIn", this.IsLoggedIn);
                query.AppendFormat("{0} = '{1}' ", "LicenceAgreement", this.LicenceAgreement);
                query.AppendFormat("WHERE ID = '{0}'", this.ID);

                LoginServer.Database.RunQuery(query.ToString());
            }
            else
            {
                StringBuilder query = new StringBuilder();

                // TODO.

                LoginServer.Database.RunQuery(query.ToString());
            }
        }
    }
}
