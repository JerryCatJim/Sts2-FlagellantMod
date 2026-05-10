using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class FearEruption : FlagellantCardModel
{
    public FearEruption() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithStress(1,1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal stressNum = base.Owner.Creature.GetPower<StressPower>()?.Amount ?? 0m;
        await CommonActions.ApplySelf<StressPower>(this);
        await DamageCmd.Attack(stressNum + base.DynamicVars["StressPower"].BaseValue).FromCard(this).TargetingAllOpponents(base.CombatState).Execute(choiceContext);
    }
}
