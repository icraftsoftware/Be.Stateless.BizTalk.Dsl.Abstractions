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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Be.Stateless.BizTalk.Dsl
{
	[SuppressMessage("ReSharper", "UnusedType.Global")]
	public class BizTalkAssemblyResolver
	{
		static BizTalkAssemblyResolver()
		{
			const string subKey = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
			using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
			using (var btsKey = baseKey.OpenSubKey(subKey))
			{
				if (btsKey == null)
				{
					DefaultProbingPaths = Array.Empty<string>();
				}
				else
				{
					var installPath = (string) btsKey.GetValue("InstallPath");
					DefaultProbingPaths = new[] {
						installPath,
						Path.Combine(installPath, @"Developer Tools"),
						Path.Combine(installPath, @"SDK\Utilities\PipelineTools")
					};
				}
			}
			Instance = new BizTalkAssemblyResolver();
		}

		private static string[] DefaultProbingPaths { get; }

		internal static BizTalkAssemblyResolver Instance { get; }

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public static void Register(Action<string> logAppender, params string[] probingPaths)
		{
			Instance._logAppender = logAppender;
			if (!DefaultProbingPaths.Any()) logAppender?.Invoke("Default probing paths to BizTalk Developer Tools and Pipeline Tools could not be found.");
			AddProbingPaths(probingPaths ?? Array.Empty<string>());
			AppDomain.CurrentDomain.AssemblyResolve += Instance.OnAssemblyResolve;
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		public static void AddProbingPaths(params string[] probingPaths)
		{
			Instance.PrivateProbingPaths = probingPaths == null
				? Enumerable.Empty<string>()
				: probingPaths.Where(p => !string.IsNullOrWhiteSpace(p)).SelectMany(jp => jp.Split(';').Where(p => !string.IsNullOrWhiteSpace(p)));
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public static void Unregister()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= Instance.OnAssemblyResolve;
		}

		private BizTalkAssemblyResolver() { }

		internal IEnumerable<string> PrivateProbingPaths { get; private set; }

		private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			// nonexistent resource assemblies
			if (args.Name.StartsWith("Microsoft.BizTalk.ExplorerOM.resources, Version=3.0.", StringComparison.OrdinalIgnoreCase)) return null;
			if (args.Name.StartsWith("Microsoft.BizTalk.Pipeline.Components.resources, Version=3.0.", StringComparison.OrdinalIgnoreCase)) return null;
			if (args.Name.StartsWith("Microsoft.ServiceModel.Channels.resources, Version=3.0.", StringComparison.OrdinalIgnoreCase)) return null;

			// nonexistent xml serializers
			if (Regex.IsMatch(args.Name, @"(Microsoft|Be\.Stateless)\..+\.XmlSerializers, Version=")) return null;

			var assemblyName = new AssemblyName(args.Name);

			var resolutionPath = DefaultProbingPaths.Concat(PrivateProbingPaths)
				.Select(path => Path.Combine(path, assemblyName.Name + ".dll"))
				.FirstOrDefault(File.Exists);
			if (resolutionPath != null)
			{
				_logAppender?.Invoke($"   Resolved assembly '{resolutionPath}'.");
				return Assembly.LoadFile(resolutionPath);
			}
			_logAppender?.Invoke($"   Could not resolve assembly '{args.Name}'.");
			return null;
		}

		private Action<string> _logAppender;
	}
}
