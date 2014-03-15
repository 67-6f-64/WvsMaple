using Common;
using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WvsGame.Maple.Characters;
using WvsGame.Maple.Data;
using WvsGame.Maple.Events;
using WvsGame.Maple.Fields;

namespace WvsGame.Maple.Scripting
{
    public abstract class NpcScript
    {
        public int MapleID { get; private set; }

        public Character Talker { get; private set; }
        public DialogType LastSentType { get; private set; }

        public string Text { get; private set; }

        public TaskCompletionSource<bool> Response { get; private set; }
        public TaskCompletionSource<byte> Selection { get; private set; }
        public TaskCompletionSource<int> Number { get; private set; }

        public void Initiate(int npcId, Character talker)
        {
            this.MapleID = npcId;
            this.Talker = talker;
        }

        public abstract Task Run();

        public void Stop()
        {
            this.Talker.LastNpc = null;
            this.Talker.NpcSession = null;
        }

        public void SetResponse(bool response)
        {
            this.Response.SetResult(response);
        }

        public void SetSelection(byte selection)
        {
            this.Selection.SetResult(selection);
        }

        public void SetNumber(int number)
        {
            this.Number.SetResult(number);
        }

        public void AddText(string text)
        {
            this.Text += text;
        }

        public async Task<bool> SendOk()
        {
            this.Response = new TaskCompletionSource<bool>();

            using (Packet outPacket = this.GetInternalPacket(DialogType.Normal))
            {
                outPacket.WriteBytes(0, 0);

                this.Talker.Client.Send(outPacket);
            }

            return await this.Response.Task;
        }

        public async Task<bool> SendNext()
        {
            this.Response = new TaskCompletionSource<bool>();

            using (Packet outPacket = this.GetInternalPacket(DialogType.Normal))
            {
                outPacket.WriteBytes(0, 1);

                this.Talker.Client.Send(outPacket);
            }

            return await this.Response.Task;
        }

        public void SendPrev()
        {
            using (Packet outPacket = this.GetInternalPacket(DialogType.Normal))
            {
                outPacket.WriteBytes(1, 0);

                this.Talker.Client.Send(outPacket);
            }
        }

        public void SendNextPrev()
        {
            using (Packet outPacket = this.GetInternalPacket(DialogType.Normal))
            {
                outPacket.WriteBytes(1, 1);

                this.Talker.Client.Send(outPacket);
            }
        }

        public async Task<bool> SendYesNo()
        {
            this.Response = new TaskCompletionSource<bool>();

            using (Packet outPacket = this.GetInternalPacket(DialogType.YesNo))
            {
                this.Talker.Client.Send(outPacket);
            }

            return await this.Response.Task;
        }

        public async Task<int> SendGetNumber(int def, int low, int high)
        {
            this.Number = new TaskCompletionSource<int>();

            using (Packet outPacket = this.GetInternalPacket(DialogType.GetNumber))
            {
                this.Talker.Client.Send(outPacket);
            }

            return await this.Number.Task;
        }

        public async Task<byte> SendMenu(params string[] selections)
        {
            this.Selection = new TaskCompletionSource<byte>();

            if (selections.Length > 0)
            {
                AddText("#b\r\n");
            }

            for (int i = 0; i < selections.Length; i++)
            {
                AddText("#L" + i + "#" + selections[i] + "#l\r\n");
            }

            using (Packet outPacket = this.GetInternalPacket(DialogType.Menu))
            {
                outPacket.WriteBytes(1, 1);

                this.Talker.Client.Send(outPacket);
            }

            return await this.Selection.Task;
        }

        public Event GetEvent(string name)
        {
            return EventFactory.Events.Contains(name) ? EventFactory.Events[name] : null;
        }

        public void SetField(int fieldId, byte portalId = 0)
        {
            this.Talker.SetField(fieldId, portalId);
        }

        public void SetField(int fieldId, string portalLabel)
        {
            this.Talker.SetField(fieldId, MapleData.CachedFields[fieldId].Portals[portalLabel].ID);
        }

        public byte GetLevel()
        {
            return this.Talker.Statistics.Level;
        }

        public short GetJob()
        {
            return this.Talker.Statistics.Job;
        }

        public bool GainMeso(int amount)
        {
            if (this.Talker.Meso >= amount)
            {
                this.Talker.Meso -= amount;
                return true;
            }

            return false;
        }

        public bool GainItem(int mapleId, int quantity = 1)
        {
            if (quantity < 0)
            {
                this.Talker.Inventory.Remove(mapleId, (short)quantity);

                return true;
            }
            else
            {
                Item item = new Item(mapleId, (short)quantity);

                if (this.Talker.Inventory.RemainingSlots(ItemType.Etcetera) > 0)
                {
                    this.Talker.Inventory.Add(item);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool HasItem(int mapleId, short quantity = 1)
        {
            return this.Talker.Inventory.Available(mapleId) >= quantity;
        }

        public bool HasOpenSlotsFor(int mapleId)
        {
            return this.Talker.Inventory.RemainingSlots(Item.GetType(mapleId)) > 0;
        }

        public bool HasQuestStarted(ushort id)
        {
            return this.Talker.Quests.Started.ContainsKey(id);
        }

        public bool HasQuestCompleted(ushort id)
        {
            return this.Talker.Quests.Completed.ContainsKey(id);
        }

        private Packet GetInternalPacket(DialogType type, bool addText = true)
        {
            this.LastSentType = type;

            Packet dialog = new Packet(ServerMessages.ScriptMessage);

            dialog.WriteByte(4);
            dialog.WriteInt(this.MapleID);
            dialog.WriteByte((byte)this.LastSentType);

            if (addText)
            {
                dialog.WriteString(this.Text);
                this.Text = "";
            }

            return dialog;
        }
    }
}
