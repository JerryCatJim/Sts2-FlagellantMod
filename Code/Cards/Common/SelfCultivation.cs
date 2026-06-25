using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class SelfCultivation : FlagellantCardModel
{
    public SelfCultivation() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPowerTip<RegenPower>();
        WithStress(2, 1);
        WithAnimName("Lash");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<StressPower>(choiceContext, this);
        if (base.Owner.Creature.GetPower<RegenPower>() is RegenPower regenPower)
        {
            await CreatureCmd.Heal(base.Owner.Creature, regenPower.Amount);
            await PowerCmd.Decrement(regenPower);
        }
    }
}
