using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class BloodWave : FlagellantCardModel
{
    public BloodWave() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithHealingPercent(5, 2);
        WithDamage(5, 2);
        WithPowerTip<RegenPower>();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        decimal healNum = GetHealingPercentHp();
        if (base.Owner.Creature.HasPower<RegenPower>())
        {
            await CreatureCmd.Heal(base.Owner.Creature, healNum);
        }
        else
        {
            await CommonActions.ApplySelf<RegenPower>(this, healNum);
        }
    }
}
