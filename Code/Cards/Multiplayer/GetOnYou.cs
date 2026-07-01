using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Core;
using Flagellant.Code.Powers;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Multiplayer;

[Pool(typeof(FlagellantCardPool))]
public class GetOnYou : FlagellantCardModel
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public GetOnYou() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AllAllies)
    {
        WithStress(2, 1);
        WithPower<StrengthPower>(2, 2);
        WithPower<PoisonPower>(2);
        WithTip(new TooltipSource((CardModel _) => FlagellantHoverTipFactory.FromResoluteOrMeltdown<ToxicMeltdown>()));
        WithAnimName("More");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;

        await PlayCardAnim();
        List<Creature> playerCreatures = base.CombatState.PlayerCreatures.Where(c => c.IsAlive).ToList();
        foreach (Creature creature in playerCreatures)
        {
            await CommonActions.Apply<GetOnYouPower>(choiceContext, creature, this, base.DynamicVars.Strength.BaseValue);
        }
        if(FlagellantModel.IsInResoluteOrMeltdown<ToxicMeltdown>(base.Owner))
        {
            await CommonActions.Apply<PoisonPower>(choiceContext, playerCreatures, this);
        }
        await CommonActions.ApplySelf<StressPower>(choiceContext, this);
    }
}
