using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class DeathComes : FlagellantCardModel
{
    public DeathComes() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPowerTip<DoomPower>();
        WithStress(10);
        WithKeyword(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DoomPower>(base.Owner.Creature, base.Owner.Creature.CurrentHp, base.Owner.Creature, this);
        await CommonActions.ApplySelf<StressPower>(this);
    }
}
