using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Data
{
    public class Database
    {
        private MySqlConnection Connection { get; set; }
        private MySqlCommand Command { get; set; }
        public MySqlDataReader Reader { get; set; }
        private string mConnectString { get; set; }
        public bool mShuttingDown { get; set; }

        public Database(string username, string password, string database, string server, ushort port = 3306)
        {
            mShuttingDown = false;
            mConnectString = "Server=" + server + "; Port=" + port + "; Database=" + database + "; Uid=" + username + "; Pwd=" + password + ";Convert Zero DateTime=True";
            Connect();
        }

        public void Connect()
        {
            try
            {
                //FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Connecting...", DateTime.Now.ToString()), true);
                Connection = new MySqlConnection(mConnectString);
                //FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] State Change...", DateTime.Now.ToString()), true);
                Connection.StateChange += new System.Data.StateChangeEventHandler(Connection_StateChange);
                //FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Opening Connection", DateTime.Now.ToString()), true);
                Connection.Open();
                //FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Connected with MySQL server with version info: {1} and uses {2}compression", DateTime.Now.ToString(), Connection.ServerVersion, Connection.UseCompression ? "" : "no "), true);
                Console.WriteLine("Connected with MySQL server with version info: {0} and uses {1}compression", Connection.ServerVersion, Connection.UseCompression ? "" : "no ");
            }
            catch (MySqlException ex)
            {
                //FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::Connect() : {1}", DateTime.Now.ToString(), ex.ToString()), true);

                Console.WriteLine(ex.ToString());
                throw new Exception(string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::Connect() : {1}", DateTime.Now.ToString(), ex.ToString()));
            }
            catch (Exception ex)
            {
                //FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::Connect() : {1}", DateTime.Now.ToString(), ex.ToString()), true);
                throw new Exception(string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::Connect() : {1}", DateTime.Now.ToString(), ex.ToString()));
            }
        }

        void Connection_StateChange(object sender, System.Data.StateChangeEventArgs e)
        {
            if (e.CurrentState == System.Data.ConnectionState.Closed && !mShuttingDown)
            {
                Console.WriteLine("MySQL Connection closed. Reconnecting!");
                Connection.StateChange -= Connection_StateChange;
                Connect();
            }
            else if (e.CurrentState == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("MySQL Connection opened!");
            }
        }

        public MySqlDataReader RunQuery(string query, params object[] args)
        {
            try
            {

                if (Reader != null && !Reader.IsClosed)
                {
                    Reader.Close();
                    Reader.Dispose();
                    Reader = null;
                }
                Command = new MySqlCommand(String.Format(query, args), Connection);
                if (query.StartsWith("SELECT"))
                {
                    Reader = Command.ExecuteReader();
                    return Reader;
                }
                else if (query.StartsWith("DELETE") || query.StartsWith("UPDATE") || query.StartsWith("INSERT") || query.StartsWith("DROP") || query.StartsWith("CREATE"))
                {
                    Command.ExecuteNonQuery();
                    return null;
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Lost connection to DB... Trying to reconnect and wait a second before retrying to run query.");
                Connect();
                System.Threading.Thread.Sleep(1000);
                RunQuery(query);
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 2055)
                {
                    Console.WriteLine("Lost connection to DB... Trying to reconnect and wait a second before retrying to run query.");
                    Connect();
                    System.Threading.Thread.Sleep(1000);
                    RunQuery(query);
                }
                else
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(query);
                    //FileWriter.WriteLine("Logs\\DB_crashes.txt", string.Format("[{0}][DB LIB] Got exception @ MySQL_Connection::RunQuery({1}) : {2}", DateTime.Now.ToString(), query, ex.ToString()), true);
                    throw new Exception(("[" + DateTime.Now.ToString() + "][DB LIB] Got exception @ MySQL_Connection::RunQuery(" + string.Format(query, args) + ") : " + ex.ToString()));
                }
            }
            return null;
        }

        public void Delete(string table, string constraints, params object[] args)
        {
            this.RunQuery("DELETE FROM {0} WHERE {1}", table, string.Format(constraints, args));
        }

        public bool Exists(string table, string constraints, params object[] args)
        {
            this.RunQuery("SELECT * FROM {0} WHERE {1}", table, String.Format(constraints, args));
            MySqlDataReader reader = this.Reader;

            return reader.HasRows;
        }

        public dynamic Fetch(string table, string field, string constraints, params object[] args)
        {
            using (MySqlDataReader reader = this.RunQuery("SELECT {0} FROM {1} WHERE {2}", field, table, String.Format(constraints, args)))
            {
                if (reader.Read())
                {
                    return reader.GetValue(0);
                }
                else
                {
                    return null;
                }
            }
        }

        public int Count(string table, string constraints, params object[] args)
        {
            this.RunQuery("SELECT COUNT(*) FROM {0} WHERE {1}", table, String.Format(constraints, args));
            MySqlDataReader reader = this.Reader;

            if (!reader.HasRows)
                return 0;
            else
            {
                reader.Read();

                int count = reader.GetInt32(0);

                reader.Close();

                return count;
            }
        }

        private object GetObject(PropertyInfo info, MySqlDataReader reader)
        {
            var type = info.PropertyType;
            var name = info.Name;

            if (type == typeof(bool))
                return reader.GetBoolean(name);
            if (type == typeof(byte))
                return reader.GetByte(name);
            else if (type == typeof(short))
                return reader.GetInt16(name);
            else if (type == typeof(int))
                return reader.GetInt32(name);
            else if (type == typeof(string))
                return reader.GetString(name);

            return null;
        }

        public bool LoadProperties(object obj, string table, int id)
        {
            string txt = string.Format("SELECT * FROM `{0}` WHERE id = {1}", table, id);
            this.RunQuery(txt);

            var query = this.Reader;

            if (query.Read())
            {
                var properties = obj.GetType().GetProperties();

                foreach (var prop in properties)
                {
                    var value = GetObject(prop, query);
                    prop.SetValue(obj, value, null);
                }

                query.Close();
                return true;
            }

            query.Close();
            return false;
        }
        public void SaveProperties(object obj, string table, int id)
        {
            var update = new StringBuilder();
            update.AppendFormat("UPDATE `{0}` SET ", table);

            var properties = obj.GetType().GetProperties();

            bool first = true;

            foreach (var prop in properties)
            {
                if (first)
                    first = false;
                else
                    update.Append(',');


                var value = prop.GetValue(obj, null);
                update.AppendFormat("`{0}` = ", prop.Name);

                if (prop.PropertyType == typeof(string))
                    update.AppendFormat("'{0}'", value);
                else
                    update.Append(value);

            }

            update.AppendFormat(" WHERE id = {0}", id);

            this.RunQuery(update.ToString());
        }
        public long InsertProperties(object obj, string table)
        {
            var insert = new StringBuilder();
            var values = new StringBuilder();

            insert.AppendFormat("INSERT `{0}`(", table);
            values.Append("VALUES(");

            var properties = obj.GetType().GetProperties();

            bool first = true;

            foreach (var prop in properties)
            {
                if (first)
                    first = false;
                else
                {
                    insert.Append(',');
                    values.Append(',');
                }

                insert.AppendFormat("`{0}`", prop.Name);

                var item = prop.GetValue(obj, null);

                if (prop.PropertyType == typeof(string))
                    values.AppendFormat("'{0}'", item);
                else
                    values.Append(item);
            }

            this.RunQuery("{0}) {1})", insert.ToString(), values.ToString());
            return this.GetLastInsertId();
        }

        public int GetLastInsertId()
        {
            return (int)Command.LastInsertedId;
        }

        public bool Update()
        {
            if (!Directory.Exists(@"..\DataSvr\Sql"))
                return false;

            List<int> pastExecutedQueries = new List<int>();
            List<int> executedQueries = new List<int>();

            this.RunQuery("SELECT QueryID FROM executed_queries");

            while (this.Reader.Read())
            {
                pastExecutedQueries.Add(this.Reader.GetInt32("QueryID"));
            }

            Dictionary<int, List<string>> toExecute = new Dictionary<int, List<string>>();

            foreach (var file in Directory.GetFiles(@"..\DataSvr\Sql"))
            {
                FileInfo fileInfo = new FileInfo(file);
                int queryID = int.Parse(fileInfo.Name.Split('-')[0]);

                if (pastExecutedQueries.Contains(queryID))
                    continue;

                List<string> queries = new List<string>();
                string buffer = "";
                StreamReader reader = fileInfo.OpenText();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line == "####################################################")
                    {
                        queries.Add(buffer);
                        buffer = "";
                    }
                    else
                        buffer += line;
                }

                if (buffer != "")
                    queries.Add(buffer);

                reader.Close();

                toExecute.Add(queryID, queries);
            }

            int ranQueries = 0;

            foreach (var queries in toExecute)
            {
                foreach (var query in queries.Value)
                {
                    this.RunQuery(query);
                    ranQueries++;
                }

                executedQueries.Add(queries.Key);
            }

            if (executedQueries.Count > 0)
            {
                StringBuilder query = new StringBuilder("INSERT INTO executed_queries VALUES ");

                foreach (int queryID in executedQueries)
                {
                    query.AppendFormat("({0}),", queryID);
                }

                query = query.Remove(query.Length - 1, 1);

                this.RunQuery(query.ToString());
            }

            return true;
        }

        public bool Ping()
        {
            if (Reader != null && !Reader.IsClosed)
                return false;
            return Connection.Ping();
        }
    }
}
