using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Cards.Token;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class Practice : FlagellantCardModel
{
    public Practice() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithAnimName("Undying");
        WithHealingPercent(10, 2);
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.FromCard<Penance>(_.IsUpgraded)));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;

        await PlayCardAnim();
        await CreatureCmd.Heal(Owner.Creature, GetHealingPercentHp());
        IEnumerable<Penance> enumerable = Penance.Create(base.Owner, 1, base.CombatState);
        if (base.IsUpgraded)
        {
            foreach (Penance item in enumerable)
            {
                CardCmd.Upgrade(item);
            }
        }
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(enumerable, PileType.Draw, base.Owner, CardPilePosition.Random));
    }
}
