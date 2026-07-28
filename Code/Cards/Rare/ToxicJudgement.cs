using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Core;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class ToxicJudgement : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyPoisonedEnemy || FlagellantModel.IsInResoluteOrMeltdown<ToxicMeltdown>(base.Owner);
    public ToxicJudgement() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(26, 6);
        WithLossPercent(30);
        WithPowerTip<PoisonPower>();
        WithAnimName("Sepsis");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGaseousImpactVfx.Create(cardPlay.Target, new Godot.Color("008000"))); //83eb85

        decimal poisonNum1 = Math.Round((cardPlay.Target?.GetPower<PoisonPower>()?.Amount ?? 0m) / 2m, MidpointRounding.AwayFromZero);
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        decimal poisonNum2 = Math.Round((cardPlay.Target?.GetPower<PoisonPower>()?.Amount ?? 0m) / 2m, MidpointRounding.AwayFromZero);
        if (cardPlay.Target != null && cardPlay.Target.IsAlive)
        {
            if (poisonNum2 > 0)
            {
                await PowerCmd.Apply<PoisonPower>(choiceContext, base.CombatState.HittableEnemies, poisonNum2, base.Owner.Creature, this);
            }
        }
        else
        {
            if (poisonNum1 > 0)
            {
                await PowerCmd.Apply<PoisonPower>(choiceContext, base.CombatState.HittableEnemies, poisonNum1, base.Owner.Creature, this);
            }
        }
        await CreatureCmd.Damage(choiceContext, Owner.Creature, GetLossPercentHp(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
    }
}
