using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mixer
{
    public enum VOLUME_ACTION
    {
        INCREASE,
        DECREASE
    }
    public class KeymapEntry
    {
        public int Slot { get; set; }
        public string? Key { get; set; }
        public string? ProcessName { get; set; }
        public VOLUME_ACTION Action { get; set; }
    }
}
