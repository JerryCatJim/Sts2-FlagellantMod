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
public class Pilgrimage : FlagellantCardModel
{
    public Pilgrimage() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithAnimName("Suffer");
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.FromCard<Penance>()));
        WithCards(2, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;

        await PlayCardAnim();
        IEnumerable<Penance> enumerable = Penance.Create(base.Owner, base.DynamicVars.Cards.IntValue, base.CombatState);
        await CardPileCmd.AddGeneratedCardsToCombat(enumerable, PileType.Hand, base.Owner);
    }
}
