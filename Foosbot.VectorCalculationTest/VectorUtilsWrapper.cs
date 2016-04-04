using Foosbot.VectorCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.VectorCalculationTest
{
    public class VectorUtilsWrapper : VectorUtils
    {
        public const int XMIN = 0;
        public const int XMAX = 800;
        public const int YMIN = 0;
        public const int YMAX = 400;
        public const double RICOCHE = 0.7;

        public VectorUtilsWrapper()
        {
            XMinBorder = XMIN;
            XMaxBorder = XMAX;
            YMinBorder = YMIN;
            YMaxBorder = YMAX;
            RicocheFactor = RICOCHE;
            _isInitilized = true;
        }
    }
}
