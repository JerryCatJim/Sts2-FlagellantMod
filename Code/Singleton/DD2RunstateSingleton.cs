using BaseLib.Abstracts;
using Flagellant.Code.Helper;
using MegaCrit.Sts2.Core.Rooms;

namespace Flagellant.Code.Singleton;

public class DD2RunstateSingleton : CustomSingletonModel
{
    public DD2RunstateSingleton() : base(HookType.Run)
    {

    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        DD2CombatSingleton.ResetValue(); 
        DD2Helper.ResetCombatDictionaries();

        return Task.CompletedTask;
    }
}