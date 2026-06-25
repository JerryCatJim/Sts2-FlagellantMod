using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class LunaticStrike : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => IsLowHealth();
    public LunaticStrike() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithDamage(8,2);
        WithStress(2,1);
        WithVar("StressPowerLow", 5);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        
        decimal stressNum = IsLowHealth() ? (base.DynamicVars["StressPowerLow"]?.BaseValue ?? 0) : (base.DynamicVars["StressPower"]?.BaseValue ?? 0);
        await CommonActions.ApplySelf<StressPower>(choiceContext, this, stressNum);
    }
}
