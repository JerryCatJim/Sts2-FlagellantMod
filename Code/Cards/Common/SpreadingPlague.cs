using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class SpreadingPlague : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => base.CombatState?.HittableEnemies.Any((Creature e) => !e.HasPower<PoisonPower>()) ?? false;
    public SpreadingPlague() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(9,3);
        WithPower<PoisonPower>(9,3);
        WithAnimName("Punish");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        if (cardPlay.Target.HasPower<PoisonPower>())
        {
            await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        }
        else
        {
            await PlayCardAnim();
            await CommonActions.Apply<PoisonPower>(choiceContext, cardPlay.Target, this);
        }
    }
}
