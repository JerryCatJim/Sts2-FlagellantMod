using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using System.Linq;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class BarristansHead : FlagellantRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StressPower>()
    ];

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        //if (side != base.Owner.Creature.Side) return;
        if (!participants.Contains(base.Owner.Creature)) return;

        decimal stressNum = base.Owner.Creature.GetPower<StressPower>()?.Amount ?? 0;
        if(stressNum > 0)
        {
            Flash();
            await CreatureCmd.GainBlock(base.Owner.Creature, stressNum, ValueProp.Move, null);
        }
    }
}