using BaseLib.Extensions;
using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class Riposte : FlagellantCardModel
{
    public Riposte() : base(1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
    {
        WithLossPercent(8);
        WithVars(new RepeatVar(3).WithUpgrade(1));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal damageNum = GetLossPercentHp();
        await CreatureCmd.Damage(choiceContext, Owner.Creature, damageNum, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        await CommonActions.CardAttack(this, cardPlay.Target, damageNum, base.DynamicVars.Repeat.IntValue).Execute(choiceContext);
    }
}