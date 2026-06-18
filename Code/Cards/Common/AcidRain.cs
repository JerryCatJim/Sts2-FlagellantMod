using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class AcidRain : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyComboMarkedEnemy;
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

        foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
        {
            decimal damage = base.DynamicVars.Damage.BaseValue;
            decimal poison = base.DynamicVars["PoisonPower"].BaseValue;
            if (hittableEnemy.IsAlive && hittableEnemy.GetPower<ComboPower>() is ComboPower comboP)
            {
                damage += base.DynamicVars["ComboUpgraded"].BaseValue;
                poison += base.DynamicVars["ComboUpgraded"].BaseValue;
                await PowerCmd.ModifyAmount(choiceContext, comboP, -1, Owner.Creature, this);
            }
            await DamageCmd.Attack(damage).FromCard(this).Targeting(hittableEnemy).Execute(choiceContext);
            if (hittableEnemy != null && hittableEnemy.IsAlive)
            {
                await CommonActions.Apply<PoisonPower>(choiceContext, hittableEnemy, this, poison);
            }
        }
        await CreatureCmd.Damage(choiceContext, Owner.Creature, GetLossPercentHp(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
    }
}
