using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Extensions
{
	internal class Compiler
	{
		private List<Assembly> References = new List<Assembly>();

		public Compiler()
		{
			this.Reference<string>();
			this.Reference<IQueryable>();
			this.Reference<WarningException>();
		}

		internal Type CompileClass(string classCode)
		{
			Type type;
			CompilerParameters compilerParameter = new CompilerParameters()
			{
				GenerateExecutable = false,
				IncludeDebugInformation = true,
				GenerateInMemory = true
			};
			CompilerParameters compilerParameter1 = compilerParameter;
			Assembly[] array = this.References.Distinct<Assembly>().ToArray<Assembly>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				Assembly assembly = array[i];
				compilerParameter1.ReferencedAssemblies.Add(assembly.Location);
			}
			CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider(new Dictionary<string, string>()
			{
				{ "CompilerVersion", "v3.5" }
			});
			try
			{
				string[] strArrays = new string[] { classCode };
				CompilerResults compilerResult = cSharpCodeProvider.CompileAssemblyFromSource(compilerParameter1, strArrays);
				this.EvaluateResult(compilerResult);
				Type[] types = compilerResult.CompiledAssembly.GetTypes();
				if (types.None<Type>())
				{
					throw new Exception(string.Concat("The dynamic type for the following class has no type, also no error messages were produced by the compiler:\r\n", classCode));
				}
				type = ((IEnumerable<Type>)types).Single<Type>((Type t) => t.Name == "Class");
			}
			finally
			{
				if (cSharpCodeProvider != null)
				{
					((IDisposable)cSharpCodeProvider).Dispose();
				}
			}
			return type;
		}

		internal Type CompileMethods(string methods)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("using System;");
			stringBuilder.AppendLine("using System.Collections;");
			stringBuilder.AppendLine("using System.Linq;");
			stringBuilder.AppendLine("using System.Collections.Generic;");
			stringBuilder.AppendLine("using System.Text;");
			stringBuilder.AppendLine("using Application.Model;");
			stringBuilder.AppendLine("using Application.DataModel;");
			stringBuilder.AppendLine("using App;");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("public static class Evaluator");
			stringBuilder.AppendLine("{");
			stringBuilder.AppendLine(methods);
			stringBuilder.AppendLine("}");
			return this.CompileClass(stringBuilder.ToString());
		}

		private void EvaluateResult(CompilerResults result)
		{
			if (result.Errors.Count != 0)
			{
				List<string> strs = new List<string>();
				List<string> strs1 = new List<string>();
				foreach (CompilerError compilerError in 
					from CompilerError e in result.Errors
					where !e.IsWarning
					select e)
				{
					if (!strs1.Contains(compilerError.ErrorText))
					{
						strs1.Add(compilerError.ErrorText);
						object[] errorText = new object[] { compilerError.ErrorText, compilerError.FileName, compilerError.Line };
						strs.AddFormat("{0} ({1}:{2})", errorText);
					}
				}
				if (strs1.Any<string>())
				{
					throw new Exception("I cannot compile the dynamic assembly ", new Exception(strs.ToLinesString<string>()));
				}
			}
		}

		private IEnumerable<Assembly> GetReferences()
		{
			Assembly[] executingAssembly = new Assembly[] { Assembly.GetExecutingAssembly(), typeof(string).Assembly, typeof(IQueryable).Assembly, typeof(WarningException).Assembly };
			return executingAssembly.Distinct<Assembly>().ToList<Assembly>();
		}

		public void Reference<T>()
		{
			this.Reference(typeof(T));
		}

		public void Reference(Type type)
		{
			this.Reference(type.Assembly);
		}

		public void Reference(Assembly assembly)
		{
			if (!this.References.Contains(assembly))
			{
				this.References.Add(assembly);
			}
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			for (int i = 0; i < (int)referencedAssemblies.Length; i++)
			{
				Assembly assembly1 = Assembly.Load(referencedAssemblies[i]);
				if (!this.References.Contains(assembly1))
				{
					this.Reference(assembly1);
				}
			}
		}
	}
}