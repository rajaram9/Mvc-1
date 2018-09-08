// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;

namespace RoutingWebSite
{
    public class EndpointRoutingController : Controller
    {
        private readonly TestResponseGenerator _generator;

        public EndpointRoutingController(TestResponseGenerator generator)
        {
            _generator = generator;
        }

        [Route("/{controller}/{action=Index}")]
        public IActionResult Index()
        {
            return Ok(true);
        }

        [Route("/{controller:test-transformer}/{action}")]
        public IActionResult ParameterTransformer()
        {
            return Ok(true);
        }
    }
}