using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using FluentAssertions;
using Nest;
using Newtonsoft.Json.Linq;
using Tests.Framework;
using Tests.Framework.Integration;
using Tests.Framework.MockData;
using Xunit;
using System.IO;

namespace Tests.Document.Single.Index
{
	[Collection(TypeOfCluster.Indexing)]
	public class AttachmentApiTests :
		ApiIntegrationTestBase<IIndexResponse, IIndexRequest<Attachment>, IndexDescriptor<Attachment>, IndexRequest<Attachment>>
	{
		private string IndexName = "attachments";

		private Attachment Document =>
			new Attachment
			{
				// Base 64 encoded version of Attachment_Test_Document.pdf
				File = Attachment.TestPdfDocument
			};

		public AttachmentApiTests(IndexingCluster cluster, EndpointUsage usage) : base(cluster, usage)
		{
			if (!this.Client.IndexExists(IndexName).Exists)
			{
				var indexResponse = this.Client.CreateIndex(IndexName, c => c
					.Mappings(m => m
						.Map<Attachment>(mm => mm
							.Properties(p => p
								.Attachment(a => a
									.Name(n => n.File)
									.AuthorField(d => d
										.Name(n => n.Author)
									)
									.FileField(d => d
										.Name(n => n.File)
									)
									.ContentLengthField(d => d
										.Name(n => n.ContentLength)
									)
									.ContentTypeField(d => d
										.Name(n => n.ContentType)
									)
									.DateField(d => d
										.Name(n => n.Date)
									)
									.KeywordsField(d => d
										.Name(n => n.Keywords)
									)
									.LanguageField(d => d
										.Name(n => n.Language)
									)
									.NameField(d => d
										.Name(n => n.Name)
									)
									.TitleField(d => d
										.Name(n => n.Title)
									)
								)
							)
						)
					)
				);

				if (!indexResponse.IsValid)
				{
					throw new Exception("Could not set up attachment index for test");
				}
			}
		}

		protected override LazyResponses ClientUsage() => Calls(
			fluent: (client, f) => client.Index<Attachment>(this.Document, f),
			fluentAsync: (client, f) => client.IndexAsync<Attachment>(this.Document, f),
			request: (client, r) => client.Index(r),
			requestAsync: (client, r) => client.IndexAsync(r)
			);

		protected override bool ExpectIsValid => true;
		protected override int ExpectStatusCode => 201;
		protected override HttpMethod HttpMethod => HttpMethod.PUT;

		protected override string UrlPath
			=> $"/{IndexName}/attachment/{CallIsolatedValue}?refresh=true";

		protected override bool SupportsDeserialization => false;

		protected override object ExpectJson =>
			new
			{
				file = Attachment.TestPdfDocument
			};

		protected override IndexDescriptor<Attachment> NewDescriptor() => new IndexDescriptor<Attachment>(this.Document);

		protected override Func<IndexDescriptor<Attachment>, IIndexRequest<Attachment>> Fluent => s => s
			.Index(IndexName)
			.Id(CallIsolatedValue)
			.Refresh();

		protected override IndexRequest<Attachment> Initializer =>
			new IndexRequest<Attachment>(this.Document, IndexName, id: CallIsolatedValue)
			{
				Refresh = true,
			};

		protected override void ExpectResponse(IIndexResponse response)
		{
			response.IsValid.Should().BeTrue();

			var searchResponse = Client.Search<Attachment>(s => s
				.Index(IndexName)
				.Query(q => q
					.Match(m => m
						.Field(a => a.File.Suffix("content"))
						.Query("NEST mapper")
					)
				)
			);

			searchResponse.IsValid.Should().BeTrue();

			// all fluent and OIS indexed  documents
			searchResponse.Documents.Count().Should().Be(4);
		}
	}
}
