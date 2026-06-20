using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Flagellant.Code.GameActions;

public sealed class PlayCardAnimAction : GameAction
{
    public override ulong OwnerId => Player.NetId;

    public override GameActionType ActionType => GameActionType.Combat;//CombatPlayPhaseOnly;

    public Player Player { get; }

    public NetCombatCard NetCombatCard { get; }

    public String CardAnimName;

    public float AnimWaitTime;

    public Creature PlayerCreature;

    public PlayCardAnimAction(CardModel cardModel, String animName, float waitTime = 0)
    {
        Player = cardModel.Owner;
        PlayerCreature = cardModel.Owner.Creature;
        NetCombatCard = NetCombatCard.FromModel(cardModel);
        CardAnimName = animName;
        AnimWaitTime = waitTime;
    }

    public PlayCardAnimAction(Player player, NetCombatCard netCombatCard, String animName, float waitTime = 0)
    {
        Player = player;
        PlayerCreature = Player.Creature;
        NetCombatCard = netCombatCard;
        CardAnimName = animName;
        AnimWaitTime = waitTime;
    }

    protected override async Task ExecuteAction()
    {
        await CreatureCmd.TriggerAnim(PlayerCreature, CardAnimName, AnimWaitTime);
    }

    protected override void CancelAction()
    {
        //CreatureCmd.TriggerAnim(PlayerCreature, "Idle", 0);
        NCreature? creatureNode = PlayerCreature.GetCreatureNode();
        if (creatureNode != null)
        {
            creatureNode.SetAnimationTrigger("Idle");
        }
    }

    public override INetAction ToNetAction()
    {
        NetPlayCardAnimAction netPlayCardAnimAction = new NetPlayCardAnimAction
        {
            card = NetCombatCard,
            animName = CardAnimName,
            waitTime = AnimWaitTime
        };
        return netPlayCardAnimAction;
    }

    public override string ToString()
    {
        return $"{"PlayCardAnimAction"} PlayerCreature: {PlayerCreature} AnimName: {CardAnimName}";
    }
}
