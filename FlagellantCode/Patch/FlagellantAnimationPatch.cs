using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using Flagellant.Character;
using static Godot.GodotObject;
using static Godot.Node;
using static Godot.PackedScene;

[HarmonyPatch(typeof(NCreature), "SetAnimationTrigger")]
public static class FlagellantAnimationPatch
{
	public static void Postfix(NCreature __instance, string trigger)
	{
		if (__instance.Entity == null || !__instance.Entity.IsPlayer)
			return;

		if (__instance.Entity.ModelId.ToString() == "CHARACTER.FLAGELLANT-FLAGELLANT")
			Log.Info("[>>>Flagellant AnimeTrigger=]" + trigger);

		switch (trigger)
		{
			case "Hit":
				PlayAnim(__instance, "flagellant_impact_recover");
				break;

			case "Attack":
				PlayAnim(__instance, "flagellant_attackD_bloodpull_antic", 3);
				break;

			case "Cast":
				PlayAnim(__instance, "flagellant_attackD_bloodpull_antic");
				break;

			case "Dead":
				PlayAnim(__instance, "flagellant_deaths_door_loop");
				break;

			default:
				PlayAnim(__instance, "flagellant_idle_A");
				break;
		}
	}

	private static void PlayAnim(NCreature node, string animName, float speed = 1f, bool fromEnd = false)
	{
		bool bFromEnd = fromEnd;
		var visual = node.GetNodeOrNull<Node2D>("TestFlagellant");
		if (visual == null) return;

		var animplayer = visual.GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		if (animplayer == null) return;

		// 切换动画
		if(!(animplayer.CurrentAnimation == "flagellant_attackD_bloodpull_antic" 
			&& animName == "flagellant_attackD_bloodpull_antic"))
		{
			//攻击动画很长，多次攻击先不打断播放
			animplayer.Stop();
		}
		animplayer.Play(animName, -1f, speed, fromEnd);

		// 只有非 Idle 动画才需要结束后切回 Idle
		if (animName != "flagellant_idle_A")
		{
			animplayer.Connect(AnimationPlayer.SignalName.AnimationFinished, Callable.From((StringName finishedAnim) =>
			{
				Log.Info(">>>" + finishedAnim + "Animation Finished ! ");

				if (finishedAnim == "flagellant_attackD_bloodpull_antic" && !bFromEnd)
				{
					//攻击动作的收手动作，反向播放一下
					PlayAnim(node, "flagellant_attackD_bloodpull_recover");
				}
				else
				{
					PlayAnim(node, "flagellant_idle_A");
				}
			}), (uint)ConnectFlags.OneShot);
		}
	}
}
