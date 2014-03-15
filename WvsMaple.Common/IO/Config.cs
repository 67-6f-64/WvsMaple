using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.IO
{
    public class Config : IDisposable
    {
        private string File { get; set; }
        private StreamReader Reader;

        public int Line { get; set; }

        private List<string> ConfigText = new List<string>();

        public Config(string filename)
        {
            File = filename;
            if (System.IO.File.Exists(filename))
            {
                Reader = new StreamReader(filename, true);
            }
            else
            {
                throw new FileNotFoundException("Unable to locate configuration file '" + File + "'.");
            }
            string line = "";
            while (!Reader.EndOfStream)
            {
                line = Reader.ReadLine().Replace("\t", ""); // Remove tabs
                line = line.Trim();
                ConfigText.Add(line);
            }
        }

        ~Config()
        {
            if (Reader != null)
                Reader.Close();
        }

        public void Close()
        {
            if (Reader != null)
            {
                Reader.Close();
                Reader.Dispose();
            }
        }

        private string GetValue(string sBlock, string sParameter)
        {
            bool startPart = false;
            string ans = "";
            Line = 0;
            foreach (string line in ConfigText)
            {
                Line++;
                if (sBlock != "" && !startPart && line == sBlock + " = {")
                {
                    // Found beginning of block
                    startPart = true;
                }
                else if (startPart && line == "}")
                {
                    // Found end of block while begin found already
                    ans = "";
                    break;
                    //throw new InvalidOperationException("Parameter '" + sParameter + "' not found in block '" + sBlock + "'. (line: " + Line.ToString() + ")");
                }
                else if (line.StartsWith(sParameter + " = "))
                {
                    if (sBlock == "")
                    {
                        ans = line.Replace(sParameter + " = ", "");
                        break;
                    }
                    else if (sBlock != "" && startPart)
                    {
                        ans = line.Replace(sParameter + " = ", "");
                        break;
                    }
                }
            }
            return ans.Trim();
        }

        public List<string> GetBlocks(string sMainBlock, bool skipBlocksInsideBlock)
        {
            List<string> ret = new List<string>();
            int block = 0; // Start out of a block
            Line = 0;
            foreach (string line in ConfigText)
            {
                Line++;
                if (block == 0 && line == sMainBlock + " = {")
                {
                    block = 1;
                }
                else if (block == 1 && line == "}")
                {
                    block = 0;
                    break;
                }
                else
                {
                    if (block >= 1)
                    {
                        if (line.Contains(" = {"))
                        {
                            // Another block found
                            block++;
                            ret.Add(line.Replace(" = {", ""));
                        }
                        else if (line == "}")
                        {
                            // Block end found
                            block--;
                        }
                    }
                }
            }
            return ret;
        }

        public List<string> GetBlocksFromBlock(string sMainBlock, int innerBlock)
        {
            List<string> ret = new List<string>();
            int block = sMainBlock == "" ? 1 : 0;
            int skipBlock = 0;
            Line = 0;
            foreach (string line in ConfigText)
            {
                Line++;
                if (block == 0 && line == sMainBlock + " = {")
                {
                    block = 1;
                }
                else if (block == 1 && line == "}")
                {
                    block = 0;
                    break;
                }
                else
                {
                    if (block >= 1)
                    {
                        if (line.Contains(" = {"))
                        {
                            // Another block found
                            if (block <= innerBlock)
                            {
                                block++;
                                ret.Add(line.Replace(" = {", ""));
                            }
                            else
                            {
                                skipBlock++; // For skipping the '}' 's
                            }
                        }
                        else if (line == "}")
                        {
                            // Block end found
                            if (skipBlock == 0)
                                block--;
                            else
                                skipBlock--;
                        }
                    }
                }
            }
            return ret;
        }

        public string GetString(string sBlock, string sParameter)
        {
            return GetValue(sBlock, sParameter);
        }

        public int GetInt(string sBlock, string sParameter)
        {
            string val = GetValue(sBlock, sParameter);
            int retval = 0;
            int.TryParse(val, out retval);
            return retval;
        }

        public uint GetUInt(string sBlock, string sParameter)
        {
            string val = GetValue(sBlock, sParameter);
            uint retval = 0;
            uint.TryParse(val, out retval);
            return retval;
        }

        public short GetShort(string sBlock, string sParameter)
        {
            string val = GetValue(sBlock, sParameter);
            short retval = 0;
            short.TryParse(val, out retval);
            return retval;
        }

        public ushort GetUShort(string sBlock, string sParameter)
        {
            string val = GetValue(sBlock, sParameter);
            ushort retval = 0;
            ushort.TryParse(val, out retval);
            return retval;
        }

        public byte GetByte(string sBlock, string sParameter)
        {
            string val = GetValue(sBlock, sParameter);
            byte retval = 0;
            byte.TryParse(val, out retval);
            return retval;
        }

        public bool GetBool(string sBlock, string sParameter)
        {
            string val = GetValue(sBlock, sParameter);
            bool retval = false;
            bool.TryParse(val, out retval);
            return retval;
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}
