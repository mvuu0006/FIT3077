using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIT3077_Pre1975.Models
{
    public class CholesterolPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Label { get; set; }
        public float[] Score { get; set; }

        public int LabelAsNumber => Label ? 1 : 0;
    }
}
