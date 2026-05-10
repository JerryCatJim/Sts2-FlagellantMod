using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class SpreadingAnxiety : FlagellantCardModel
{
    public SpreadingAnxiety() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithAnimName("More");
        WithStress(1);
        WithCostUpgradeBy(-1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        decimal stressNum = base.Owner.Creature.GetPower<StressPower>()?.Amount ?? 0m;
        await CommonActions.ApplySelf<StressPower>(this);
        foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<SpreadingAnxietyPower>(hittableEnemy, -(stressNum + base.DynamicVars["StressPower"].BaseValue), base.Owner.Creature, this);
        }
    }
}
