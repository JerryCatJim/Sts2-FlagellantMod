using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.DisplayHpVar;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Pilgrimage : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => LostHpThisTurn(base.Owner.Creature);

    public Pilgrimage() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithAnimName("Lash");
        WithCards(2);
        WithVar("ExtraDraw", 1);
        WithLostHpThisTurnDisplay();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        if(LostHpThisTurn(base.Owner.Creature))
        {
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue + base.DynamicVars["ExtraDraw"].BaseValue, base.Owner);
        }
        else
        {
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        }
        if(IsUpgraded && LostHpThisTurn(base.Owner.Creature))
        {
            await CreatureCmd.Heal(base.Owner.Creature, LostHpThisTurnNum(base.Owner.Creature));
        }
    }

    private static decimal LostHpThisTurnNum(Creature creature)
    {
        var entry = CombatManager.Instance.History.Entries
        .OfType<DamageReceivedEntry>()
        .Where(e => e.HappenedThisTurn(creature.CombatState)
            && e.Receiver == creature
            && e.Result.UnblockedDamage > 0);
        return entry?.Sum(e => e.Result.UnblockedDamage) ?? 0m;
    }
    private static bool LostHpThisTurn(Creature creature)
    {
        return CombatManager.Instance.History.Entries.
            OfType<DamageReceivedEntry>().
            Any((DamageReceivedEntry e) => e.HappenedThisTurn(creature.CombatState)
                && e.Receiver == creature 
                && e.Result.UnblockedDamage > 0);
    }
}
