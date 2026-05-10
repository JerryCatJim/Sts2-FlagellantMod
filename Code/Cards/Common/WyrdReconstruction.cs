using BaseLib.Utils;
using Flagellant.Audio;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.DisplayHpVar;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class WyrdReconstruction : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => IsLowHealth();

    public WyrdReconstruction() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithHealingPercent(15, 3);
        WithVar("HealingPercentLow", 25, 5);
        WithVars(new HealingMaxHpVar("HealingMaxHpLow", 0));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        AudioManager.PlayCombatSfx("res://Flagellant/Sounds/Occultist/sfx_hero_occ_wyrd_use.wav", true, true, -6);
        if(IsLowHealth())
        {
            await CreatureCmd.Heal(base.Owner.Creature, GetHealingPercentHp(base.DynamicVars["HealingPercentLow"].BaseValue));
        }
        else
        {
            await CreatureCmd.Heal(base.Owner.Creature, GetHealingPercentHp());
        }
    }

    private bool IsLowHealth(decimal Percent = 30m)
    {
        if (base.Owner.Creature == null) return false;

        return base.Owner.Creature.CurrentHp/base.Owner.Creature.MaxHp * 100m <= Percent;
    }
}
