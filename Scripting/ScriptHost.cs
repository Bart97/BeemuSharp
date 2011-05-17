using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

namespace EOHax.Scripting
{
	/// <summary>
	/// Script hosting object
	/// </summary>
	public class ScriptHost : MarshalByRefObject, IDisposable
	{
		public Random rng = new Random();
		internal Dictionary<string, CompilerInfo> compilerInfo;
		internal Dictionary<string, CodeDomProvider> compilers;
		public AppDomain domain;

		/// <summary>
		/// Factory to create a ScripHost attached to an anonymous AppDomain.
		/// </summary>
		public static ScriptHost Create()
		{
			AppDomain domain = AppDomain.CreateDomain("EOHax_ScriptHost_" + System.Guid.NewGuid());
			object[] args = { domain };
			return domain.CreateInstanceAndUnwrap(typeof(ScriptHost).Assembly.FullName, typeof(ScriptHost).FullName, false, 0, null, args, null, null) as ScriptHost;
		}
		
		public ScriptHost(AppDomain domain)
		{
			this.domain = domain;
			compilerInfo = new Dictionary<string, CompilerInfo>();
			compilers = new Dictionary<string, CodeDomProvider>();

			foreach (CompilerInfo info in CodeDomProvider.GetAllCompilerInfo())
			{
				foreach (string language in info.GetLanguages())
				{
					compilerInfo[language] = info;
				}
			}
		}
		
		/// <summary>
		/// Compiles a Script object to allow it to be Executed
		/// </summary>
		/// <param name="script">Script object to be compiled</param>
		public void Compile(Script script)
		{
			if (!compilerInfo.ContainsKey(script.Language))
				throw new Exception("Unknown or unsupported language");

			string providerName = compilerInfo[script.Language].CodeDomProviderType.FullName;
			CodeDomProvider provider;

			if (!compilers.ContainsKey(providerName))
			{
				provider = compilerInfo[script.Language].CreateProvider();
				compilers.Add(providerName, provider);
			}
			else
			{
				provider = compilers[providerName];
			}
			
			CompilerParameters parameters = new CompilerParameters(script.referencedAssemblies.ToArray())
			{
				GenerateExecutable = false,
				GenerateInMemory = true,
				OutputAssembly = rng.Next().ToString() + ".dll"
			};
			
			script.results = provider.CompileAssemblyFromSource(parameters, script.Source);

			script.Compiled = !script.results.Errors.HasErrors;

			if (script.Compiled)
			{
				script.assembly = script.results.CompiledAssembly;
			}
		}
		
		/// <summary>
		/// Executes a Script object
		/// </summary>
		/// <param name="script"></param>
		public object Execute(Script script)
		{
            return Execute(script, null);
        }
        public object Execute(Script script, Object obj)
        {
			if (!script.Compiled)
				throw new Exception("Script is not compiled");
			
			Console.WriteLine("Execute() in {0}", AppDomain.CurrentDomain.FriendlyName);

			MethodInfo entry = null;

			foreach (var type in script.assembly.GetTypes())
			{
				foreach (var method in type.GetMethods())
				{
					if (method.IsStatic && method.Name == "ScriptMain")
						entry = method;
				}
			}

			if (entry == null)
				throw new EntryPointNotFoundException("ScriptMain not found");

			Console.WriteLine("entry = {0}", entry);
			Console.WriteLine("assembly = {0}", script.assembly.FullName);

			return entry.Invoke(obj, null);
		}
		
		public string[] GetLanguages()
		{
			string[] languages = new string[compilerInfo.Count];
			compilerInfo.Keys.CopyTo(languages, 0);
			return languages;
		}

		public void Dispose()
		{
			AppDomain.Unload(domain);
		}
	}
}
