using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class Deathless : FlagellantCardModel
{
    public Deathless() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithAnimName("Endure");
        WithHealingPercent(35, 5);
        WithPowerTip<DoomPower>();
        WithKeyword(CardKeyword.Exhaust, UpgradeType.None);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CreatureCmd.Heal(Owner.Creature, GetHealingPercentHp());
        await CommonActions.ApplySelf<DoomPower>(this, GetHealingPercentHp());
    }
}
