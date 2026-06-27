using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class Unyielding : FlagellantCardModel
{
    public Unyielding() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<UnyieldingPower>(1,1);
        WithPowerTip<DoomPower>();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<UnyieldingPower>(choiceContext, this);
    }
}
