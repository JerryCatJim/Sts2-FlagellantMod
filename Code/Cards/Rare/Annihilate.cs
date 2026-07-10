using BaseLib.Extensions;
using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class Annihilate : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => HasAnyComboMarkedEnemy;
    public Annihilate() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(13,4);
        WithVars(new DamageVar("ComboDamage",26,ValueProp.Move).WithUpgrade(8));
        WithPower<ComboPower>(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        if (cardPlay.Target.GetPower<ComboPower>() is ComboPower comboP)
        {
            await PowerCmd.ModifyAmount(choiceContext, comboP, -1, Owner.Creature, this);
            await CommonActions.CardAttack(this, cardPlay, cardPlay.Target, base.DynamicVars["ComboDamage"].BaseValue, ValueProp.Move).Execute(choiceContext);
        }
        else
        {
            await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        }
    }
}
