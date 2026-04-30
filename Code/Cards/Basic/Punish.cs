using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.ResoluteOrMeltdown;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
using Flagellant.Code.Character;
using Flagellant.Code.Powers;

namespace Flagellant.Code.Cards.Basic;

[Pool(typeof(FlagellantCardPool))]
public class Punish : FlagellantCardModel
{
    public Punish() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(12, 4);
        WithPoison(4,2);
        WithLossPercent(10,-2);
        WithAnimName("Punish");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        await CommonActions.Apply<PoisonPower>(cardPlay.Target, this);
        await CreatureCmd.Damage(choiceContext, base.Owner.Creature, GetLossPercentDamage(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
    }
}
