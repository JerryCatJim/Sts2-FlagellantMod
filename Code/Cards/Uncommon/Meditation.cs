using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Meditation : FlagellantCardModel
{
    protected override bool HasEnergyCostX => true;
    public Meditation() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPowerTip<RegenPower>();
        WithPowerTip<StressPower>();
        WithAnimName("Deathless");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();

        int num = ResolveEnergyXValue();
        int stressNum = IsUpgraded ? num + 1 : num;
        int regenNum = IsUpgraded ? num * 2 : num;
        if(regenNum > 0)
        {
            await PowerCmd.Apply<RegenPower>(choiceContext, base.Owner.Creature, regenNum, base.Owner.Creature, this);
        }
        if(stressNum > 0)
        {
            await PowerCmd.Apply<StressPower>(choiceContext, base.Owner.Creature, stressNum, base.Owner.Creature, this);
        }
    }
}
