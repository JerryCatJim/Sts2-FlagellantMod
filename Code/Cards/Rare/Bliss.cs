using BaseLib.Utils;
using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class Bliss : FlagellantCardModel
{
    public Bliss() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPowerTip<StressPower>();
        WithPowerTip<RegenPower>();
        WithPower<BlissPower>(1);
        WithDamage(7, 3);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        await CommonActions.ApplySelf<BlissPower>(choiceContext, this);
    }
}
