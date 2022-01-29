// Copyright (c) 2022 DataStructures.NET.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/DataStructures.NET

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuzzer;

internal interface IValidator<TTested, TOracle>
{
    public string ToTestCase(TTested tested);

    public void Validate(TTested tested, TOracle oracle);
}
