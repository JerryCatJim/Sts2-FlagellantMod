using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class TransferMisfortune : FlagellantCardModel
{

    public TransferMisfortune() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPowerTip<DoomPower>();
        WithCards(1);
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal doomNum = base.Owner.Creature.GetPower<DoomPower>()?.Amount ?? 0m;
        if(doomNum > 0)
        {
            await PowerCmd.Remove<DoomPower>(base.Owner.Creature);
            await PowerCmd.Apply<DoomPower>(base.CombatState.HittableEnemies, doomNum, base.Owner.Creature, this);
        }
        await CommonActions.Draw(this, choiceContext);
    }
}
