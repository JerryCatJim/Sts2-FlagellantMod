using Flagellant.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Flagellant.Code.Helper;

public static class DD2Helper
{
    public static bool IsFlagellant(Creature? creature)
    {
        return creature != null && creature.Player != null && creature.Player.Character is IGetDD2CharacterType DD2Character && DD2Character.TryGetCharacterType() == "Flagellant";
    }
    public static bool IsFlagellant(Player? player)
    {
        return player != null && player.Character is IGetDD2CharacterType DD2Character && DD2Character.TryGetCharacterType() == "Flagellant";
    }
}
