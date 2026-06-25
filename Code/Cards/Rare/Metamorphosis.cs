using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class Metamorphosis : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => IsStressGreaterEqual();
    public Metamorphosis() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithStress(5);
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithAnimName("Lash");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        if(IsStressGreaterEqual())
        {
            await CommonActions.ApplySelf<StressPower>(choiceContext, this);
        }
    }
}
