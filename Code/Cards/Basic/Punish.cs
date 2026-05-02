using BaseLib.Abstracts;
using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using Flagellant.Code.Powers;
using Flagellant.Code.ResoluteOrMeltdown;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Flagellant.Code.Cards.Ancient;

namespace Flagellant.Code.Cards.Basic;

[Pool(typeof(FlagellantCardPool))]
public class Punish : FlagellantCardModel, ITranscendenceCard
{
    public Punish() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(8);
        WithPoison(4);
        WithLossPercent(10);
        WithAnimName("Punish");
        WithPower<ComboPower>(1);  //要注册过这个类型的值 才能在Formatter中正确解析{ComboPower:{comboIcons()}}等类似的格式
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        await CommonActions.CardAttack(this, cardPlay.Target).Execute(choiceContext);
        await CommonActions.Apply<PoisonPower>(cardPlay.Target, this);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, GetLossPercentHp(), ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        if(IsUpgraded)
        {
            await CommonActions.Apply<ComboPower>(cardPlay.Target, this);
        }
    }
    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Card<Execution>();
    }
}
