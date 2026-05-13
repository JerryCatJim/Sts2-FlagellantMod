using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Necrosis : FlagellantCardModel
{
    public Necrosis() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithAnimName("Necrosis");
        WithPowerTip<StressPower>();
        WithPowerTip<PoisonPower>();
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
        WithBlock(1); //Just for displaying keyword.
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null && cardPlay.Target.HasPower<PoisonPower>() && cardPlay.Target.HasPower<ComboPower>())
        {
            await PlayCardAnim();
            decimal StressNum = Owner.Creature.GetPower<StressPower>()?.Amount ?? 0;
            if(StressNum > 0)
            {
                await CommonActions.Apply<PoisonPower>(cardPlay.Target, this, StressNum);
            }
            decimal blockNum = cardPlay.Target.GetPower<PoisonPower>()?.Amount ?? 0;
            if (blockNum > 0)
            {
                await CreatureCmd.GainBlock(base.Owner.Creature, blockNum, ValueProp.Move, cardPlay);
            }
        }
    }
}
