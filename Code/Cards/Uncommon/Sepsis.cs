using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Sepsis : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyComboMarkedEnemy;
    protected override bool IsPlayable => HasAnyPoisonedEnemy;
    public Sepsis() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithAnimName("Sepsis");
        WithVar("TriggerPoison", 2, 1);
        WithPowerTip<PoisonPower>();
        WithPower<ComboPower>(1);  //要注册过这个类型的值 才能在Formatter中正确解析{ComboPower:{comboIcons()}}等类似的格式
        WithCalculatedVar("CalculatedPoisonDamage", 0,
            ((CardModel card, Creature? target) =>
            {
                if (target == null || !target.HasPower<PoisonPower>()) return 0;

                int poisonTimes = target.HasPower<ComboPower>() ? card.DynamicVars["TriggerPoison"].IntValue + 1 : card.DynamicVars["TriggerPoison"].IntValue;
                decimal damageNum = 0;
                for (int i = 0; i < poisonTimes; i++)
                {
                    damageNum += Math.Max(0, target?.GetPower<PoisonPower>()?.Amount - i ?? 0);
                }
                return damageNum;
            }
            ));
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        if (cardPlay.Target != null && cardPlay.Target.HasPower<PoisonPower>())
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGaseousImpactVfx.Create(cardPlay.Target, new Godot.Color("008000")));  //83eb85

            PoisonPower? PP = cardPlay.Target.GetPower<PoisonPower>();
            int TriggerTimes = (int)(base.DynamicVars["TriggerPoison"]?.BaseValue ?? 0) + (cardPlay.Target.HasPower<ComboPower>() ? 1 : 0);
            if (cardPlay.Target.GetPower<ComboPower>() is ComboPower comboP)
            {
                await PowerCmd.ModifyAmount(choiceContext, comboP, -1, Owner.Creature, this);
            }
            for (int i = 0; i < TriggerTimes; i++)
            {
                if (PP != null && PP.Amount > 0 && cardPlay.Target != null && cardPlay.Target.IsAlive)
                {
                    await CreatureCmd.Damage(choiceContext,
                        cardPlay.Target, PP.Amount, ValueProp.Unblockable | ValueProp.Unpowered,
                        null, this, cardPlay);
                    await PowerCmd.Decrement(PP);
                }
            }
        }
    }
}
