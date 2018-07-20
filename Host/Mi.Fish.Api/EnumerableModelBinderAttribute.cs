using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Mi.Fish.Api
{
    public class EnumerableModelBinderAttribute : ModelBinderAttribute
    {
        public EnumerableModelBinderAttribute() : base(typeof(EnumerableModelBinder))
        {
            
        }
    }
}
