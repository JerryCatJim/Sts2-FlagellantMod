using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class HisPrison : FlagellantRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        //ThornsPower is ValueProp.SkipHurtAnim, don't trigger relic when received damage from it.
        if (target == base.Owner.Creature && dealer != null && dealer.IsMonster && props != ValueProp.SkipHurtAnim)
        {
            Flash();
            await PowerCmd.Apply<PoisonPower>(dealer, 1, Owner.Creature, null);
        }
    }
}