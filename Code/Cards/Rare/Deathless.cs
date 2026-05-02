using BaseLib.Utils;
using Flagellant.Code.Abstract;
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
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Flagellant.Code.Cards.Basic;

[Pool(typeof(FlagellantCardPool))]
public class Deathless : FlagellantCardModel
{
    public Deathless() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithAnimName("Endure");
        WithHealingPercent(35, 5);
        WithPowerTip<DoomPower>();
        WithKeyword(CardKeyword.Exhaust, UpgradeType.None);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        await CreatureCmd.Heal(Owner.Creature, GetHealingPercentHp());
        await CommonActions.ApplySelf<DoomPower>(this, GetHealingPercentHp());
    }
}
