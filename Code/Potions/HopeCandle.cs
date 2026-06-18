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
public class HopeCandle : FlagellantPotionModel
{
    // The base amount of Miracles to add
    public override PotionRarity Rarity => PotionRarity.Uncommon;
    public override PotionUsage Usage => PotionUsage.CombatOnly;
    public override TargetType TargetType => TargetType.AnyPlayer;


    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StressPower>(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        if (target?.Player == null) return;
        if(target != null &&  target.HasPower<StressPower>())
        {
            StressPower? SP = target.GetPower<StressPower>();
            if (SP != null && SP.Amount > 0)
            {
                await PowerCmd.Apply<StrengthPower>(ctx, target, SP.Amount, Owner.Creature, null);
            }
        }
    }
}