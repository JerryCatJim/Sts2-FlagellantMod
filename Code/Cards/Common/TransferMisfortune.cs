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
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.HasPower<DoomPower>();
    public TransferMisfortune() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<DoomPower>(9);
        WithCards(2);
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;

        if (Owner.Creature.GetPower<DoomPower>() is DoomPower doomP)
        {
            await PowerCmd.ModifyAmount(choiceContext, doomP, -base.DynamicVars.Doom.BaseValue, Owner.Creature, this);
        }
        await PowerCmd.Apply<DoomPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Doom.BaseValue, base.Owner.Creature, this);
        await CommonActions.Draw(this, choiceContext);
    }
}
