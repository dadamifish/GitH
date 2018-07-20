﻿using System;
using System.Collections.Generic;
using System.Text;
using Mi.Fish.Common;

namespace Mi.Fish.Infrastructure.Results
{
    /// <summary>
    /// 
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 详细信息
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public ResultCode Code { get; set; }

        /// <summary>
        /// 状态码描述信息
        /// </summary>
        /// <value>
        /// The code desc.
        /// </value>
        public string CodeDesc => _codeDesc ?? (_codeDesc = Code.DisplayName());

        private string _codeDesc;

        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccess => Code == ResultCode.Ok;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="code"></param>
        public Result(ResultCode code)
        {
            Code = code;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public Result(ResultCode code, string message) : this(code)
        {
            Message = message;
        }

        /// <summary>
        /// Ok
        /// </summary>
        public static Result Ok => new Result(ResultCode.Ok);

        /// <summary>
        /// FromData
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<TData> FromData<TData>(TData data) => new Result<TData>(ResultCode.Ok, data);

        /// <summary>
        /// Fail
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result<TData> Fail<TData>(string error) => new Result<TData>(ResultCode.Fail, default(TData), error);

        /// <summary>
        /// Fail
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="code"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result<TData> Fail<TData>(ResultCode code, string error = null) => new Result<TData>(code, default(TData), error);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T> : Result
    {
        /// <summary>
        /// 
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        public Result(ResultCode code, T data) : this(code, data, string.Empty)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="data"></param>
        /// <param name="message"></param>
        public Result(ResultCode code, T data, string message) : base(code, message)
        {
            Data = data;
        }
    }
}
