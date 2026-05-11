using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Basic;

[Pool(typeof(FlagellantCardPool))]
public class Fester : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyComboMarkedEnemy;
    public Fester() : base(0, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithAnimName("Fester");
        WithPower<VulnerablePower>(1, 1);
        WithHealingPercent(8, 2);
        WithCards(1);
        WithPower<ComboPower>(1);  //要注册过这个类型的值 才能在Formatter中正确解析{ComboPower:{comboIcons()}}等类似的格式
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await PlayCardAnim();
        await CommonActions.Apply<VulnerablePower>(cardPlay.Target, this);
        if(cardPlay.Target.GetPower<ComboPower>() is ComboPower comboP)
        {
            //DO NOT use Decrement or Remove, Need receive the applier to trigger ComboPower amount decreased Event by "AfterComboChanged"(IAfterComboChanged Interface).
            await PowerCmd.ModifyAmount(comboP, -1, Owner.Creature, this);
            await CommonActions.Draw(this, choiceContext);
            await CreatureCmd.Heal(Owner.Creature, GetHealingPercentHp());
        }
    }
}
