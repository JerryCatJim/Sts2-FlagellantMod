using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Code.Powers;

public class UndyingPower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _usedThisTurn;
    public bool UsedThisTurn
    {
        get
        {
            return _usedThisTurn;
        }
        private set
        {
            if (_usedThisTurn != value)
            {
                AssertMutable();
                _usedThisTurn = value;
            }
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (CombatManager.Instance.IsInProgress && target == base.Owner 
            && result.UnblockedDamage > 0 && !UsedThisTurn && base.CombatState.CurrentSide == base.Owner.Side
            && base.Owner != null && base.Owner.Player != null)
        {
            Flash();
            UsedThisTurn = true;
            await CardPileCmd.Draw(choiceContext, Amount, base.Owner.Player);
        }
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != base.Owner.Side) return Task.CompletedTask;

        UsedThisTurn = false;
        return Task.CompletedTask;
    }
}
