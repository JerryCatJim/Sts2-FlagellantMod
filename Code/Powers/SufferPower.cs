using BaseLib.Utils;
using Flagellant.Audio;
using Flagellant.Code.Abstract;
using Flagellant.Code.Commands;
using Flagellant.Code.Core;
using Flagellant.Code.ResoluteOrMeltdown;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flagellant.Code.Cards.Uncommon;
using Flagellant.Code.Cards.Rare;

namespace Flagellant.Code.Powers;

public sealed class SufferPower : FlagellantPowerModel, IAfterStressChanged
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DoomPower>(),
        HoverTipFactory.FromPower<StressPower>()
    ];

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (base.CombatState.CurrentSide != base.Owner.Side || delta <= 0m || creature == null || creature != Owner) return;
        DoomPower? DPwr = creature.GetPower<DoomPower>();
        if (DPwr != null)
        {
            await PowerCmd.ModifyAmount(DPwr, -Math.Round(delta), creature, ModelDb.Card<Suffer>());
        }
    }
    public async Task AfterStressAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (base.CombatState.CurrentSide != base.Owner.Side) return;
        await CreatureCmd.Heal(base.Owner, GetHealingPercentHp(Amount));
    }
}
