﻿using System.Threading.Tasks;
using Nest;
using Tests.Framework;
using Tests.Framework.MockData;
using static Tests.Framework.UrlTester;

#pragma warning disable 618

namespace Tests.Search.Percolator.UnregisterPercolator
{
	public class UnregisterPercolatorUrlTests
	{
		[SkipVersion("5.0.0-alpha2,5.0.0-alpha3", "deprecated")]
		[U] public async Task Urls()
		{
			var name = "name-of-perc";
			var index = "indexx";
			await DELETE($"/{index}/.percolator/{name}")
				.Fluent(c=>c.UnregisterPercolator<Project>(name, s=>s.Index(index)))
				.Request(c=>c.UnregisterPercolator(new UnregisterPercolatorRequest(index, name)))
				.FluentAsync(c=>c.UnregisterPercolatorAsync<Project>(name, s=>s.Index(index)))
				.RequestAsync(c=>c.UnregisterPercolatorAsync(new UnregisterPercolatorRequest(index, name)))
				;

			await DELETE($"/project/.percolator/{name}")
				.Fluent(c=>c.UnregisterPercolator<Project>(name))
				.Request(c=>c.UnregisterPercolator(new UnregisterPercolatorRequest(typeof(Project), name)))
				.FluentAsync(c=>c.UnregisterPercolatorAsync<Project>(name))
				.RequestAsync(c=>c.UnregisterPercolatorAsync(new UnregisterPercolatorRequest(IndexName.From<Project>(), name)))
				;
		}
	}
}
