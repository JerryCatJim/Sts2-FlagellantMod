using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Hooks;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class SpreadingAnxiety : FlagellantCardModel
{
    public SpreadingAnxiety() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithAnimName("Endure");
        WithStress(2);
        WithCostUpgradeBy(-1);
        WithKeyword(CardKeyword.Exhaust);
        WithPowerTip<StrengthPower>();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;

        await PlayCardAnim();
        decimal stressNum = DD2Hooks.ModifyStressPower(Owner.Creature.GetPower<StressPower>(), DynamicVars["StressPower"].BaseValue, Owner.Creature, Owner.Creature, this);
        await CommonActions.ApplySelf<StressPower>(choiceContext, this);
        foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<SpreadingAnxietyPower>(choiceContext, hittableEnemy, stressNum, base.Owner.Creature, this);
        }
    }
}
