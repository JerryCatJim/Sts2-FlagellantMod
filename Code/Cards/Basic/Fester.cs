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
public class Fester : FlagellantCardModel
{
    public Fester() : base(0, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithAnimName("Fester");
        WithPower<VulnerablePower>(1, 1);
        WithHealingPercent(10, 2);
        WithCards(1);
        WithPower<ComboPower>(1);  //要注册过这个类型的值 才能在Formatter中正确解析{ComboPower:{comboIcons()}}等类似的格式
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await PlaySkillAnim();
        await CommonActions.Apply<VulnerablePower>(cardPlay.Target, this);
        if(cardPlay.Target.HasPower<ComboPower>())
        {
            await PowerCmd.Remove<ComboPower>(cardPlay.Target);
            await CommonActions.Draw(this, choiceContext);
            await CreatureCmd.Heal(Owner.Creature, GetHealingPercentHp());
        }
    }
}
