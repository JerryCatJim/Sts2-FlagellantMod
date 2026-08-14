using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class Deathless : FlagellantCardModel
{
    public Deathless() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithAnimName("Deathless");
        WithHealingPercent(35, 5);
        WithPowerTip<StressPower>();
        WithKeyword(CardKeyword.Exhaust, UpgradeType.None);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        if (base.Owner.Creature.GetPower<StressPower>() is StressPower SP)
        {
            await PowerCmd.ModifyAmount(choiceContext, SP, -SP.Amount, Owner.Creature, this);
        }
        await CreatureCmd.Heal(Owner.Creature, GetHealingPercentHp());
    }
}
