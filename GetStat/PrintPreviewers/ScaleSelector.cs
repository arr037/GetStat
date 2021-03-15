using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStat.Models;

namespace GetStat.PrintPreviewers
{
    public sealed class ScaleSelector
        : BaseVM
    {
        double scale = 1;
        public double Scale
        {
            get { return scale; }
            set { scale = value;
                OnPropertyChanged(nameof(scale));
            }
        }

        public void Zoom(int delta)
        {
            Scale *= 1 + Math.Sign(delta) * 0.1;
        }
    }
}
