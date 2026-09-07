using BaseLib.Extensions;
using BaseLib.Utils;
using Flagellant.Code.Character;
using Flagellant.Code.Config;
using Flagellant.Code.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Abstract;

[Pool(typeof(FlagellantCardPool))]
public abstract class FlagellantCardModel(
    int canonicalEnergyCost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool shouldShowInCardLibrary = true) :
    MyConstructedCardModel(canonicalEnergyCost, type, rarity, targetType, shouldShowInCardLibrary)
{
    public string _cardSelectAnimName = "DoNothing";
    public string _cardPlayAnimName = "DoNothing";
    public string CardSelectAnimName => _cardSelectAnimName;
    public string CardPlayAnimName => _cardPlayAnimName;
    protected FlagellantCardModel WithAnimName(string AnimName)
    {
        _cardSelectAnimName = AnimName;
        _cardPlayAnimName = AnimName;
        return this;
    }
    protected async Task PlayCardAnim(float waitTime = 0.0f)
    {
        if (!HasCardPlayAnimName())
        {
            return;
        }
        await CreatureCmd.TriggerAnim(Owner.Creature, "CardPlay/" + CardPlayAnimName, waitTime);
    }
    protected bool HasCardPlayAnimName()
    {
        if (CardPlayAnimName == null || CardPlayAnimName == "" || CardPlayAnimName == "DoNothing")
        {
            return false;
        }
        return true;
    }
    #region PackedAttackCommands
    protected static AttackCommand CardAttackWithCustomAnim(CardModel card, CardPlay? play, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        if (card is FlagellantCardModel myCard && myCard.HasCardPlayAnimName())
        {
            return CommonActions.CardAttack(card, play, hitCount, vfx, sfx, tmpSfx).WithAttackerAnim("CardPlay/" + myCard.CardPlayAnimName, 0f);
        }
        else
        {
            return CommonActions.CardAttack(card, play, hitCount, vfx, sfx, tmpSfx);
        }
    }
    protected static AttackCommand CardAttackWithCustomAnim(CardModel card, CardPlay? cardPlay, Creature? target, decimal damage, ValueProp valueProp, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        if (card is FlagellantCardModel myCard && myCard.HasCardPlayAnimName())
        {
            return CommonActions.CardAttack(card, cardPlay, target, damage, valueProp, hitCount, vfx, sfx, tmpSfx).WithAttackerAnim("CardPlay/" + myCard.CardPlayAnimName, 0f);
        }
        else
        {
            return CommonActions.CardAttack(card, cardPlay, target, damage, valueProp, hitCount, vfx, sfx, tmpSfx);
        }
    }
    protected static AttackCommand CardAttackWithCustomAnim(CardModel card, CardPlay? cardPlay, Creature? target, CalculatedDamageVar calculatedDamage, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        if (card is FlagellantCardModel myCard && myCard.HasCardPlayAnimName())
        {
            return CommonActions.CardAttack(card, cardPlay, target, calculatedDamage, hitCount, vfx, sfx, tmpSfx).WithAttackerAnim("CardPlay/" + myCard.CardPlayAnimName, 0f);
        }
        else
        {
            return CommonActions.CardAttack(card, cardPlay, target, calculatedDamage, hitCount, vfx, sfx, tmpSfx);
        }
    }
    protected static AttackCommand CardAttackWithCustomAnim(CardModel card, CardPlay? cardPlay, Creature? target, CalculatedDamageVar calculatedDamage, ValueProp valueProp, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        if (card is FlagellantCardModel myCard && myCard.HasCardPlayAnimName())
        {
            return CommonActions.CardAttack(card, cardPlay, target, calculatedDamage, valueProp, hitCount, vfx, sfx, tmpSfx).WithAttackerAnim("CardPlay/" + myCard.CardPlayAnimName, 0f);
        }
        else
        {
            return CommonActions.CardAttack(card, cardPlay, target, calculatedDamage, valueProp, hitCount, vfx, sfx, tmpSfx);
        }
    }
    #endregion PackedAttackCommands

    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            if (FlagellantConfig.ShouldUseRemadeCardImage
                && ResourceLoader.Exists($"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RemadeCardImagePath()))
            {
                path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RemadeCardImagePath();
            }
            return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();
        }
    }

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath
    {
        get
        {
            /*var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
			return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();*/
            return CustomPortraitPath;
        }
    }

    //Optional and I'm not sure it's functional yet.
    public override string BetaPortraitPath
    {
        get
        {
            /*var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
			return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();*/
            return CustomPortraitPath;
        }
    }
}
