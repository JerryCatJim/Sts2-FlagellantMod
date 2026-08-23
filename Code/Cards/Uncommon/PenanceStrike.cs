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

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class PenanceStrike : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyComboMarkedEnemy;
    public PenanceStrike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithDamage(9, 3);
        WithVar("CreateCards", 3);
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.FromCard<Penance>()));
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.FromCard<Penance>(true)));
        WithPower<ComboPower>(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        int createCards = 1;
        bool hasCombo = false;
        if (cardPlay.Target.GetPower<ComboPower>() is ComboPower comboP)
        {
            hasCombo = true;
            //DO NOT use Decrement or Remove, Need receive the applier to trigger ComboPower amount decreased Event by "AfterComboChanged"(IAfterComboChanged Interface).
            await PowerCmd.ModifyAmount(choiceContext, comboP, -1, Owner.Creature, this);
            createCards = base.DynamicVars["CreateCards"].IntValue;
        }
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        IEnumerable<Penance> enumerable = Penance.Create(base.Owner, createCards, base.CombatState);
        if (hasCombo)
        {
            foreach (Penance item in enumerable)
            {
                CardCmd.Upgrade(item);
            }
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(enumerable, PileType.Draw, base.Owner, CardPilePosition.Random));
    }
}
