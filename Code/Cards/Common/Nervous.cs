using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class Nervous : FlagellantCardModel
{
    public Nervous() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<StressPower>(2);
        WithBlock(1); //Just for displaying keyword.
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal stressNum = base.Owner.Creature.GetPower<StressPower>()?.Amount ?? 0m;
        await CommonActions.ApplySelf<StressPower>(this);
        await CreatureCmd.GainBlock(base.Owner.Creature, stressNum + GetStressBeforeReceived(), ValueProp.Move, cardPlay);
    }
}
