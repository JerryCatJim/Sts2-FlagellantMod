using BaseLib.Utils;
using Flagellant.Code.Abstract;
using Flagellant.Code.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Flagellant.Code.Cards.Uncommon;

[Pool(typeof(FlagellantCardPool))]
public class Martyrdom : FlagellantCardModel
{
    private decimal _doomNum = 0;
    public Martyrdom() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithPowerTip<DoomPower>();
        WithAnimName("More");
        WithKeyword(CardKeyword.Exhaust);
        WithCalculatedDamage(0, ((CardModel card, Creature? c) =>
        {
            if (card != null && card is Martyrdom myCard)
            {
                if (myCard._doomNum != 0)
                {
                    return myCard._doomNum;
                }
                else
                {
                    return myCard.Owner.Creature.GetPower<DoomPower>()?.Amount ?? 0m;
                }
            }
            return 0;
        }
        ));
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayCardAnim();
        _doomNum = base.Owner.Creature.GetPower<DoomPower>()?.Amount ?? 0m;
        if (_doomNum > 0 || base.DynamicVars.CalculatedDamage.EnchantedValue > 0)
        {
            if (base.CombatState != null)
            {
                NFireBurstVfx? child1 = NFireBurstVfx.Create(base.Owner.Creature, 0.75f);
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child1);
                foreach (Creature hittableEnemy in base.CombatState.HittableEnemies)
                {
                    NFireBurstVfx? child2 = NFireBurstVfx.Create(hittableEnemy, 0.75f);
                    NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(child2);
                }
            }
            await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

            if (_doomNum > 0m)
            {
                await CreatureCmd.Damage(choiceContext, Owner.Creature, _doomNum, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
                _doomNum = 0;
            }
        }
        //无条件移除，别挪到if (_doomNum > 0m)里，以免打出此牌造成伤害后又因为各种原因获得了灾厄
        await PowerCmd.Remove<DoomPower>(base.Owner.Creature);
    }
}