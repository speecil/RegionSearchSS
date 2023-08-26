using BeatSaberMarkupLanguage.GameplaySetup;
using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using System.Collections.Generic;
using System.Reflection;
using IPALogger = IPA.Logging.Logger;
namespace RegionSearchSS
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        public static Harmony harmony;

        public static Dictionary<string, string> RegionCountryDict = new Dictionary<string, string>()
        {
            {"asia", "AF,AM,AZ,BH,BD,BT,BN,KH,CN,CY,GE,HK,IN,ID,IR,IQ,IL,JP,JO,KZ,KP,KR,KW,KG,LA,LB,MO,MY,MV,MN,MM,NP,OM,PK,PS,PH,QA,SA,SG,LK,SY,TW,TJ,TH,TL,TR,TM,AE,UZ,VN,YE" },
            {"europe", "AX,AL,AD,AT,BY,BE,BA,BG,HR,CZ,DK,EE,FO,FI,FR,DE,GI,GR,GG,VA,HU,IS,IE,IM,IT,JE,LV,LI,LT,LU,MT,MD,MC,ME,NL,MK,NO,PL,PT,RO,RU,SM,RS,SK,SI,ES,SJ,SE,CH,UA,GB" },
            {"africa", "DZ,AO,BJ,BW,BF,BI,CV,CM,CF,TD,KM,CG,CD,CI,DJ,EG,GQ,ER,SZ,ET,GA,GM,GH,GN,GW,KE,LS,LR,LY,MG,MW,ML,MR,MU,YT,MA,MZ,NA,NE,NG,RE,RW,SH,ST,SN,SC,SL,SO,ZA,SS,SD,TZ,TG,TN,UG,ZM,ZW"},
            {"oceania", "AS,AU,CX,CC,CK,FJ,PF,GU,HM,KI,MH,FM,NR,NC,NZ,NU,NF,MP,PW,PG,PN,WS,SB,TK,TO,TV,UM,VU,WF" },
            {"americas", "AI,AG,AR,AW,BS,BB,BZ,BM,BO,BQ,BV,BR,CA,KY,CL,CO,CR,CU,CW,DM,DO,EC,SV,FK,GF,GL,GD,GP,GT,GY,HT,HN,JM,MQ,MX,MS,NI,PA,PY,PE,PR,BL,KN,LC,MF,PM,VC,SX,GS,SR,TT,TC,US,UY,VE,VG,VI" }
        };

        [Init]
        public void Init(IPALogger logger, IPA.Config.Config conf)
        {
            Instance = this;
            Log = logger;
            DaConfig.Instance = conf.Generated<DaConfig>();
            harmony = new Harmony("Speecil.BeatSaber.RegionSearchSS");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            GameplaySetup.instance.AddTab("RegionSearchSS", "RegionSearchSS.UI.settings.bsml", RegionSearchSS.UI.DaMainMenuUI.DaViewController.instance, MenuType.All);
        }
    }
}
