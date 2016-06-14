﻿using System;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Tests.Framework;
using Tests.Framework.Integration;
using Xunit;

namespace Tests.Cat.CatHealth
{
	[Collection(TypeOfCluster.ReadOnly)]
	public class CatHealthApiTests : ApiIntegrationTestBase<ICatResponse<CatHealthRecord>, ICatHealthRequest, CatHealthDescriptor, CatHealthRequest>
	{
		public CatHealthApiTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }
		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.CatHealth(),
			fluentAsync: (client, f) => client.CatHealthAsync(),
			request: (client, r) => client.CatHealth(r),
			requestAsync: (client, r) => client.CatHealthAsync(r)
		);

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 200;
		protected override HttpMethod HttpMethod => HttpMethod.GET;
		protected override string UrlPath => "/_cat/health";

		protected override void ExpectResponse(ICatResponse<CatHealthRecord> response)
		{
			response.Records.Should().NotBeEmpty().And.Contain(a => !string.IsNullOrEmpty(a.Status));
		}
	}

	[Collection(TypeOfCluster.ReadOnly)]
	public class CatHealthNoTimestampApiTests : ApiIntegrationTestBase<ICatResponse<CatHealthRecord>, ICatHealthRequest, CatHealthDescriptor, CatHealthRequest>
	{
		public CatHealthNoTimestampApiTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }
		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.CatHealth(f),
			fluentAsync: (client, f) => client.CatHealthAsync(f),
			request: (client, r) => client.CatHealth(r),
			requestAsync: (client, r) => client.CatHealthAsync(r)
		);

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 200;
		protected override HttpMethod HttpMethod => HttpMethod.GET;
		protected override string UrlPath => "/_cat/health?ts=false";

		protected override Func<CatHealthDescriptor, ICatHealthRequest> Fluent => s => s
			.Ts(false);

		protected override CatHealthRequest Initializer => new CatHealthRequest
		{
			Ts = false
		};

		protected override void ExpectResponse(ICatResponse<CatHealthRecord> response)
		{
			response.Records.Should().NotBeEmpty().And.Contain(a => !string.IsNullOrEmpty(a.Status));

			foreach (var record in response.Records)
			{
				record.Timestamp.Should().BeNullOrWhiteSpace();
			}
		}
	}

}
