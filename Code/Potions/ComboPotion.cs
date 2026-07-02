using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Potions;

[Pool(typeof(FlagellantPotionPool))]
public class ComboPotion : FlagellantPotionModel
{
    // The base amount of Miracles to add
    public override PotionRarity Rarity => PotionRarity.Common;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyEnemy;


    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ComboPower>(),
        HoverTipFactory.FromPower<VulnerablePower>()
    ];

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        if (target != null)
        {
            await PowerCmd.Apply<ComboPower>(ctx, target, 1, Owner.Creature, null);
            await PowerCmd.Apply<VulnerablePower>(ctx, target, 1, Owner.Creature, null);
        }
    }
}