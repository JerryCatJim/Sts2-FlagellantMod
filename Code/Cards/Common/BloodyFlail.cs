using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class BloodyFlail : FlagellantCardModel
{
    public BloodyFlail() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithAnimName("Lash");
        WithLossPercent(8);
        WithPower<VigorPower>(6, 2);
        WithStress(1, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<VigorPower>(choiceContext, this);
        await CommonActions.ApplySelf<StressPower>(choiceContext, this);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, GetLossPercentHp(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
    }
}
