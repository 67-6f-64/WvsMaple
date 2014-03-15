using Common.Net;

namespace WvsGame.Maple.Fields
{
    public interface ISpawnable
    {
        Packet GetCreatePacket();
        Packet GetSpawnPacket();
        Packet GetDestroyPacket();
    }
}
