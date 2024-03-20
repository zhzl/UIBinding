using Mono.Cecil.Cil;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Reflection;

namespace UIBinding
{
    public static class BindingInjectTool
    {
        class CustomResolver : DefaultAssemblyResolver
        {
            public void Register(AssemblyDefinition assembly)
            {
                this.RegisterAssembly(assembly);
                this.AddSearchDirectory(Path.GetDirectoryName(assembly.MainModule.FileName));
            }
        }

        [InitializeOnLoadMethod]
        static void Startup()
        {
            EditorApplication.playModeStateChanged += OnExitingEditMode;
        }

        private static void OnExitingEditMode(PlayModeStateChange playMode)
        {
            // 自动注入，暂时先放在这里
            if (playMode != PlayModeStateChange.ExitingEditMode)
                return;

            Generate();
        }

        private static readonly string BindingDllPath = $"{Application.dataPath}/../Library/ScriptAssemblies/UIBinding.dll";

        private static string DllPath
        {
            get
            {
                var settings = BindingSettings.GetOrCreateSettings();
                switch (settings.injectPath)
                {
                    case SourcePath.RelativeToScriptAssemblies:
                        return $"{Application.dataPath}/../Library/ScriptAssemblies/{settings.injectDll}";
                    case SourcePath.RelativeToHybridCLRData:
                        return $"{Application.dataPath}/../HybridCLRData/HotUpdateDlls/Android/{settings.injectDll}";
                    case SourcePath.RelativeToAssets:
                        return $"{Application.dataPath}/{settings.injectDll}";
                }
                return string.Empty;
            }
        }

