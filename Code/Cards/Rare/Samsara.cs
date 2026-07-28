using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class Samsara : FlagellantCardModel
{
    public Samsara() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithPowerTip<SamsaraPower>();
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithAnimName("More");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();

        decimal LostHp = base.Owner.Creature.MaxHp - base.Owner.Creature.CurrentHp;
        decimal ChangedHp = LostHp - base.Owner.Creature.CurrentHp;
        await CreatureCmd.SetCurrentHp(base.Owner.Creature, LostHp);
        await PowerCmd.Apply<SamsaraPower>(choiceContext, base.Owner.Creature, -ChangedHp, base.Owner.Creature, this);
    }
}
