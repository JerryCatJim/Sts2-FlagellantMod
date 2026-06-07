using Flagellant.Code.Monster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Audio
{
    internal static class AudioCfg
    {
        public static String GetFlagellantPath(String AudioName)
        {
            String FileName = "";
            String PathHead = "res://Flagellant/Sounds/";
            String FileEnd = ".wav";

            switch (AudioName)
            {
                //CardSelect
                case "CardSelect/Punish":
                case "CardSelect/Necrosis":
                    FileName = "sfx_hero_flg_punish_antic";
                    break;
                case "CardSelect/Fester":
                case "CardSelect/AcidRain":
                    FileName = "sfx_hero_flg_acid_antic";
                    break;
                case "CardSelect/Deathless":
                case "CardSelect/Endure":
                case "CardSelect/More":
                case "CardSelect/Sepsis":
                case "CardSelect/Undying":
                    FileName = "sfx_hero_flg_sepsis_antic";
                    break;
                case "CardSelect/Lash":
                case "CardSelect/Suffer":
                    FileName = "sfx_hero_flg_suffer_antic";
                    break;
                //CardPlay
                case "CardPlay/Punish":
                    FileName = "sfx_hero_flg_punish_use";
                    break;
                case "CardPlay/Fester":
                    FileName = "sfx_hero_flg_fester_use";
                    break;
                case "CardPlay/Deathless":
                case "CardPlay/Suffer":
                case "CardPlay/Undying":
                    FileName = "sfx_hero_flg_suffer_use";
                    break;
                case "CardPlay/Endure":
                    FileName = "sfx_hero_flg_endure_use";
                    break;
                case "CardPlay/Lash":
                    FileName = "sfx_hero_flg_lash_use";
                    break;
                case "CardPlay/AcidRain":
                    FileName = "sfx_hero_flg_acid_use";
                    break;
                case "CardPlay/More":
                    FileName = "sfx_hero_flg_more_use";
                    break;
                case "CardPlay/Sepsis":
                    FileName = "sfx_hero_flg_sepsis_use";
                    break;
                case "CardPlay/Necrosis":
                    FileName = "sfx_hero_flg_necro_use";
                    break;
                //CardPlayRecover
                case "CardPlay/Punish_Recover":
                case "CardPlay/Necrosis_Recover":
                    FileName = "sfx_hero_flg_punish_return";
                    break;
                case "CardPlay/Fester_Recover":
                case "CardPlay/AcidRain_Recover":
                    FileName = "sfx_hero_flg_acid_return";
                    break;
                case "CardPlay/Deathless_Recover":
                case "CardPlay/Endure_Recover":
                case "CardPlay/More_Recover":
                case "CardPlay/Undying_Recover":
                    FileName = "sfx_hero_flg_more_return";
                    break;
                case "CardPlay/Sepsis_Recover":
                    FileName = "sfx_hero_flg_sepsis_return";
                    break;
                case "CardPlay/Lash_Recover":
                case "CardPlay/Suffer_Recover":
                    FileName = "sfx_hero_flg_suffer_return";
                    break;
                //Do nothing
                default:
                    break;
            }
            return FileName == "" ? FileName : PathHead+FileName+FileEnd;
        }

        public static String GetDeathPath(String AudioName)
        {
            String FileName = "";
            String PathHead = "res://Flagellant/Monster_Death/Sound_Death/";
            String FileEnd = ".wav";

            switch (AudioName)
            {
                case "Spawn":
                    FileName = "death_intro_v4";
                    break;
                case "Hit":
                    FileName = "vo_monster_death_hurt_0" + (DeathListenForRunStateSingleton.HitCount + 1);
                    DeathListenForRunStateSingleton.HitCount++;
                    break;
                case "Dead":
                    FileName = "sfx_death_death";
                    break;

                //Attack_XXX
                case "Attack_Point":
                    FileName = "sfx_death_mm_antic";
                    break;
                case "Attack_B":
                    FileName = "sfx_death_reaver_antic";
                    break;
                case "Attack_C":
                    FileName = "sfx_death_waning_antic";
                    break;
                case "Attack_Trample":
                    FileName = "sfx_death_trample_antic";
                    break;

                //XXX_Action
                case "Point_Action":
                    FileName = "sfx_death_mm_use";
                    break;
                case "B_Action":
                    FileName = "sfx_death_reaver_use";
                    break;
                case "C_Action":
                    FileName = "sfx_death_waning_use";
                    break;
                case "Trample_Action":
                    FileName = "sfx_death_trample_use";
                    break;

                //XXX_Recover
                case "Point_Recover":
                    FileName = "sfx_death_mm_return";
                    break;
                case "B_Recover":
                    FileName = "sfx_death_shadow_return";
                    break;
                case "C_Recover":
                    FileName = "sfx_death_waning_return";
                    break;
                case "Trample_Recover":
                    FileName = "sfx_death_trample_return";
                    break;

                //DoNothing
                default:
                    break;
            }
            return FileName == "" ? FileName : PathHead + FileName + FileEnd;
        }
    }
}
