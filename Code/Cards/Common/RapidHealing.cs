using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class RapidHealing : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => IsLowHealth();
    public RapidHealing() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithHealingPercent(5, 3);
        WithBlock(3, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsLowHealth())
        {
            await CommonActions.CardBlock(this, cardPlay);
        }
        await CreatureCmd.Heal(base.Owner.Creature, GetHealingPercentHp());
    }
    private bool IsLowHealth(decimal Percent = 30m)
    {
        if (base.Owner.Creature == null) return false;

        return base.Owner.Creature.CurrentHp / base.Owner.Creature.MaxHp * 100m <= Percent;
    }
}
