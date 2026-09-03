using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Audio;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class CalmStrike : FlagellantCardModel
{
    protected override bool ShouldGlowGoldInternal => IsStressLessEqual(5);
    public CalmStrike() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithDamage(5, 2);
        WithStress(5);
        WithPower<ComboPower>(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        CombatAudioManager.PlayCombatSfx("res://Flagellant/Sounds/Watcher/calm_enter.ogg", true, true, 0);

        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        if(IsStressLessEqual(5))
        {
            await CommonActions.Apply<ComboPower>(choiceContext, cardPlay.Target, this);
        }
    }
}
