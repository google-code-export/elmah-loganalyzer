﻿using ElmahLogAnalyzer.Core.Infrastructure.Cache;
using ElmahLogAnalyzer.Core.Infrastructure.Web;
using ElmahLogAnalyzer.Core.Integrations.HttpUserAgentResolving;
using NUnit.Framework;

namespace ElmahLogAnalyzer.IntegrationTests.Integrations
{
	[TestFixture]
	public class UserAgentStringClientInformationResolverTests : IntegrationTestBase
	{
		[Test]
		public void Resolve_ResolvesClientInformation()
		{
			// arrange
			var resolver = new HttpUserAgentStringClientInformationResolver(new WebRequestHelper(), new CacheHelper());
			
			const string httpUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; .NET CLR 3.0.04506.30; .NET CLR 3.0.04506.648; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET4.0C; .NET4.0E)";

			// act
			var result = resolver.Resolve(httpUserAgent);

			// assert
			Assert.That(result.Browser, Is.EqualTo("Internet Explorer 7.0"));
			Assert.That(result.Platform, Is.EqualTo("Windows"));
			Assert.That(result.OperatingSystem, Is.EqualTo("Windows XP"));
		}
	}
}
