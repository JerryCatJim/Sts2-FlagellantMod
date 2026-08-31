using Flagellant.Code.Helper;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace Flagellant.Code.Events;
internal class RMSubscriber
{
    public static void Subscribe()
    {
        ModHelper.SubscribeForCombatStateHooks(MainFile.ModId, CollectModels);
    }

    public static IEnumerable<AbstractModel> CollectModels(CombatState combatState)
    {
        return combatState.Players
            .Select(RMHelper.GetResoluteOrMeltdownModel)
            .Where(s => s is not NoResoluteAndMeltdown);
    }
}
