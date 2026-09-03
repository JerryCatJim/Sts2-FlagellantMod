using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class BeyondCure : FlagellantCardModel
{
    public BeyondCure() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithVar("SelfPoison", 3, -1);
        WithPower<PoisonPower>(12, 1);
        WithPower<WeakPower>(1, 1);
        WithAnimName("Sepsis");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await PlayCardAnim();
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGaseousImpactVfx.Create(cardPlay.Target, new Godot.Color("008000")));  //83eb85

        await CommonActions.ApplySelf<PoisonPower>(choiceContext, this, base.DynamicVars["SelfPoison"].BaseValue);
        await CommonActions.Apply<PoisonPower>(choiceContext, cardPlay.Target, this);
        await CommonActions.Apply<WeakPower>(choiceContext, cardPlay.Target, this);
    }
}
