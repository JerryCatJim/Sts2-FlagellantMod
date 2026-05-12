using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Martyrdom : FlagellantCardModel
{
    protected override bool IsPlayable => base.Owner.Creature.HasPower<DoomPower>();
    public Martyrdom() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithPowerTip<DoomPower>();
        WithPower<ComboPower>(1);
        WithAnimName("Lash");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal doomNum = base.Owner.Creature.GetPower<DoomPower>()?.Amount ?? 0m;
        if(doomNum > 0m)
        {
            await CreatureCmd.Damage(choiceContext, Owner.Creature, doomNum, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
            decimal newDoomNum = base.Owner.Creature.GetPower<DoomPower>()?.Amount ?? doomNum;
            await DamageCmd.Attack(newDoomNum).FromCard(this).TargetingAllOpponents(base.CombatState).Execute(choiceContext);
            await PowerCmd.Apply<ComboPower>(base.CombatState.HittableEnemies, base.DynamicVars["ComboPower"].BaseValue, base.Owner.Creature, this);
            if (IsUpgraded)
            {
                await PowerCmd.Remove<DoomPower>(base.Owner.Creature);
            }
        }
    }
}