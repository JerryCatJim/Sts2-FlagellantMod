namespace Flagellant.Code.Audio;

public static class CombatAudioCfg
{
    public static string GetFlagellantPath(string AudioName)
    {
        string FileName = "";
        string PathHead = "res://Flagellant/Sounds/";
        string FileEnd = ".wav";

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
            case "CardSelect/Undying":
                FileName = "sfx_hero_flg_redeem_antic";
                break;
            case "CardSelect/Endure":
            case "CardSelect/More":
            case "CardSelect/Sepsis":
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
                FileName = "sfx_hero_flg_redeem_use";
                break;
            case "CardPlay/Undying":
                FileName = "sfx_hero_flg_reclaim_use";
                break;
            case "CardPlay/Suffer":
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
            case "CardPlay/Undying_Recover":
                FileName = "sfx_hero_flg_redeem_return";
                break;
            case "CardPlay/Endure_Recover":
            case "CardPlay/More_Recover":
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
        return FileName == "" ? FileName : PathHead + FileName + FileEnd;
    }

    public static float GetFlagellantVolumeDB(string AudioName)
    {
        float TempDB = -10.0f;
        switch (AudioName)
        {
            //CardSelect
            case "CardSelect/Punish":
            case "CardSelect/Necrosis":
            case "CardSelect/Fester":
            case "CardSelect/AcidRain":
            case "CardSelect/Deathless":
            case "CardSelect/Endure":
            case "CardSelect/More":
            case "CardSelect/Sepsis":
            case "CardSelect/Undying":
            case "CardSelect/Lash":
            case "CardSelect/Suffer":
                TempDB = -10;
                break;
            //CardPlay
            case "CardPlay/Punish":
            case "CardPlay/Suffer":
            case "CardPlay/Sepsis":
            case "CardPlay/Necrosis":
                TempDB = -6;
                break;
            case "CardPlay/AcidRain":
                TempDB = -10;
                break;
            case "CardPlay/Lash":
            case "CardPlay/Endure":
                TempDB = -8;
                break;
            case "CardPlay/Fester":
            case "CardPlay/Deathless":
            case "CardPlay/Undying":
            case "CardPlay/More":
                TempDB = -7;
                break;
            //CardPlayRecover
            case "CardPlay/Punish_Recover":
            case "CardPlay/Necrosis_Recover":
                TempDB = -8;
                break;
            case "CardPlay/Fester_Recover":
            case "CardPlay/AcidRain_Recover":
            case "CardPlay/Sepsis_Recover":
                TempDB = -10;
                break;
            case "CardPlay/Lash_Recover":
            case "CardPlay/Suffer_Recover":
                TempDB = -8;
                break;
            case "CardPlay/Deathless_Recover":
            case "CardPlay/Endure_Recover":
            case "CardPlay/More_Recover":
            case "CardPlay/Undying_Recover":
                TempDB = -6;
                break;
            //Do nothing
            default:
                TempDB = -10;
                break;
        }
        return TempDB;
    }
}
