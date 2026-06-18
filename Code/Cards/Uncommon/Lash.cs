using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Lash : FlagellantCardModel
{
    public Lash() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithAnimName("Lash");
        WithLossPercent(8, -2);
        WithHealingPercent(12, 3);
        WithStress(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CreatureCmd.Damage(choiceContext, Owner.Creature, GetLossPercentHp(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        await CreatureCmd.Heal(base.Owner.Creature, GetHealingPercentHp());
        await CommonActions.ApplySelf<StressPower>(choiceContext, this);
    }
}
