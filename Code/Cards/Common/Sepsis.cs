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
public class Sepsis : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyComboMarkedEnemy;
    protected override bool IsPlayable => HasAnyPoisonedEnemy;
    public Sepsis() : base(2, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithAnimName("Sepsis");
        WithVar("TriggerPoison", 2, 1);
        WithPowerTip<PoisonPower>();
        WithPower<ComboPower>(1);  //要注册过这个类型的值 才能在Formatter中正确解析{ComboPower:{comboIcons()}}等类似的格式
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Target != null && cardPlay.Target.HasPower<PoisonPower>())
        {
            await PlayCardAnim();
            PoisonPower? PP = cardPlay.Target.GetPower<PoisonPower>();
            int TriggerTimes = (int)(base.DynamicVars["TriggerPoison"]?.BaseValue ?? 0) + (cardPlay.Target.HasPower<ComboPower>() ? 1 : 0);
            if(cardPlay.Target.GetPower<ComboPower>() is ComboPower comboP)
            {
                await PowerCmd.ModifyAmount(comboP, -1, Owner.Creature, this);
            }
            for(int i = 0; i < TriggerTimes; i++)
            {
                if (PP != null && PP.Amount > 0)
                {
                    await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), 
                        cardPlay.Target, PP.Amount, ValueProp.Unblockable | ValueProp.Unpowered, 
                        null, this);
                    await PowerCmd.Decrement(PP);
                }
            }
        }
    }
}
