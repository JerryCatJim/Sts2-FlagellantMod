using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class Scourge : FlagellantCardModel
{
    public Scourge() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithAnimName("Deathless");
        WithPowerTip<DoomPower>();
        WithPowerTip<PoisonPower>();
        WithKeyword(CardKeyword.Exhaust, UpgradeType.None);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;

        await PlayCardAnim();
        decimal doomNum = Owner.Creature.GetPower<DoomPower>()?.Amount ?? 0m;
        if (doomNum > 0)
        {
            await CreatureCmd.Heal(Owner.Creature, doomNum);
            await PowerCmd.Apply<PoisonPower>(choiceContext, base.CombatState.HittableEnemies, doomNum, base.Owner.Creature, this);
        }
    }
}
