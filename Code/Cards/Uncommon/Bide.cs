using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Bide : FlagellantCardModel
{
    protected override bool HasEnergyCostX => true;
    public Bide() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithAnimName("More");
        WithLossPercent(8);
        WithPower<DoomPower>(9);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (base.CombatState == null) return;

        await PlayCardAnim();
        int num = base.IsUpgraded ? ResolveEnergyXValue() + 1 : ResolveEnergyXValue();
        for (int i = 0; i < num; i++)
        {
            await CreatureCmd.Damage(choiceContext, Owner.Creature, GetLossPercentHp(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
            if (Owner.Creature.GetPower<DoomPower>() is DoomPower doomP)
            {
                await PowerCmd.ModifyAmount(choiceContext, doomP, -base.DynamicVars.Doom.BaseValue, Owner.Creature, this);
            }
        }
    }
}
