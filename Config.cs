using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLRealism
{
    public class Config
    {
        [Description("Apply bleeding effect when player is hit by 939s claw?")]
        public bool ApplyBleedingPer939Claw { get; set; } = true;
    }
}
