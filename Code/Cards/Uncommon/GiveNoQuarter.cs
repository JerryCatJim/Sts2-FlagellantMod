using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class GiveNoQuarter : FlagellantCardModel
{
    private bool ShouldApplyGiveNoQuarterPower = false;
    protected override bool ShouldGlowGoldInternal => HasAnyComboMarkedEnemy;
    public GiveNoQuarter() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPower<GiveNoQuarterPower>(1);
        WithPower<ComboPower>(1);
        WithDamage(8,2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Target !=  null && cardPlay.Target.GetPower<ComboPower>() is ComboPower comboP)
        {
            await PowerCmd.ModifyAmount(choiceContext, comboP, -1, base.Owner.Creature, this);
            ShouldApplyGiveNoQuarterPower = true;
            //await CommonActions.ApplySelf<GiveNoQuarterPower>(choiceContext, this);
        }
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        //刚获得能力之后会收到附加该能力卡牌的打出完成事件，所以不要在OnPlay中应用power，而是在AfterCardPlayed应用power
        if (cardPlay.Card.Owner.Creature == base.Owner.Creature
            && cardPlay != null
            && cardPlay.Card is GiveNoQuarter GQ && GQ == this)
        {
            if(GQ.ShouldApplyGiveNoQuarterPower)
            {
                await CommonActions.ApplySelf<GiveNoQuarterPower>(choiceContext, GQ);
                GQ.ShouldApplyGiveNoQuarterPower = false;
            }
        }
    }
}
