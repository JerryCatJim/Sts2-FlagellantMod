using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Necrosis : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyPoisonedEnemy;
    public Necrosis() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithAnimName("Necrosis");
        WithDamage(6,3);
        WithPower<RegenPower>(2);
        WithPowerTip<PoisonPower>();
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;

        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        foreach (Creature creature in base.CombatState.HittableEnemies)
        {
            if (creature.HasPower<PoisonPower>())
            {
                await CommonActions.ApplySelf<RegenPower>(choiceContext, this);
            }
        }
    }
}
