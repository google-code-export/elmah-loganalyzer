﻿using System.Collections.Generic;
using ElmahLogAnalyzer.Core.Infrastructure.Cache;
using NUnit.Framework;

namespace ElmahLogAnalyzer.IntegrationTests.Infrastructure.Cache
{
	[TestFixture]
	public class CacheHelperTests : IntegrationTestBase
	{
		[Test]
		public void Get_NotInCache_ReturnsNull()
		{
			// arrange
			var helper = new CacheHelper();

			// act
			var result = helper.Get<Dictionary<string, string>>("somekey");

			// assert
			Assert.That(result, Is.Null);
		}

		[Test]
		public void Get_InCache_ReturnsCachedItem()
		{
			// arrange
			var helper = new CacheHelper();

			var dictionary = new Dictionary<string, string>();

			helper.Set("someotherkey", dictionary);

			// act
			var result = helper.Get<Dictionary<string, string>>("someotherkey");

			// assert
			Assert.That(result, Is.EqualTo(dictionary));
		}
	}
}
