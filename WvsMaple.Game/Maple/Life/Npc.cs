using Common;
using Common.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WvsGame.Maple.Characters;
using WvsGame.Maple.Data;
using WvsGame.Maple.Fields;
using WvsGame.Maple.Scripting;
using WvsGame.Net;

namespace WvsGame.Maple.Life
{
    public class Npc : LifeObject, ISpawnable
    {
        public Character Controller { get; set; }

        public string Label { get; private set; }
        public int StorageCost { get; private set; }

        public Npc(string type, int id, int mapleId, Position position, short foothold, short minimumClickX, short maximumClickX, int respawnTime, bool facesLeft)
            : base(id, mapleId, position, foothold, minimumClickX, maximumClickX, respawnTime, facesLeft)
        {
            this.Label = MapleData.CachedNpcs.Labels[this.MapleID];
            //this.StorageCost = MapleData.CachedNpcs.StorageCosts[this.MapleID];
        }

        public void Operate(Packet inPacket, Character talker)
        {
            NpcScript script = talker.NpcSession;

            if (script == null)
            {
                return;
            }

            DialogType type = (DialogType)inPacket.ReadByte();

            if (type != script.LastSentType)
            {
                return;
            }

            byte selection = inPacket.ReadByte();

            switch (type)
            {
                case DialogType.Normal:
                    {
                        switch (selection)
                        {
                            case 0:

                                script.SetResponse(false);

                                break;

                            case 1:

                                script.SetResponse(true);

                                break;

                            default:

                                script.Stop();

                                break;
                        }
                    }
                    break;

                case DialogType.YesNo:
                    {
                        switch (selection)
                        {
                            case 0:

                                script.SetResponse(false);

                                break;

                            case 1:

                                script.SetResponse(true);

                                break;

                            default:

                                script.Stop();

                                break;
                        }
                    }
                    break;

                case DialogType.GetText:
                    {
                        switch (selection)
                        {
                            case 0:

                                script.Stop();

                                break;

                            case 1:

                                //script.StringAnswer = inPacket.ReadString();
                                script.Run();

                                break;

                            default:

                                script.Stop();

                                break;
                        }
                    }
                    break;

                case DialogType.Menu:
                case DialogType.Question:
                    {
                        switch (selection)
                        {
                            case 0:

                                script.Stop();

                                break;

                            case 1:

                                script.SetSelection(inPacket.ReadByte());

                                break;

                            default:

                                script.Stop();

                                break;
                        }
                    }
                    break;
            }
        }

        public void Act(Packet inPacket)
        {
            using (Packet p = new Packet(ServerMessages.NpcSetSpecialAction))
            {
                p.WriteInt(this.ID);

                if (inPacket.Remaining == 2)
                {
                    p.WriteShort(inPacket.ReadShort());
                }
                else if (inPacket.Remaining > 2)
                {
                    p.WriteBytes(inPacket.ReadLeftoverBytes());
                }

                this.Controller.Field.Broadcast(p);
            }
        }

        public void Converse()
        {
            if (GameServer.Scripts[ScriptType.Npc].ContainsKey(this.Label))
            {
                try
                {
                    this.Controller.NpcSession = (NpcScript)Activator.CreateInstance(GameServer.Scripts[ScriptType.Npc][this.Label]);
                    this.Controller.LastNpc = this;

                    this.Controller.NpcSession.Initiate(this.MapleID, this.Controller);
                    this.Controller.NpcSession.Run();
                }
                catch (Exception e)
                {
                    MainForm.Instance.Log("Character '{0}' failed to converse with Npc '{1}':\r\n {2}", this.Controller.Name, this.Label, e.ToString());
                }
            }
            else
            {
                MainForm.Instance.Log("Character '{0}' tried to converse with an unimplemented Npc '{1}'.", this.Controller.Name, this.Label);
            }
        }

        public void AssignController()
        {
            if (this.Controller == null)
            {
                int leastControlled = int.MaxValue;
                Character newController = null;

                lock (this.Field.Characters)
                {
                    foreach (Character loopCharacter in this.Field.Characters)
                    {
                        if (loopCharacter.ControlledNpcs.Count < leastControlled)
                        {
                            leastControlled = loopCharacter.ControlledNpcs.Count;
                            newController = loopCharacter;
                        }
                    }
                }

                if (newController != null)
                {
                    newController.ControlledNpcs.Add(this);
                }
            }
        }

        private Packet GetInternalPacket(bool requestControl)
        {
            Packet spawn = new Packet(requestControl ? ServerMessages.NpcChangeController : ServerMessages.NpcEnterField);

            if (requestControl)
            {
                spawn.WriteBool(true);
            }

            spawn.WriteInt(this.ID);
            spawn.WriteInt(this.MapleID);
            spawn.WriteShort(this.Position.X);
            spawn.WriteShort(this.Position.Y);
            spawn.WriteBool(!this.FacesLeft);
            spawn.WriteShort(this.Foothold);
            spawn.WriteShort(this.MinimumClickX);
            spawn.WriteShort(this.MaximumClickX);

            return spawn;
        }

        public Packet GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public Packet GetSpawnPacket()
        {
            return this.GetInternalPacket(false);
        }

        public Packet GetDestroyPacket()
        {
            Packet destroy = new Packet(ServerMessages.NpcLeaveField);

            destroy.WriteInt(this.ObjectID);

            return destroy;
        }

        public Packet GetControlRequestPacket()
        {
            return this.GetInternalPacket(true);
        }

        public Packet GetControlCancelPacket()
        {
            Packet cancelControl = new Packet(ServerMessages.NpcChangeController);

            cancelControl.WriteByte(0);
            cancelControl.WriteInt(this.ObjectID);

            return cancelControl;
        }
    }
}
