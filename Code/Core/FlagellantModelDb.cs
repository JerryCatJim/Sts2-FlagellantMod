using MegaCrit.Sts2.Core.Models;
using Flagellant.Code.ResoluteOrMeltdown;

namespace Flagellant.Code.Core;

public class FlagellantModelDb
{
    public static T ResoluteOrMeltdown<T>() where T : ResoluteOrMeltdownModel
    {
        return ModelDb.GetById<T>(ModelDb.GetId<T>());
    }
}