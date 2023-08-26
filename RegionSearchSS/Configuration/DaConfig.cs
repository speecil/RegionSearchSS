using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace RegionSearchSS
{
    internal class DaConfig
    {
        public static DaConfig Instance { get; set; }

        public virtual string SelectedRegion { get; set; } = "Default";

        public virtual bool Enabled { get; set; }
    }
}
