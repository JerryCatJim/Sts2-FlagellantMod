using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class AbsorbPain : FlagellantCardModel
{
    public AbsorbPain() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPowerTip<StressPower>();
        WithPowerTip<RegenPower>();
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal stressNum = base.Owner.Creature.GetPower<StressPower>()?.Amount ?? 0m;
        if(stressNum > 0)
        {
            await CommonActions.ApplySelf<RegenPower>(choiceContext, this, stressNum);
        }
    }
}
