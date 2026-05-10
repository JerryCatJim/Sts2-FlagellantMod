using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class SpreadingPlague : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => !HasAnyPoisonedEnemy;
    public SpreadingPlague() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(10,3);
        WithPowerTip<PoisonPower>();
        WithAnimName("Punish");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        if(cardPlay.Target.HasPower<PoisonPower>())
        {
            await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        }
        else
        {
            await PlayCardAnim();
            await CommonActions.Apply<PoisonPower>(cardPlay.Target, this, base.DynamicVars.Damage.BaseValue);
        }
    }
}
