using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class MentalDeviation : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyComboMarkedEnemy;
    public MentalDeviation() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithDamage(6, 2);
        WithStress(5);
        WithPower<ComboPower>(1);
        WithAnimName("AcidRain");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;

        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        foreach(Creature creature in base.CombatState.HittableEnemies)
        {
            if(creature.GetPower<ComboPower>() is ComboPower comboP)
            {
                await PowerCmd.ModifyAmount(choiceContext, comboP, -1, base.Owner.Creature, this);
                await CommonActions.ApplySelf<StressPower>(choiceContext, this);
            }
        }
    }
}
