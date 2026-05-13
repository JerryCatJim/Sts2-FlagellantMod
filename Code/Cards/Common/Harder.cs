using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class Harder : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => base.Owner.Creature.HasPower<DoomPower>();
    public Harder() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithHealingPercent(10, 2);
        WithPowerTip<DoomPower>();
        WithCards(1);
        WithAnimName("Lash");
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        decimal healNum = GetHealingPercentHp();
        await CreatureCmd.Heal(base.Owner.Creature, healNum);
        if(Owner.Creature.GetPower<DoomPower>() is DoomPower doomP)
        {
            await PowerCmd.ModifyAmount(doomP, -healNum, Owner.Creature, this);
        }
        await CommonActions.Draw(this, choiceContext);
    }
}
