using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant.Audio
{
    internal static class AudioCfg
    {
        public static String GetPath(String AudioName)
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
    }
}
