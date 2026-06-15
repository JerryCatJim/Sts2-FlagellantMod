using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class BeyondCure : FlagellantCardModel
{
    public BeyondCure() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithVar("SelfPoison", 3, 1);
        WithPower<PoisonPower>(9, 1);
        WithPower<WeakPower>(1, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<PoisonPower>(this, base.DynamicVars["SelfPoison"].BaseValue);
        await CommonActions.Apply<PoisonPower>(cardPlay.Target, this);
        await CommonActions.Apply<WeakPower>(cardPlay.Target, this);
    }
}
