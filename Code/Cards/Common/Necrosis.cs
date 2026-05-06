using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class Necrosis : FlagellantCardModel
{
    public Necrosis() : base(2, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithAnimName("Necrosis");
        WithPowerTip<StressPower>();
        WithPowerTip<PoisonPower>();
        WithCostUpgradeBy(-1);
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
