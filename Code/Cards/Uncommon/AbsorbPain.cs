using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class AbsorbPain : FlagellantCardModel
{
    public AbsorbPain() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPowerTip<StressPower>();
        WithPowerTip<RegenPower>();
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
        WithAnimName("Deathless");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        decimal stressNum = base.Owner.Creature.GetPower<StressPower>()?.Amount ?? 0m;
        if(stressNum > 0)
        {
            await CommonActions.ApplySelf<RegenPower>(choiceContext, this, stressNum);
        }
    }
}
