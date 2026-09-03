using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Cards.Token;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class RelievedStrike : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyComboMarkedEnemy;
    public RelievedStrike() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithPower<ComboPower>(1);
        WithDamage(8, 2);
        WithEnergy(2);
        WithCards(1);
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.FromCard<Penance>(_.IsUpgraded)));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        bool hasCombo = false;
        if (cardPlay.Target.GetPower<ComboPower>() is ComboPower comboP)
        {
            hasCombo = true;
            await PowerCmd.ModifyAmount(choiceContext, comboP, -1, base.Owner.Creature, this);
        }
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        if (hasCombo && base.CombatState != null)
        {
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner);
            IEnumerable<Penance> enumerable = Penance.Create(base.Owner, base.DynamicVars.Cards.IntValue, base.CombatState);
            if (IsUpgraded)
            {
                foreach (Penance item in enumerable)
                {
                    CardCmd.Upgrade(item);
                }
            }
            await CardPileCmd.AddGeneratedCardsToCombat(enumerable, PileType.Hand, base.Owner);
        }
    }
}
