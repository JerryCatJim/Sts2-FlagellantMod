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
using System.Collections.Generic;
using System.Threading.Tasks;
using Flagellant.Cards;
using Flagellant.Powers;
using Flagellant.Abstract;

namespace Flagellant.Cards;

public class FlagellantPowerUp() : FlagellantCard(1,
	CardType.Power, CardRarity.Common,
	TargetType.Self)
{
	protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("RellyPower",5m)];


	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await PowerCmd.Apply<RelicPower>(Owner.Creature, DynamicVars["RellyPower"].BaseValue, Owner.Creature, this);
	}

	protected override void OnUpgrade()
	{
		DynamicVars["RellyPower"].UpgradeValueBy(2m);
	}
}
