using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.ValueProps;
using Flagellant.Code.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;

namespace Flagellant.Code.Cards.Basic;

[Pool(typeof(FlagellantCardPool))]
public class FlagellantRelly : FlagellantCardModel
{
    public FlagellantRelly() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
	{
		WithPower<RelicPower>(-5, 2);
		WithPower<StrengthPower>(5);
	}


	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		if((Owner.Creature.GetPower<RelicPower>()?.Amount ?? 0) >= 5m)
		{
			await CommonActions.ApplySelf<RelicPower>(this);
			await CommonActions.ApplySelf<StrengthPower>(this);
        }
	}
}
