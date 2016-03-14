using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.VectorCalculation
{
    public class VectorCallculationUnit
    {
        Helpers.UpdateMarkupLineDelegate _onUpdateMarkupLine;

        public VectorCallculationUnit(Helpers.UpdateMarkupLineDelegate onUpdateMarkupLine)
        {
            _onUpdateMarkupLine = onUpdateMarkupLine;
        }
    }
}
