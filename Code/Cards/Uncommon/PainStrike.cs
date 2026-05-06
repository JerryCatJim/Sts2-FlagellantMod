using BaseLib.Utils;
using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class PainStrike : FlagellantCardModel
{
    public PainStrike() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithDamage(9);
        WithPowerTip<StressPower>();
        WithVar("Multiplier", 1, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal multiDamage = base.DynamicVars["Multiplier"]?.BaseValue ?? 0;
        int stressNum = Owner.Creature.HasPower<StressPower>() ? (Owner.Creature?.GetPower<StressPower>()?.Amount ?? 0) : 0;
        await CommonActions.CardAttack(this, cardPlay.Target, base.DynamicVars.Damage.BaseValue+stressNum*multiDamage).Execute(choiceContext);
    }
}
