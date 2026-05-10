using BaseLib.Abstracts;
using Flagellant.Code.Cards.Uncommon;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Flagellant.Code.Powers;

public class SpreadingAnxietyPower : CustomTemporaryPowerModelWrapper<SpreadingAnxiety, StrengthPower>
{
    //protected override int LastForXExtraTurns => 0;  //BUFF可以存在几回合
    /*public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "default_power.png".PowerImagePath();
        }
    }
    public override string CustomBigIconPath => CustomPackedIconPath;*/

    //public override AbstractModel OriginModel => ModelDb.Card<SpreadingAnxiety>();

    //public override PowerModel InternallyAppliedPower => ModelDb.Power<StrengthPower>();
    /*protected override Func<PlayerChoiceContext, Creature, decimal, Creature?, CardModel?, bool, Task> ApplyPowerFunc =>
        (context, target, amount, applier, cardSource, silent) =>
        {
            // 在这里实现你自定义的 Power 应用逻辑
            return PowerCmd.Apply<StrengthPower>(target, amount, applier, cardSource, silent); //new ThrowingPlayerChoiceContext(),
        };*/
}