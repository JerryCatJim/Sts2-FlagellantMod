using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Hooks;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Nervous : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.HasPower<DoomPower>();

    private decimal _calculatedStress = 0;
    public Nervous() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithStress(2);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithCalculatedVar("CalculatedStress", 0, ((CardModel card, Creature? c) =>
        {
            if (card != null && card is Nervous myCard)
            {
                if (myCard._calculatedStress != 0)
                {
                    return myCard._calculatedStress;
                }
                else
                {
                    decimal delta = 0m;
                    if (myCard.DynamicVars.TryGetValue("StressPower", out var dynamicVar))
                    {
                        delta = dynamicVar.BaseValue;
                    }
                    return DD2Hooks.ModifyStressPower(myCard.Owner.Creature.CombatState ,myCard.Owner.Creature.GetPower<StressPower>(), delta, myCard.Owner.Creature, myCard.Owner.Creature, myCard);
                }
            }
            return 0;
        }
        ));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        _calculatedStress = DD2Hooks.ModifyStressPower(Owner.Creature.CombatState, Owner.Creature.GetPower<StressPower>(), DynamicVars["StressPower"].BaseValue, Owner.Creature, Owner.Creature, this);
        await CommonActions.ApplySelf<StressPower>(choiceContext, this);
        if (Owner.Creature.GetPower<DoomPower>() is DoomPower doomP)
        {
            await PowerCmd.ModifyAmount(choiceContext, doomP, -_calculatedStress, Owner.Creature, this);
        }
        _calculatedStress = 0;
    }
}
