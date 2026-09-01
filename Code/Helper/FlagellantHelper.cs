using Flagellant.Code.Config;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Flagellant.Code.Helper;

public static class FlagellantHelper
{
    public static void ResetAdvancedConditions(AnimationTree? animTree, Creature creature)
    {
        AnimationTree? animationTree = animTree;
        if (animationTree == null && creature != null)
        {
            NCreature? creatureNode = creature.GetCreatureNode();
            if (creatureNode != null)
            {
                animationTree = creatureNode.Visuals.GetNodeOrNull<AnimationTree>("AnimationTree");
            }
        }
        if (animationTree != null)
        {
            if (FlagellantConfig.ShouldUseDeathDoorIdle)
            {
                animationTree.Set("parameters/conditions/HitToIdle", !DD2Helper.WillDieInDoom(creature));
                animationTree.Set("parameters/conditions/HitToDeathIdle", DD2Helper.WillDieInDoom(creature));
            }
            else
            {
                animationTree.Set("parameters/conditions/HitToIdle", true);
                animationTree.Set("parameters/conditions/HitToDeathIdle", false);
            }
        }
    }
    public static bool IsInAnyIdle(AnimationTree? animTree, Creature creature)
    {
        AnimationTree? animationTree = animTree;
        if (animationTree == null && creature != null)
        {
            NCreature? creatureNode = creature.GetCreatureNode();
            if (creatureNode != null)
            {
                animationTree = creatureNode.Visuals.GetNodeOrNull<AnimationTree>("AnimationTree");
            }
        }
        if (animationTree != null)
        {
            var state_machine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
            if (state_machine != null)
            {
                return state_machine.GetCurrentNode() == "Idle" ||
                    state_machine.GetCurrentNode() == "CalmIdle" ||
                    state_machine.GetCurrentNode() == "Revive"; 
                //|| state_machine.GetFadingFromNode() == "Idle" || xxxxxx;
            }
        }
        return false;
    }
}
