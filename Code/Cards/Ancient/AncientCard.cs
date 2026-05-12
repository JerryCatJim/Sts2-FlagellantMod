using BaseLib.Utils;
using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;

namespace Flagellant.Code.Cards.Ancient;

[Pool(typeof(FlagellantCardPool))]
public class Execution : FlagellantCardModel
{
    public Execution() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithDamage(12,4);
        WithPoison(6,2);
        WithHealingPercent(10);
        WithAnimName("Punish");
        WithPower<ComboPower>(1);  //要注册过这个类型的值 才能在Formatter中正确解析{ComboPower:{comboIcons()}}等类似的格式
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        await CommonActions.Apply<PoisonPower>(cardPlay.Target, this);
        await CreatureCmd.Heal(Owner.Creature, GetLossPercentHp());
        if (IsUpgraded)
        {
            await CommonActions.Apply<ComboPower>(cardPlay.Target, this);
        }
    }
}
