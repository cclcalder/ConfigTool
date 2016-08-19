using System;
using System.Collections;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

internal class DynamicExpressionsCompiler<T>
{
	private Type ListType;

	private IEnumerable<T> List;

	private static Dictionary<string, MethodInfo> WhereCache;

	private static Dictionary<Type, Dictionary<string, MethodInfo>> SelectCache;

	private string FullTypeName
	{
		get
		{
			return this.ListType.FullName;
		}
	}

	static DynamicExpressionsCompiler()
	{
		DynamicExpressionsCompiler<T>.WhereCache = new Dictionary<string, MethodInfo>();
		DynamicExpressionsCompiler<T>.SelectCache = new Dictionary<Type, Dictionary<string, MethodInfo>>();
	}

	public DynamicExpressionsCompiler(IEnumerable<T> list) : this(list, typeof(T))
	{
	}

	public DynamicExpressionsCompiler(IEnumerable<T> list, Type listType)
	{
		this.List = list;
		this.ListType = listType;
	}

	private MethodInfo CompileMethod<K>(string method, string condition, string expression)
	{
		string str = this.CreateCode<K>(method, condition, expression);
		Compiler compiler = new Compiler();
		compiler.Reference(this.ListType);
		return compiler.CompileClass(str).GetMethod("Run");
	}

    private string CreateCode<K>(string method, string condition, string expression)
    {
        object[] fullTypeName;
        StringBuilder stringBuilder = new StringBuilder();
        Type[] listType = new Type[] { this.ListType, typeof(K) };
        IEnumerable<string> @namespace = 
            from t in listType
            select t.Namespace;
        string[] strArrays = new string[] { "System", "System.Linq", "System.Collections", "System.Collections.Generic" };
        foreach (string str in @namespace.Concat<string>(strArrays).Distinct<string>())
        {
            fullTypeName = new object[] { str };
            stringBuilder.AddFormattedLine("using {0};", fullTypeName);
        }
        stringBuilder.AppendLine("public static class Class");
        stringBuilder.AppendLine("{");
        fullTypeName = new object[] { this.FullTypeName, typeof(K).FullName };
        stringBuilder.AddFormattedLine("public static IEnumerable<{1}> Run(IEnumerable<{0}> list)", fullTypeName);
        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine("foreach (var each in list)");
        stringBuilder.AppendLine("{");
        if (condition.HasValue())
        {
            fullTypeName = new object[] { condition };
            stringBuilder.AddFormattedLine("if ({0})", fullTypeName);
        }
        fullTypeName = new object[] { expression };
        stringBuilder.AddFormattedLine("yield return {0};", fullTypeName);
        stringBuilder.AppendLine("}");
        stringBuilder.AppendLine("}");
        stringBuilder.AppendLine("}");
        return stringBuilder.ToString();
    }

	internal IEnumerable<K> Select<K>(string query)
	{
		Dictionary<string, MethodInfo> strs;
		lock (DynamicExpressionsCompiler<T>.SelectCache)
		{
			if (!DynamicExpressionsCompiler<T>.SelectCache.ContainsKey(typeof(K)))
			{
				strs = new Dictionary<string, MethodInfo>();
				DynamicExpressionsCompiler<T>.SelectCache.Add(typeof(K), strs);
			}
			else
			{
				strs = DynamicExpressionsCompiler<T>.SelectCache[typeof(K)];
			}
		}
		lock (strs)
		{
			if (!strs.ContainsKey(query))
			{
				strs.Add(query, this.CompileMethod<K>("Select", null, query));
			}
		}
		IEnumerable enumerable = this.List.Cast(this.ListType);
		MethodInfo item = strs[query];
		object[] objArray = new object[] { enumerable };
		object obj = item.Invoke(null, objArray);
		return (obj as IEnumerable).Cast<K>();
	}

	internal IEnumerable<T> Where(string criteria)
	{
		string str = string.Concat(this.ListType.FullName, "|", criteria);
		lock (DynamicExpressionsCompiler<T>.WhereCache)
		{
			if (!DynamicExpressionsCompiler<T>.WhereCache.ContainsKey(str))
			{
				MethodInfo methodInfo = this.CompileMethod<T>("Where", criteria, "each");
				DynamicExpressionsCompiler<T>.WhereCache.Add(str, methodInfo);
			}
		}
		IEnumerable enumerable = this.List.Cast(this.ListType);
		MethodInfo item = DynamicExpressionsCompiler<T>.WhereCache[str];
		object[] objArray = new object[] { enumerable };
		object obj = item.Invoke(null, objArray);
		return (obj as IEnumerable).Cast<T>();
	}
}