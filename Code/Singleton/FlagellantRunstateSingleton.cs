using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Rooms;

namespace Flagellant.Code.Singleton;

public class FlagellantRunstateSingleton : CustomSingletonModel
{
    public FlagellantRunstateSingleton() : base(HookType.Run)
    {

    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        FlagellantCombatSingleton.ResetValue();

        return Task.CompletedTask;
    }
}