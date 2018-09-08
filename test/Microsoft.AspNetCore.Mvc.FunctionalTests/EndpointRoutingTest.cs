// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests
{
    public class EndpointRoutingTest : RoutingTestsBase<RoutingWebSite.Startup>
    {
        public EndpointRoutingTest(MvcTestFixture<RoutingWebSite.Startup> fixture)
            : base(fixture)
        {
        }

        [Fact]
        public async Task ParameterTransformer_TokenReplacement_Found()
        {
            // Arrange & Act
            var response = await Client.GetAsync("http://localhost/_ParameterTransformer_/_Test_");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<bool>(body);

            Assert.True(result);
        }

        [Fact]
        public async Task ParameterTransformer_TokenReplacement_NotFound()
        {
            // Arrange & Act
            var response = await Client.GetAsync("http://localhost/ParameterTransformer/Test");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AttributeParameters_Found()
        {
            // Arrange & Act
            var response = await Client.GetAsync("http://localhost/EndpointRouting/Index");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<bool>(body);

            Assert.True(result);
        }

        [Fact]
        public async Task AttributeParameters_DefaultValue_Found()
        {
            // Arrange & Act
            var response = await Client.GetAsync("http://localhost/EndpointRouting");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<bool>(body);

            Assert.True(result);
        }

        [Fact]
        public async Task AttributeParameters_ParameterTransformer_Found()
        {
            // Arrange & Act
            var response = await Client.GetAsync("http://localhost/_EndpointRouting_/ParameterTransformer");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<bool>(body);

            Assert.True(result);
        }

        [Fact]
        public async Task AttributeParameters_ParameterTransformer_NotFound()
        {
            // Arrange & Act
            var response = await Client.GetAsync("http://localhost/EndpointRouting/ParameterTransformer");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async override Task HasEndpointMatch()
        {
            // Arrange & Act
            var response = await Client.GetAsync("http://localhost/Routing/HasEndpointMatch");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<bool>(body);

            Assert.True(result);
        }

        [Fact]
        public async override Task RouteData_Routers_ConventionalRoute()
        {
            // Arrange & Act
            var response = await Client.GetAsync("http://localhost/RouteData/Conventional");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultData>(body);

            Assert.Equal(
                Array.Empty<string>(),
                result.Routers);
        }

        [Fact]
        public async override Task RouteData_Routers_AttributeRoute()
        {
            // Arrange & Act
            var response = await Client.GetAsync("http://localhost/RouteData/Attribute");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResultData>(body);

            Assert.Equal(
                Array.Empty<string>(),
                result.Routers);
        }

        // Endpoint routing exposes HTTP 405s for HTTP method mismatches
        [Fact]
        public override async Task ConventionalRoutedController_InArea_ActionBlockedByHttpMethod()
        {
            // Arrange & Act
            var response = await Client.GetAsync("http://localhost/Travel/Flight/BuyTickets");

            // Assert
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(AttributeRoutedAction_MultipleRouteAttributes_WithMultipleHttpAttributes_RespectsConstraintsData))]
        public override async Task AttributeRoutedAction_MultipleRouteAttributes_WithMultipleHttpAttributes_RespectsConstraints(
            string url,
            string method)
        {
            // Arrange
            var expectedUrl = new Uri(url).AbsolutePath;

            // Act
            var response = await Client.SendAsync(new HttpRequestMessage(new HttpMethod(method), url));

            // Assert
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        // Endpoint routing exposes HTTP 405s for HTTP method mismatches
        [Theory]
        [MemberData(nameof(AttributeRoutedAction_RejectsRequestsWithWrongMethods_InRoutesWithoutExtraTemplateSegmentsOnTheActionData))]
        public override async Task AttributeRoutedAction_RejectsRequestsWithWrongMethods_InRoutesWithoutExtraTemplateSegmentsOnTheAction(
            string method,
            string url)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), $"http://localhost{url}");

            // Assert
            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(AttributeRouting_MixedAcceptVerbsAndRoute_UnreachableData))]
        public override async Task AttributeRouting_MixedAcceptVerbsAndRoute_Unreachable(string path, string verb)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(verb), "http://localhost" + path);

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }
    }
}
