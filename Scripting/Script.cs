using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace EOHax.Scripting
{
	public class Script : MarshalByRefObject
	{
		public List<string> referencedAssemblies;
		public Assembly assembly;
		public CompilerResults results;

		public string Source { get; private set; }
		public string Language { get; private set; }
		public bool Compiled { get; set; }

		public CompilerErrorCollection Errors
		{
			get
			{
				return (results != null) ? results.Errors : null;
			}
		}
		
		public static Script LoadFromFile(string fileName)
		{
			int dotPosition = fileName.LastIndexOf('.');

			if (dotPosition < 0)
				throw new Exception("Script filename must have an extension");

			return new Script(CodeDomProvider.GetLanguageFromExtension(fileName.Substring(dotPosition)), File.ReadAllText(fileName));
		}
		
		public static Script LoadFromString(string language, string script)
		{
			return new Script(language, script);
		}
		
		public Script(string language, string source)
		{
			this.Language = language;
			this.Source = source;
			this.referencedAssemblies = new List<string>();
            referencedAssemblies.Add("System.dll");
		}
	}
}
