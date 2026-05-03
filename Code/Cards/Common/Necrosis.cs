using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class Necrosis : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => base.CombatState?.HittableEnemies.Any((Creature e) => e.HasPower<PoisonPower>() && e.HasPower<ComboPower>()) ?? false;
    protected override bool IsPlayable => base.CombatState?.HittableEnemies.Any((Creature e) => e.HasPower<PoisonPower>() && e.HasPower<ComboPower>()) ?? false;
    public Necrosis() : base(2, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithAnimName("Necrosis");
        WithPower<ComboPower>(1);
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null && cardPlay.Target.HasPower<PoisonPower>() && cardPlay.Target.HasPower<ComboPower>())
        {
            await PlayCardAnim();
            PoisonPower? PP = cardPlay.Target.GetPower<PoisonPower>();
            if (PP != null && PP.Amount > 0)
            {
                await PowerCmd.Remove<ComboPower>(cardPlay.Target);
                await CreatureCmd.GainBlock(base.Owner.Creature, PP.Amount, ValueProp.Move, cardPlay);
            }
        }
    }
}
