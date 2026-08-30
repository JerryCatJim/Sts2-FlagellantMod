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
public class RelievedStrike : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => IsStressGreaterEqual();
    public RelievedStrike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithDamage(6, 3);
        WithStress(5);
        WithEnergy(1);
        WithCards(1);
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.FromCard<Penance>()));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        if(IsStressGreaterEqual() && base.CombatState != null)
        {
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
            IEnumerable<Penance> enumerable = Penance.Create(base.Owner, base.DynamicVars.Cards.IntValue, base.CombatState);
            await CardPileCmd.AddGeneratedCardsToCombat(enumerable, PileType.Hand, base.Owner);
        }
    }
}
