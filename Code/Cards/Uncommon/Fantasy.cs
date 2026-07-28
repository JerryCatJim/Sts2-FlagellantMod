using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Core;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Fantasy : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => IsStressGreaterEqual() || FlagellantModel.IsInResoluteOrMeltdown<ToxicMeltdown>(base.Owner);
    public Fantasy() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithTip(new TooltipSource((CardModel _) => FlagellantHoverTipFactory.FromResoluteOrMeltdown<ToxicMeltdown>()));
        WithVars(new RepeatVar(2));
        WithStress(5);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int repeatTimes = IsStressGreaterEqual() || FlagellantModel.IsInResoluteOrMeltdown<ToxicMeltdown>(base.Owner)
            ? base.DynamicVars.Repeat.IntValue : 1;
        await CommonActions.CardAttack(this, cardPlay, repeatTimes).Execute(choiceContext);
    }
}
