using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Internal;

namespace Mi.Fish.Api
{
    public class EnumerableModelBinder : IModelBinder
    {
        /// <summary>Attempts to bind a model.</summary>
        /// <param name="bindingContext">The <see cref="T:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext" />.</param>
        /// <returns>
        /// <para>
        /// A <see cref="T:System.Threading.Tasks.Task" /> which will complete when the model binding process completes.
        /// </para>
        /// <para>
        /// If model binding was successful, the <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext.Result" /> should have
        /// <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingResult.IsModelSet" /> set to <c>true</c>.
        /// </para>
        /// <para>
        /// A model binder that completes successfully should set <see cref="P:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingContext.Result" /> to
        /// a value returned from <see cref="M:Microsoft.AspNetCore.Mvc.ModelBinding.ModelBindingResult.Success(System.Object)" />.
        /// </para>
        /// </returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelMetadata.Name);

            var value = valueResult.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries);

            if (!value.Any())
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var valueType = bindingContext.ModelMetadata.ElementType;
            var converter = TypeDescriptor.GetConverter(valueType);

            var array = Array.CreateInstance(valueType, value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                array.SetValue(converter.ConvertFromString(value[i]), i);
            }

            bindingContext.Result = ModelBindingResult.Success(array);

            return Task.CompletedTask;
        }
    }
}