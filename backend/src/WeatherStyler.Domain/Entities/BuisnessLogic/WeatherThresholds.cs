using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherStyler.Domain.Entities.BuisnessLogic
{
    public class WeatherThresholds
    {
        public float RainThreshold { get; set; } = 0.1f;
        public float WindThreshold { get; set; } = 15.0f;
        public int SunnyCloudThreshold { get; set; } = 30;
    }
}
