using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class BloodDonation : FlagellantCardModel
{
    private decimal _maxHpDamage = 0;
    public BloodDonation() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithHealingPercent(12,4); //并不是真的回血，是获取最大生命值百分比的数值
        WithLossPercent(12,-4);
        WithAnimName("AcidRain");
        WithCalculatedDamage(0, ((CardModel card, Creature? c) =>
        {
            if (card != null && card is BloodDonation myCard)
            {
                if (myCard._maxHpDamage != 0)
                {
                    return myCard._maxHpDamage;
                }
                else
                {
                    return myCard.GetHealingPercentHp();
                }
            }
            return 0;
        }
        ));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _maxHpDamage = GetHealingPercentHp();
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, GetLossPercentHp(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        _maxHpDamage = 0;
    }
}
