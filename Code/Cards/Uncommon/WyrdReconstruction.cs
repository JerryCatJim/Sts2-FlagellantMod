using BaseLib.Utils;
using Flagellant.Code.Audio;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.DisplayHpVar;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class WyrdReconstruction : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => IsLowHealth();

    public WyrdReconstruction() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithHealingPercent(12, 3);
        WithVar("HealingPercentLow", 20, 5);
        WithVars(new HealingMaxHpVar("HealingMaxHpLow", 0));
        WithPowerTip<StableReconstructionPower>();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CombatAudioManager.PlayCombatSfx("res://Flagellant/Sounds/Occultist/sfx_hero_occ_wyrd_use.wav", true, true, -6);
        if(IsLowHealth())
        {
            await CreatureCmd.Heal(base.Owner.Creature, GetHealingPercentHp(base.DynamicVars["HealingPercentLow"].BaseValue));
        }
        else
        {
            await CreatureCmd.Heal(base.Owner.Creature, GetHealingPercentHp());
        }
    }
}