        [MenuItem("Tools/UIBinding/Generate %Q", priority = 0)]
        static void Generate()
        {
            try
            {
                EditorApplication.LockReloadAssemblies();

                if (ValidDllPath())
                {
                    var bindingAssembly = AssemblyDefinition.ReadAssembly(BindingDllPath);
                    var customResolver = new CustomResolver();
                    customResolver.Register(bindingAssembly);

                    ReaderParameters readerParameters = new ReaderParameters();
                    readerParameters.ReadSymbols = true;
                    readerParameters.SymbolReaderProvider = new Mono.Cecil.Pdb.PdbReaderProvider();
                    readerParameters.AssemblyResolver = customResolver;

                    var assembly = AssemblyDefinition.ReadAssembly(DllPath, readerParameters);
                    Inject(assembly.MainModule);

                    WriterParameters writerParameters = new WriterParameters();
                    writerParameters.WriteSymbols = true;
                    writerParameters.SymbolWriterProvider = new Mono.Cecil.Pdb.PdbWriterProvider();
                    assembly.Write(DllPath, writerParameters);

                    Debug.Log($"绑定成功");
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", $"请在 Tools/UIBinding/Settings 中指定 InjectDll!", "关闭");
                }
            }
            finally
            {
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static bool ValidDllPath()
        {
            if (string.IsNullOrEmpty(DllPath))
                return false;

            if (!File.Exists(DllPath))
                return false;

            return true;
        }

        private static void Inject(ModuleDefinition module)
        {
            foreach (var type in module.Types)
            {
                if (type.BaseType == null)
                    continue;

                if (!type.BaseType.Name.Equals(nameof(ViewModelBase), System.StringComparison.Ordinal))
                    continue;

                // 注入 Set<T> 方法
                int propertyIndex = 0;
                var baseSetMethod = module.ImportReference(typeof(ViewModelBase).GetMethod("Set", BindingFlags.NonPublic | BindingFlags.Instance));
                foreach (var property in type.Properties)
                {
                    if (!property.HasAttribute<NotifyAttribute>())
                        continue;

                    var setMethodBody = property.SetMethod.Body;
                    var cilWorker = setMethodBody.GetILProcessor();
                    var ldc_index = CreateConstInstruction(cilWorker, propertyIndex);
                    var instructionCount = setMethodBody.Instructions.Count;
                    if (instructionCount >= 9 &&  // 注入后的方法至少有 9 条指令
                        setMethodBody.Instructions[0].OpCode == OpCodes.Nop &&
                        setMethodBody.Instructions[instructionCount - 2].OpCode == OpCodes.Nop)
                    {
                        // 注入过了，只修改index
                        setMethodBody.Instructions[setMethodBody.Instructions.Count - 6] = ldc_index;
                        propertyIndex++;
                        continue;
                    }
                    else
                    {
                        // 首次注入
                        var baseSetGenericMethod = baseSetMethod.MakeGenericMethod(property.PropertyType);
                        var callBaseSetMethod = cilWorker.Create(OpCodes.Call, baseSetGenericMethod);
                        var ldarg0 = cilWorker.Create(OpCodes.Ldarg_0);
                        var ldarg1 = cilWorker.Create(OpCodes.Ldarg_1);
                        var propertyName = cilWorker.Create(OpCodes.Ldstr, property.Name);
                        var pop = cilWorker.Create(OpCodes.Pop);
                        cilWorker.InsertBefore(setMethodBody.Instructions[0], cilWorker.Create(OpCodes.Nop));
                        cilWorker.InsertBefore(setMethodBody.Instructions[setMethodBody.Instructions.Count - 1], ldarg0);
                        cilWorker.InsertAfter(ldarg0, ldarg1);
                        cilWorker.InsertAfter(ldarg1, ldc_index);
                        cilWorker.InsertAfter(ldc_index, propertyName);
                        cilWorker.InsertAfter(propertyName, callBaseSetMethod);
                        cilWorker.InsertAfter(callBaseSetMethod, pop);
                        cilWorker.InsertAfter(pop, cilWorker.Create(OpCodes.Nop));
                        propertyIndex++;
                    }
                }

                // 注入 Init 方法
                var propertyCount = propertyIndex;
                var baseInitMethod = module.ImportReference(typeof(ViewModelBase).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance));
                var ctorMethods = type.Methods.Where(x => x.Name == ".ctor");
                foreach (var ctorMethod in ctorMethods)
                {
                    var ctorMethodBody = ctorMethod.Body;
                    var cilWorker = ctorMethodBody.GetILProcessor();
                    var ldc_count = CreateConstInstruction(cilWorker, propertyCount);
                    var instructionCount = ctorMethodBody.Instructions.Count;
                    if (instructionCount >= 6 && // 注入后的方法至少有 6 条指令
                        ctorMethodBody.Instructions[0].OpCode == OpCodes.Nop &&
                        ctorMethodBody.Instructions[instructionCount - 2].OpCode == OpCodes.Nop)
                    {
                        // 注入过了，只修改index
                        ctorMethodBody.Instructions[ctorMethodBody.Instructions.Count - 4] = ldc_count;
                        continue;
                    }
                    else
                    {
                        var callBaseInitMethod = cilWorker.Create(OpCodes.Call, baseInitMethod);
                        var ldarg0 = cilWorker.Create(OpCodes.Ldarg_0);
                        cilWorker.InsertBefore(ctorMethodBody.Instructions[0], cilWorker.Create(OpCodes.Nop));
                        cilWorker.InsertBefore(ctorMethodBody.Instructions[ctorMethodBody.Instructions.Count - 1], ldarg0);
                        cilWorker.InsertAfter(ldarg0, ldc_count);
                        cilWorker.InsertAfter(ldc_count, callBaseInitMethod);
                        cilWorker.InsertAfter(callBaseInitMethod, cilWorker.Create(OpCodes.Nop));
                    }
                }
            }
        }

        private static Instruction CreateConstInstruction(ILProcessor cilWorker, int propertyIndex)
        {
            switch (propertyIndex)
            {
                case 0: return cilWorker.Create(OpCodes.Ldc_I4_0);
                case 1: return cilWorker.Create(OpCodes.Ldc_I4_1);
                case 2: return cilWorker.Create(OpCodes.Ldc_I4_2);
                case 3: return cilWorker.Create(OpCodes.Ldc_I4_3);
                case 4: return cilWorker.Create(OpCodes.Ldc_I4_4);
                case 5: return cilWorker.Create(OpCodes.Ldc_I4_5);
                case 6: return cilWorker.Create(OpCodes.Ldc_I4_6);
                case 7: return cilWorker.Create(OpCodes.Ldc_I4_7);
                case 8: return cilWorker.Create(OpCodes.Ldc_I4_8);
                default:
                    if (propertyIndex >= -128 && propertyIndex <= 127)
                    {
                        return cilWorker.Create(OpCodes.Ldc_I4_S, (sbyte)propertyIndex);
                    }
                    else
                    {
                        return cilWorker.Create(OpCodes.Ldc_I4, propertyIndex);
                    }
            }
        }

        private static bool HasAttribute<T>(this PropertyDefinition property) where T : System.Attribute
        {
            var attributes = property.CustomAttributes;
            foreach (var attribute in attributes)
            {
                if (attribute.AttributeType.Name.Equals(typeof(T).Name))
                    return true;
            }
            return false;
        }

        public static MethodReference MakeGenericMethod(this MethodReference self, params TypeReference[] arguments)
        {
            if (self.GenericParameters.Count != arguments.Length)
                throw new System.ArgumentException();

            var instance = new GenericInstanceMethod(self);
            foreach (var argument in arguments)
                instance.GenericArguments.Add(argument);

            return instance;
        }
    }
}
