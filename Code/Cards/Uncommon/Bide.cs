using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Cards.Token;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Bide : FlagellantCardModel
{
    protected override bool HasEnergyCostX => true;
    public Bide() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.FromCard<Penance>(_.IsUpgraded)));
        WithAnimName("More");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;

        await PlayCardAnim();
        int num = base.IsUpgraded ? ResolveEnergyXValue() + 1 : ResolveEnergyXValue();
        IEnumerable<Penance> enumerable = Penance.Create(base.Owner, num, base.CombatState);
        if (base.IsUpgraded)
        {
            foreach (Penance item in enumerable)
            {
                CardCmd.Upgrade(item);
            }
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(enumerable, PileType.Draw, base.Owner, CardPilePosition.Random));
    }
}
