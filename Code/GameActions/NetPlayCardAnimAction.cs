/*using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.GameActions;

namespace Flagellant.Code.GameActions;

public struct NetPlayCardAnimAction : INetAction, IPacketSerializable
{
    public NetCombatCard card;
    public String animName;
    public float waitTime;

    public GameAction ToGameAction(Player player)
    {
        return new PlayCardAnimAction(player, card, animName, waitTime);
    }

    public void Serialize(PacketWriter writer)
    {
        writer.Write(card);
        writer.WriteString(animName);
        writer.WriteFloat(waitTime);
    }

    public void Deserialize(PacketReader reader)
    {
        card = reader.Read<NetCombatCard>();
        animName = reader.ReadString();
        waitTime = reader.ReadFloat();
    }

    public override string ToString()
    {
        return $"NetPlayAnimAction ({card}) animName: {animName}";
    }
}
*/