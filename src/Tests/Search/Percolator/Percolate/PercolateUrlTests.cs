﻿using System.Threading.Tasks;
using Nest;
using Tests.Framework;
using Tests.Framework.MockData;
using static Tests.Framework.UrlTester;

#pragma warning disable 618 // testing deprecated percolate APIs

namespace Tests.Search.Percolator.Percolate
{
	[SkipVersion("5.0.0-alpha2,5.0.0-alpha3", "deprecated")]
	public class PercolateUrlTests
	{
		[U] public async Task Urls()
		{
			var id = "name-of-doc";
			var index = "indexx";
			await POST($"/{index}/project/{id}/_percolate")
				.Fluent(c=>c.Percolate<Project>(s=>s.Id(id).Index(index)))
				.Request(c=>c.Percolate(new PercolateRequest<Project>(index, typeof(Project), id)))
				.FluentAsync(c=>c.PercolateAsync<Project>(s=> s.Id(id).Index(index)))
				.RequestAsync(c=>c.PercolateAsync(new PercolateRequest<Project>(index, typeof(Project), id)))
				;

			await POST($"/project/project/{id}/_percolate")
				.Fluent(c=>c.Percolate<Project>(s=>s.Id(id)))
				.Request(c=>c.Percolate(new PercolateRequest<Project>(id)))
				.FluentAsync(c=>c.PercolateAsync<Project>(s=>s.Id(id)))
				.RequestAsync(c=>c.PercolateAsync(new PercolateRequest<Project>(id)))
				;

			await POST($"/{index}/project/_percolate")
				.Fluent(c=>c.Percolate<Project>(s=>s.Index(index)))
				.Request(c=>c.Percolate(new PercolateRequest<Project>(index, typeof(Project))))
				.FluentAsync(c=>c.PercolateAsync<Project>(s=> s.Index(index)))
				.RequestAsync(c=>c.PercolateAsync(new PercolateRequest<Project>(index, typeof(Project))))
				;

			await POST($"/project/project/_percolate")
				.Fluent(c=>c.Percolate<Project>(s=>s))
				.Request(c=>c.Percolate(new PercolateRequest<Project>()))
				.FluentAsync(c=>c.PercolateAsync<Project>(s=>s))
				.RequestAsync(c=>c.PercolateAsync(new PercolateRequest<Project>()))
				;

			var document = new Project { Name = "foo" };

			await POST($"/project/project/_percolate")
				.Fluent(c=>c.Percolate<Project>(s=>s.Document(document)))
				.Request(c=>c.Percolate(new PercolateRequest<Project>(document)))
				.FluentAsync(c=>c.PercolateAsync<Project>(s=>s.Document(document)))
				.RequestAsync(c=>c.PercolateAsync(new PercolateRequest<Project>(document)))
				;
		}
	}
}
