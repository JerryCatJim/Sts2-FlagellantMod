using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Relics;

[Pool(typeof(FlagellantRelicPool))]
public class DismassHead : FlagellantRelicModel, IAfterComboChanged
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ComboPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    public async Task AfterComboChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel? cardSource)
    {
        if (amount <= 0m || applier != Owner.Creature) return;

        Flash();
        await PowerCmd.Apply<VulnerablePower>(choiceContext, power.Owner, 1, Owner.Creature, null);
    }
}