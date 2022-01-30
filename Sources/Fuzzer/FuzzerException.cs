// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;

namespace Fuzzer;

internal class FuzzerException : Exception
{
    public FuzzerException(Exception exception, string testCase, string operation)
        : this(exception.Message, testCase, operation)
    {
    }

    public FuzzerException(string message, string testCase, string operation)
        : base($"{message}\nTest case:\n  Test case: {testCase}\n  Operation: {operation}")
    {
    }
}
