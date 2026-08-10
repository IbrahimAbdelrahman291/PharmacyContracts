using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.SharedKernel.Wrappers
{
    public class Result
    {
        public bool Succeeded { get; protected set; }
        public string[] Errors { get; protected set; } = Array.Empty<string>();

        public static Result Success() => new() { Succeeded = true };
        public static Result Failure(params string[] errors) => new() { Succeeded = false, Errors = errors };
    }

    public class Result<T> : Result
    {
        public T? Data { get; private set; }

        public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };
        public static new Result<T> Failure(params string[] errors) => new() { Succeeded = false, Errors = errors };
    }
}
