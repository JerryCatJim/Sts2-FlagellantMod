using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class PressThisAdvantage : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyComboMarkedEnemy;
    public PressThisAdvantage() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<PressThisAdvantagePower>(1);
        WithPower<ComboPower>(1);
        WithDamage(6,3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        if(cardPlay.Target !=  null && cardPlay.Target.GetPower<ComboPower>() is ComboPower comboP)
        {
            await PowerCmd.ModifyAmount(comboP, -1, base.Owner.Creature, this);
            await CommonActions.ApplySelf<PressThisAdvantagePower>(this);
        }
    }
}
