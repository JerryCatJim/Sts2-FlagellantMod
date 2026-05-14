using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class BoneMeltingPalm : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => base.CombatState?.HittableEnemies.Any((Creature e) => e.HasPower<DoomPower>()) ?? false;
    public BoneMeltingPalm() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(10, 3);
        WithPowerTip<DoomPower>();
        WithPowerTip<PoisonPower>();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        if(cardPlay.Target != null && cardPlay.Target.GetPower<DoomPower>() is DoomPower doomP)
        {
            decimal doomNum = doomP.Amount;
            await PowerCmd.Remove(doomP);
            await PowerCmd.Apply<PoisonPower>(cardPlay.Target, doomNum, base.Owner.Creature, this);
        }
    }
}
