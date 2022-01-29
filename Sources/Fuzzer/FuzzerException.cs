// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzer;

internal class FuzzerException : Exception
{
    public FuzzerException(ValidationException validationException, string testCase, string operation)
        : this(validationException.Message, testCase, operation)
    {
    }

    public FuzzerException(string message, string testCase, string operation)
        : base($"{message}\nTest case:\n  Tree: {testCase}\n  Operation: {operation}")
    {
    }
}
