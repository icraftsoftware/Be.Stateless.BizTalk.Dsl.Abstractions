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
					SystemProbingFolderPaths = Array.Empty<string>();
				}
				else
				{
					var installPath = (string) btsKey.GetValue("InstallPath");
					SystemProbingFolderPaths = new[] {
						installPath,
						Path.Combine(installPath, @"Developer Tools"),
						Path.Combine(installPath, @"SDK\Utilities\PipelineTools")
					};
				}
			}
			Instance = new BizTalkAssemblyResolver();
		}

		internal static BizTalkAssemblyResolver Instance { get; }

		private static string[] SystemProbingFolderPaths { get; }

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public static void Register(Action<string> logAppender, params string[] probingFolderPaths)
		{
			Instance._logAppender = logAppender;
			if (!SystemProbingFolderPaths.Any()) logAppender?.Invoke("System probing folder paths to BizTalk Developer Tools and Pipeline Tools could not be found.");
			AddProbingFolderPaths(probingFolderPaths ?? Array.Empty<string>());
			AppDomain.CurrentDomain.AssemblyResolve += Instance.OnAssemblyResolve;
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		public static void AddProbingFolderPaths(params string[] probingFolderPaths)
		{
			Instance.UserProbingFolderPaths = probingFolderPaths?.Where(p => !string.IsNullOrWhiteSpace(p))
					.SelectMany(jp => jp.Split(';').Where(p => !string.IsNullOrWhiteSpace(p)))
					.Distinct()
					.ToArray()
				?? Enumerable.Empty<string>();
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public static void Unregister()
		{
			AppDomain.CurrentDomain.AssemblyResolve -= Instance.OnAssemblyResolve;
			Instance._logAppender = null;
		}

		private BizTalkAssemblyResolver() { }

		internal IEnumerable<string> UserProbingFolderPaths { get; private set; }

		private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
		{
			// nonexistent resource assemblies
			if (args.Name.StartsWith("Microsoft.BizTalk.ExplorerOM.resources, Version=", StringComparison.OrdinalIgnoreCase)) return null;
			if (args.Name.StartsWith("Microsoft.BizTalk.Pipeline.Components.resources, Version=", StringComparison.OrdinalIgnoreCase)) return null;
			if (args.Name.StartsWith("Microsoft.ServiceModel.Channels.resources, Version=", StringComparison.OrdinalIgnoreCase)) return null;

			// nonexistent xml serializers
			if (Regex.IsMatch(args.Name, @"(Microsoft|Be\.Stateless)\..+\.XmlSerializers, Version=")) return null;

			var assemblyName = new AssemblyName(args.Name);

			var resolutionPath = SystemProbingFolderPaths.Concat(UserProbingFolderPaths)
				.Select(
					path => {
						var probedPath = Path.Combine(path, assemblyName.Name + ".dll");
						_logAppender?.Invoke($"   Probing '{probedPath}'.");
						return probedPath;
					})
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
