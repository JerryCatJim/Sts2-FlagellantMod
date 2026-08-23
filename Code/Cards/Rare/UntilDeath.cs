using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Audio;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Flagellant.Code.Cards.Rare;

[Pool(typeof(FlagellantCardPool))]
public class UntilDeath : FlagellantCardModel
{
    public UntilDeath() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<UntilDeathPower>(50);
        WithKeyword(CardKeyword.Ethereal);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (LocalContext.IsMe(base.Owner))
        {
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(NSmokyVignetteVfx.Create(new Color(1.0f, 0.15f, 0.08f, 0.66f), new Color(4.0f, 0.0f, 0.0f, 0.33f)));
        }
        CombatAudioManager.PlayCombatSfx("res://Flagellant/Sounds/Watcher/wrath_enter.ogg", true, true, 0);
        await CommonActions.ApplySelf<UntilDeathPower>(choiceContext, this);
        if (IsUpgraded && base.CombatState != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(base.CombatState.CreateCard<UntilDeath>(base.Owner), PileType.Hand, base.Owner);
        }
    }
}
