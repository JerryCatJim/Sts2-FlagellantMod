using BaseLib.Utils;
using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Flagellant.Code.Cards.Uncommon;

namespace Flagellant.Code.Powers;

public sealed class EndurePower : FlagellantPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StressPower>()
    ];

    #region GainStressWhenHpLoss
    /*private class Data
    {
        public readonly Dictionary<CardModel, int> playedCards = new Dictionary<CardModel, int>();
    }

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != base.Owner)
        {
            return Task.CompletedTask;
        }
        if (base.CombatState.CurrentSide != base.Owner.Side)
        {
            return Task.CompletedTask;
        }
        GetInternalData<Data>().playedCards.Add(cardPlay.Card, 0);
        return Task.CompletedTask;
    }*/

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (CombatManager.Instance.IsInProgress && target == base.Owner 
            && result.UnblockedDamage > 0 && base.CombatState.CurrentSide == base.Owner.Side)
        {
            /*if (cardSource == null || !GetInternalData<Data>().playedCards.ContainsKey(cardSource))
            {
                await PowerCmd.Apply<StressPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
            }
            else
            {
                GetInternalData<Data>().playedCards[cardSource] += base.Amount;
            }*/
            await PowerCmd.Apply<StressPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
        }
    }

    /*public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == base.Owner && GetInternalData<Data>().playedCards.Remove(cardPlay.Card, out var value))
        {
            await CommonActions.ApplySelf<StressPower>(choiceContext, cardPlay.Card, value);
        }
    }*/
    #endregion GainStressWhenHpLoss

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;
        
        await PowerCmd.Apply<StressPower>(choiceContext, player.Creature, Amount, player.Creature, ModelDb.Card<Endure>());
        Flash();
    }
}
