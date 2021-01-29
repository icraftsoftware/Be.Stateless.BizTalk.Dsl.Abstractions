#region Copyright & License

// Copyright © 2012 - 2021 François Chabot
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

using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Be.Stateless.BizTalk.Dsl.Extensions
{
	internal static class AssemblyNameExtensions
	{
		internal static bool IsNonExistentMicrosoftAssembly(this AssemblyName assemblyName)
		{
			return Regex.IsMatch(assemblyName.Name, @"^Microsoft\.BizTalk\.(ExplorerOM|Pipeline\.Components)\.(resources|XmlSerializers)$", RegexOptions.IgnoreCase)
				|| Regex.IsMatch(assemblyName.Name, @"^Microsoft\.ServiceModel\.(Channels)\.(resources|XmlSerializers)$", RegexOptions.IgnoreCase);
		}

		internal static bool IsNonExistentStatelessAssembly(this AssemblyName assemblyName)
		{
			return Regex.IsMatch(assemblyName.Name, @"^Be\.Stateless\..+\.(resources|XmlSerializers)$", RegexOptions.IgnoreCase);
		}

		internal static bool IsResourceAssembly(this AssemblyName assemblyName)
		{
			return assemblyName.Name.EndsWith(".resources", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
