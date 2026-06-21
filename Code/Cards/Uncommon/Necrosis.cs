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
public class Necrosis : FlagellantCardModel
{
    public Necrosis() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithAnimName("Necrosis");
        WithPower<PoisonPower>(4);
        WithPower<RegenPower>(4);
        WithPower<ComboPower>(1);
        WithCostUpgradeBy(-1);
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await PlayCardAnim();
        await CommonActions.Apply<PoisonPower>(choiceContext, cardPlay.Target, this);
        if(cardPlay.Target.GetPower<ComboPower>() is ComboPower comboP)
        {
            await PowerCmd.ModifyAmount(choiceContext, comboP, -1, Owner.Creature, this);
            await CommonActions.ApplySelf<RegenPower>(choiceContext, this);
        }
    }
}
