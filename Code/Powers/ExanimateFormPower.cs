using Flagellant.Code.Abstract;
using Flagellant.Code.Cards.Token;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace Flagellant.Code.Powers;

public sealed class ExanimateFormPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Penance>()
    ];
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner) && base.Owner.Player != null)  //if (side == base.Owner.Side)
        {
            IEnumerable<Penance> enumerable = Penance.Create(base.Owner.Player, Amount, base.CombatState);
            await CardPileCmd.AddGeneratedCardsToCombat(enumerable, PileType.Hand, base.Owner.Player);
        }
    }
}
