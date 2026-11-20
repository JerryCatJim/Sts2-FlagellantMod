using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Cards.Token;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class Martyrdom : FlagellantCardModel
{
    public Martyrdom() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPowerTip<DoomPower>();
        WithAnimName("Lash");
        WithKeyword(CardKeyword.Exhaust);
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.FromCard<Penance>()));
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NGroundFireVfx.Create(base.Owner.Creature));
        IEnumerable<CardModel> enumerable = PileType.Exhaust.GetPile(base.Owner).Cards.Where((CardModel c) => c is Penance).ToList();
        foreach (CardModel item in enumerable)
        {
            await CardCmd.AutoPlay(choiceContext, item, cardPlay.Target, AutoPlayType.Default, skipXCapture: false, skipCardPileVisuals : false);
        }
    }
}