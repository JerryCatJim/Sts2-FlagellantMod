using BaseLib.Extensions;
using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Lash : FlagellantCardModel
{
    public Lash() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithAnimName("Lash");
        WithLossPercent(5);
        WithPower<RegenPower>(1);
        WithVars(new RepeatVar(2).WithUpgrade(1));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        for(int i = 0; i < base.DynamicVars.Repeat.IntValue; i++)
        {
            await CreatureCmd.Damage(choiceContext, Owner.Creature, GetLossPercentHp(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
            await CommonActions.ApplySelf<RegenPower>(choiceContext, this);
        }
    }
}
