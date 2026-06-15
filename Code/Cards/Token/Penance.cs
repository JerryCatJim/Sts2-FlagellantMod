using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Penance : FlagellantCardModel
{
    public Penance() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithLossPercent(8, -3);
        WithStress(1, 1);
        WithCards(1);
        WithKeyword(CardKeyword.Exhaust);
        WithAnimName("Lash");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.ApplySelf<StressPower>(this);
        await CommonActions.Draw(this, choiceContext);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, GetLossPercentHp(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
    }
}
