using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class SelfDestruction : FlagellantCardModel
{
    public SelfDestruction() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<DoomPower>(9, 4);
        WithCards(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DoomPower>(choiceContext, this);
        await PowerCmd.Apply<DoomPower>(choiceContext, base.CombatState.HittableEnemies, base.DynamicVars.Doom.BaseValue, base.Owner.Creature, this);
        await CommonActions.Draw(this, choiceContext);
    }
}
