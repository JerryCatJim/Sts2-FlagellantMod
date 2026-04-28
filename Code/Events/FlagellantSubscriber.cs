using Flagellant.Code.Core;
using Flagellant.Code.ResoluteOrMeltdown;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Code.Events;
internal class FlagellantSubscriber
{
    public static void Subscribe()
    {
        ModHelper.SubscribeForCombatStateHooks(MainFile.ModId, CollectModels);
    }

    public static IEnumerable<AbstractModel> CollectModels(CombatState combatState)
    {
        return combatState.Players
            .Select(FlagellantModel.GetResoluteOrMeltdownModel)
            .Where(s => s is not NoResoluteAndMeltdown);
    }
}
