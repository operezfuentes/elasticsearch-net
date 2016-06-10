﻿using System;
using System.Collections.Generic;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Tests.Framework;
using Tests.Framework.Integration;
using Tests.Framework.MockData;
using Xunit;
using static Nest.Infer;

namespace Tests.Indices.MappingManagement.GetMapping
{
	[Collection(TypeOfCluster.ReadOnly)]
	public class GetMappingApiTests : ApiIntegrationTestBase<IGetMappingResponse, IGetMappingRequest, GetMappingDescriptor<Project>, GetMappingRequest>
	{
		public GetMappingApiTests(ReadOnlyCluster cluster, EndpointUsage usage) : base(cluster, usage) { }

		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.GetMapping<Project>(f),
			fluentAsync: (client, f) => client.GetMappingAsync<Project>(f),
			request: (client, r) => client.GetMapping(r),
			requestAsync: (client, r) => client.GetMappingAsync(r)
		);

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 200;
		protected override HttpMethod HttpMethod => HttpMethod.GET;
		protected override string UrlPath => "/project/_mapping/project?ignore_unavailable=true";

		protected override Func<GetMappingDescriptor<Project>, IGetMappingRequest> Fluent => d => d
			.IgnoreUnavailable();

		protected override GetMappingRequest Initializer => new GetMappingRequest(Index<Project>(), Type<Project>())
		{
			IgnoreUnavailable = true
		};

		protected override void ExpectResponse(IGetMappingResponse response)
		{
			response.IsValid.Should().BeTrue();

			var visitor = new TestVisitor();
			response.Accept(visitor);

			visitor.CountsShouldContainKeyAndCountBe("type", 1);
			visitor.CountsShouldContainKeyAndCountBe("object", 4);
			visitor.CountsShouldContainKeyAndCountBe("date", 4);
			visitor.CountsShouldContainKeyAndCountBe("text", 9);
			visitor.CountsShouldContainKeyAndCountBe("keyword", 9);
			visitor.CountsShouldContainKeyAndCountBe("ip", 1);
			visitor.CountsShouldContainKeyAndCountBe("number", 2);
			visitor.CountsShouldContainKeyAndCountBe("geo_point", 2);
			visitor.CountsShouldContainKeyAndCountBe("completion", 2);
			visitor.CountsShouldContainKeyAndCountBe("nested", 1);
		}
	}

	internal class TestVisitor : IMappingVisitor
	{
		public TestVisitor()
		{
			Counts = new Dictionary<string, int>();
		}

		public int Depth { get; set; }

		public Dictionary<string, int> Counts { get; }

		private void Increment(string key)
		{
			if (!Counts.ContainsKey(key))
			{
				Counts.Add(key, 0);
			}
			Counts[key] += 1;
		}

		public void CountsShouldContainKeyAndCountBe(string key, int count)
		{
			this.Counts.ContainsKey(key).Should().BeTrue();
			this.Counts[key].Should().Be(count);
		}

		public void Visit(StringProperty mapping)
		{
			Increment("string");
		}

		public void Visit(DateProperty mapping)
		{
			Increment("date");
		}

		public void Visit(BinaryProperty mapping)
		{
			Increment("binary");
		}

		public void Visit(NestedProperty mapping)
		{
			Increment("nested");
		}

		public void Visit(GeoPointProperty mapping)
		{
			Increment("geo_point");
		}

		public void Visit(AttachmentProperty mapping)
		{
			Increment("attachment");
		}

		public void Visit(CompletionProperty mapping)
		{
			Increment("completion");
		}

		public void Visit(TokenCountProperty mapping)
		{
			Increment("token_count");
		}

		public void Visit(Murmur3HashProperty mapping)
		{
			Increment("murmur3");
		}

		public void Visit(NumberProperty mapping)
		{
			Increment("number");
		}

		public void Visit(GeoShapeProperty mapping)
		{
			Increment("geo_shape");
		}

		public void Visit(IpProperty mapping)
		{
			Increment("ip");
		}

		public void Visit(ObjectProperty mapping)
		{
			Increment("object");
		}

		public void Visit(BooleanProperty mapping)
		{
			Increment("boolean");
		}

		public void Visit(TextProperty mapping)
		{
			Increment("text");
		}

		public void Visit(KeywordProperty mapping)
		{
			Increment("keyword");
		}

		public void Visit(TypeMapping mapping)
		{
			Increment("type");
		}
	}
}
