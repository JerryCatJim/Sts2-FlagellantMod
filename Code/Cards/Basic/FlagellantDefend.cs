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
public class FlagellantDefend : FlagellantCardModel
{
    public FlagellantDefend() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
	{
		WithTags(CardTag.Defend);
		WithBlock(5, 3);
		WithPower<StressPower>(2);
		WithAnimName("Lash");
	}

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Lash", 0.0f);
        //await CommonActions.CardBlock(this, cardPlay);
        //await CommonActions.ApplySelf<StressPower>(this); //BaseLib的方法如果填入负数，就会把值存为负数，Json里就会解析出负数到卡牌界面上，所以手动修改
        await PowerCmd.Apply<StressPower>(Owner.Creature, -DynamicVars["StressPower"].BaseValue, Owner.Creature, this);
	}
}
