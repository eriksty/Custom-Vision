﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public class Prediction
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public Boolean CanAdvertisement { get; set; }
        public double Probability { get; set; }
    }
}
