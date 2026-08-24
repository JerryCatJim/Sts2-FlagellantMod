using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class BeingTowardsDeath : FlagellantCardModel
{
    public BeingTowardsDeath() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithAnimName("Punish");
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithCalculatedDamage(0, (CardModel card, Creature? creature) =>
        {
            var entry = CombatManager.Instance.History.Entries
            .OfType<DamageReceivedEntry>()
            .Where(e => e.Dealer == card.Owner.Creature
            && e.Receiver == card.Owner.Creature
            && e.Result.UnblockedDamage > 0);
            return entry?.Sum(e => e.Result.UnblockedDamage) ?? 0m;
        }
        );
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }
}
