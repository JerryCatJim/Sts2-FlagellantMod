using BaseLib.Abstracts;
using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Cards.Ancient;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using Flagellant.Code.ResoluteOrMeltdown;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Flagellant.Code.Cards.Basic;

[Pool(typeof(FlagellantCardPool))]
public class AcidRain : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyComboEnemy;
    public AcidRain() : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
        WithDamage(4);
        WithPoison(4);
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
            if (hittableEnemy.IsAlive && hittableEnemy.HasPower<ComboPower>())
            {
                damage += base.DynamicVars["ComboUpgraded"].BaseValue;
                poison += base.DynamicVars["ComboUpgraded"].BaseValue;
                await PowerCmd.Remove<ComboPower>(hittableEnemy);
            }
            //await CommonActions.CardAttack(this, hittableEnemy, damage).Execute(choiceContext);
            DamageCmd.Attack(damage).FromCard(this).Targeting(hittableEnemy).Execute(choiceContext);
            if (hittableEnemy != null && hittableEnemy.IsAlive)
            {
                CommonActions.Apply<PoisonPower>(hittableEnemy, this, poison);
            }
        }
    }
}
