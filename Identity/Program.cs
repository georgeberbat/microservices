﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace Identity
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return new IdentityHost().Run(args).GetAwaiter().GetResult();
        }
    }
}