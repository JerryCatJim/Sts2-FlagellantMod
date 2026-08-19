using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Multiplayer;

[Pool(typeof(FlagellantCardPool))]
public class Redemption : FlagellantCardModel
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public Redemption() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
    {
        WithPower<RegenPower>(4,1);
        WithLossPercent(50);
        WithKeyword(CardKeyword.Exhaust);
        WithAnimName("Undying");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;

        await PlayCardAnim();
        List<Creature> playerCreatures = base.CombatState.PlayerCreatures.Where(c => c.IsAlive).ToList();
        await CommonActions.Apply<RegenPower>(choiceContext, playerCreatures, this);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, GetLossPercentHp(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
    }
}
