using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flagellant.Code.Cards.Basic;

[Pool(typeof(FlagellantCardPool))]
public class LashsGift : FlagellantCardModel
{
    public LashsGift() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithAnimName("Lash");
        WithBlock(8, 4);
        WithStress(1);
        WithPower<AddComboPower>(1);
        WithPower<ComboPower>(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<StressPower>(this);
        await CommonActions.ApplySelf<AddComboPower>(this);
    }
}
