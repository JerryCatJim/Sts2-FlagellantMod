using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class BarristansHead : FlagellantRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != base.Owner.Creature.Side) return;

        decimal stressNum = base.Owner.Creature.GetPower<StressPower>()?.Amount ?? 0;
        if(stressNum > 0)
        {
            Flash();
            await CreatureCmd.GainBlock(base.Owner.Creature, stressNum, ValueProp.Move, null);
        }
    }
}