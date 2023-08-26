using BeatSaberMarkupLanguage.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace RegionSearchSS.UI
{
    internal class DaMainMenuUI
    {
        public class DaViewController : PersistentSingleton<DaViewController>
        {
            DaConfig config = DaConfig.Instance;

            [UIValue("regionOptions")]
            private List<object> options = new object[] { "Default", "Americas", "Oceania", "Asia", "Europe", "Africa" }.ToList();

            [UIValue("modEnabled")]
            private bool modEnabled
            {
                get => config.Enabled; set => config.Enabled = value;
            }

            [UIValue("SelectedRegion")]

            string SelectedRegion
            {
                get => config.SelectedRegion;
                set
                {
                    config.SelectedRegion = value;
                }
            }
        }
    }
}
