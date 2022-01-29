// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;

namespace Fuzzer;

internal class ValidationException : Exception
{
    public ValidationException(string message)
        : base(message)
    {
    }
}
