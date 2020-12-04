#region Copyright & License

// Copyright © 2012 - 2020 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Be.Stateless.BizTalk.Dsl
{
	public class BizTalkAssemblyResolverFixture
	{
		[Fact]
		public void RefineEmptyProbingPath()
		{
			BizTalkAssemblyResolver.AddProbingPaths(string.Empty);
			BizTalkAssemblyResolver.Instance.PrivateProbingPaths.Should().BeEmpty();
		}

		[Fact]
		public void RefineEmptyProbingPathArray()
		{
			BizTalkAssemblyResolver.AddProbingPaths(string.Empty);
			BizTalkAssemblyResolver.Instance.PrivateProbingPaths.Should().BeEmpty();
		}

		[Fact]
		public void RefineJoinedProbingPaths()
		{
			BizTalkAssemblyResolver.AddProbingPaths(@"c:\folder\one;c:\folder\two", @"c:\folder\six; ;;c:\folder\ten;");
			BizTalkAssemblyResolver.Instance.PrivateProbingPaths.Should().BeEquivalentTo(@"c:\folder\one", @"c:\folder\two", @"c:\folder\six", @"c:\folder\ten");
		}

		[Fact]
		public void RefineNullProbingPath()
		{
			BizTalkAssemblyResolver.AddProbingPaths(null);
			BizTalkAssemblyResolver.Instance.PrivateProbingPaths.Should().BeEmpty();
		}

		[Fact]
		public void RefineNullProbingPathArray()
		{
			BizTalkAssemblyResolver.AddProbingPaths(new string[] { null });
			BizTalkAssemblyResolver.Instance.PrivateProbingPaths.Should().BeEmpty();
		}

		[Fact]
		public void RefineProbingPath()
		{
			BizTalkAssemblyResolver.AddProbingPaths(@"c:\folder\one");
			BizTalkAssemblyResolver.Instance.PrivateProbingPaths.Should().BeEquivalentTo(@"c:\folder\one");
		}

		[Fact]
		public void RefineProbingPathArray()
		{
			var probingPaths = new[] { @"c:\folder\one\file.dll", @"c:\folder\two\file.dll" }.Select(Path.GetDirectoryName).ToArray();
			BizTalkAssemblyResolver.AddProbingPaths(probingPaths);
			BizTalkAssemblyResolver.Instance.PrivateProbingPaths.Should().BeEquivalentTo(@"c:\folder\one", @"c:\folder\two");
		}
	}
}
