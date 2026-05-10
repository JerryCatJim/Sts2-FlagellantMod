using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Meditation : FlagellantCardModel
{
    protected override bool HasEnergyCostX => true;
    public Meditation() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<RegenPower>(1);
        WithStress(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int num = ResolveEnergyXValue();
        num += IsUpgraded ? 1 : 0;
        await PowerCmd.Apply<RegenPower>(base.Owner.Creature, num, base.Owner.Creature, this);
        await PowerCmd.Apply<StressPower>(base.Owner.Creature, num, base.Owner.Creature, this);
    }
}
