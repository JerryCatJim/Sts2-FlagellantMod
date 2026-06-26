using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Pilgrimage : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => LostHpThisTurn(base.Owner.Creature);

    public Pilgrimage() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithAnimName("Lash");
        WithCards(2);
        WithVar("ExtraDraw", 1, 1);
        WithLostHpThisTurnDisplay();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        decimal drawNum = LostHpThisTurn(base.Owner.Creature) ? 
            base.DynamicVars.Cards.BaseValue + base.DynamicVars["ExtraDraw"].BaseValue : 
            base.DynamicVars.Cards.BaseValue;
        await CardPileCmd.Draw(choiceContext, drawNum, base.Owner);
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
