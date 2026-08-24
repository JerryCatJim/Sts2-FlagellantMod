using BaseLib.Extensions;
using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class Riposte : FlagellantCardModel
{
    private decimal _lastLostHp = 0;
    public Riposte() : base(1, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
    {
        WithLossPercent(12);
        WithVars(new RepeatVar(3).WithUpgrade(1));
        WithCalculatedDamage(0, ((CardModel card, Creature? c) =>
        {
            if(card != null && card is Riposte myCard)
            {
                if (myCard._lastLostHp != 0)
                {
                    return myCard._lastLostHp;
                }
                else
                {
                    return myCard.GetLossPercentHp();
                }
            }
            return 0;
        }
        ));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _lastLostHp = GetLossPercentHp();
        await CreatureCmd.Damage(choiceContext, Owner.Creature, _lastLostHp, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        await CommonActions.CardAttack(this, cardPlay, base.DynamicVars.Repeat.IntValue).Execute(choiceContext);
        _lastLostHp = 0;
    }
}