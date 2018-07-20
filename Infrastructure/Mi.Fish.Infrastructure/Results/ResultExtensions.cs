using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.Infrastructure.Results
{
    public static class ResultExtensions
    {
        public static Result BaseResult<T>(this Result<T> result)
        {
            return new Result(result.Code, result.Message);
        }

        public static Result BaseResult(this Result result)
        {
            return new Result(result.Code, result.Message);
        }
    }
}
