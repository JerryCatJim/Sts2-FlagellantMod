using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Cards.Token;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Cards.Common;

[Pool(typeof(FlagellantCardPool))]
public class PurifyMind : FlagellantCardModel
{
    public PurifyMind() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithTip(new TooltipSource((CardModel _) => HoverTipFactory.FromCard<Penance>(_.IsUpgraded)));
        WithCards(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel? cardModel = 
            (await CardSelectCmd.FromHand(
                prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, (int)base.DynamicVars.Cards.BaseValue), 
                context: choiceContext, 
                player: base.Owner, 
                filter: null, 
                source: this)
            )
            .FirstOrDefault();
        if (cardModel != null && base.CombatState != null)
        {
            CardModel cardModel2 = base.CombatState.CreateCard<Penance>(base.Owner);
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(cardModel2);
            }
            await CardCmd.Transform(cardModel, cardModel2);
        }
    }
}