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

namespace Flagellant.Code.Potions;

[Pool(typeof(FlagellantPotionPool))]
public class ScourgePotion : FlagellantPotionModel
{
    // The base amount of Miracles to add
    public override PotionRarity Rarity => PotionRarity.Rare;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;


    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StressPower>()
    ];

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        if (target?.Player == null) return;
        if (target != null)
        {
            await PowerCmd.Apply<StressPower>(ctx, target, 10, Owner.Creature, null);
            decimal num = Math.Round(target.MaxHp * 45m / 100m, MidpointRounding.AwayFromZero);
            await CreatureCmd.SetCurrentHp(target, num < 1m ? 1m : num);
        }
    }
}