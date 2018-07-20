using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public class AlterAppOrderStatusOutputDto
    {
        public bool Result { get; set; }
        public bool HasMealCode { get; set; }
        public string MealCode { get; set; }
        public string Message { get; set; }
    }
}
