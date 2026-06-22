using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Multiplayer;

[Pool(typeof(FlagellantCardPool))]
public class Atonement : FlagellantCardModel
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    public Atonement() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        WithPowerTip<VulnerablePower>();
        WithPowerTip<WeakPower>();
        WithPowerTip<FrailPower>();
        WithPowerTip<DoomPower>();
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithAnimName("More");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await PlayCardAnim();
        List<PowerModel> OriginalList = new List<PowerModel>()
        {
            cardPlay.Target.GetPower<VulnerablePower>(),
            cardPlay.Target.GetPower<WeakPower>(),
            cardPlay.Target.GetPower<FrailPower>(),
            cardPlay.Target.GetPower<DoomPower>()
        }
        .Where(p => p != null)
        .ToList();

        foreach (PowerModel power in OriginalList)
        {
            PowerModel myPower = (PowerModel)power.MutableClone();
            await PowerCmd.Apply(choiceContext, myPower, base.Owner.Creature, power.Amount, base.Owner.Creature, this);
        }
        foreach (PowerModel power in OriginalList)
        {
            await PowerCmd.Remove(power);
        }
    }
}
