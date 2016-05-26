using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foosbot.ImageProcessingUnit.Detection.Contracts
{
    public interface IImageConfiguration
    {
        int CircleDetectionGrayThreshold { get; set; }
        double CircleDetectionCannyThreshold { get; set; }
        double CircleDetectionAccumulatorThreshold { get; set; }
        double CircleDetectionInverseRatio { get; set; }
        int MotionDetectionGrayThreshold { get; set; }
        double MinimalMotionAreaThreshold { get; set; }
        double MinimalMotionPixelsFactor { get; set; }

    }
}
