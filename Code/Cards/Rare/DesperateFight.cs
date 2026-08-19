using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class DesperateFight : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => IsLowHealth();

    public DesperateFight() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(19, 5);
        WithHealingPercent(12, 3);
        WithStress(5);
        WithAnimName("Sepsis");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(IsLowHealth())
        {
            await CommonActions.ApplySelf<StressPower>(choiceContext, this);
        }
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        await CreatureCmd.Heal(base.Owner.Creature, GetHealingPercentHp());
    }
}
