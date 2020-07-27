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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Be.Stateless.BizTalk.Dsl
{
	/// <summary>
	/// Interface that is used to build fluent interfaces and hides methods declared by <see cref="object"/> from Intellisense.
	/// </summary>
	/// <remarks>
	/// Code that consumes implementations of this interface should expect one of two things:
	/// <list type="number">
	/// <item>
	/// When referencing the interface from within the same solution (project reference), you will still see the methods this
	/// interface is meant to hide.
	/// </item>
	/// <item>
	/// When referencing the interface through the compiled output assembly (external reference), the standard Object methods
	/// will be hidden as intended.
	/// </item>
	/// </list>
	/// See http://bit.ly/ifluentinterface for more information.
	/// </remarks>
	/// <seealso href="http://bit.ly/ifluentinterface">How to hide System.Object members from your interfaces</seealso>
	[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public DSL API.")]
	[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public DSL API.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IFluentInterface
	{
		/// <summary>
		/// Redeclaration that hides the <see cref="object.Equals(object)"/> method from Intellisense.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		bool Equals(object other);

		/// <summary>
		/// Redeclaration that hides the <see cref="object.GetHashCode()"/> method from Intellisense.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		int GetHashCode();

		/// <summary>
		/// Redeclaration that hides the <see cref="object.GetType()"/> method from Intellisense.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
		Type GetType();

		/// <summary>
		/// Redeclaration that hides the <see cref="object.ToString()"/> method from Intellisense.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		string ToString();
	}
}
