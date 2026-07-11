using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class AcidRain : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyComboMarkedEnemy;

    private readonly Dictionary<Creature, bool> MarkedEnemies = new Dictionary<Creature, bool>();
    public AcidRain() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(4,1);
        WithPoison(4,1);
        WithLossPercent(8);
        WithAnimName("AcidRain");
        WithVar("ComboUpgraded", 2, 1);
        WithPower<ComboPower>(1);  //要注册过这个类型的值 才能在Formatter中正确解析{ComboPower:{comboIcons()}}等类似的格式
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;

        MarkedEnemies.Clear();
        foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
        {
            MarkedEnemies.Add(hittableEnemy, hittableEnemy.HasPower<ComboPower>());
            if (hittableEnemy.IsAlive && hittableEnemy.GetPower<ComboPower>() is ComboPower comboP)
            {
                await PowerCmd.ModifyAmount(choiceContext, comboP, -1, Owner.Creature, this);
            }
        }
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        foreach(KeyValuePair<Creature, bool> pairs in MarkedEnemies)
        {
            if(pairs.Key != null && pairs.Key.IsAlive)
            {
                decimal poison = base.DynamicVars["PoisonPower"].BaseValue;
                poison += pairs.Value ? base.DynamicVars["ComboUpgraded"].BaseValue : 0;
                await CommonActions.Apply<PoisonPower>(choiceContext, pairs.Key, this, poison);
            }
        }
        await CreatureCmd.Damage(choiceContext, Owner.Creature, GetLossPercentHp(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource != this) return 0m;

        if(target != null && MarkedEnemies.ContainsKey(target))
        {
            if (MarkedEnemies[target] == true)
            {
                return base.DynamicVars["ComboUpgraded"].BaseValue;
            }
        }
        return 0m;
    }
}
