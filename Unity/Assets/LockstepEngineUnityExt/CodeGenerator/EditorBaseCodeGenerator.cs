using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Lockstep.Logging;
using Lockstep.Serialization;
using Lockstep.Util;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Lockstep.CodeGenerator {
    public class EditorBaseCodeGenerator : ICodeHelper {
        public GenInfo GenInfo;
        protected HashSet<Type> togenCodeTypes = new HashSet<Type>();
        protected HashSet<Type> needNameSpaceTypes = new HashSet<Type>();

        public EditorBaseCodeGenerator(GenInfo GenInfo){
            this.GenInfo = GenInfo;
        }

        public Type[] GetTypes(){
#if UNITY_5_3_OR_NEWER
            return typeof(Define).Assembly.GetTypes();
#else
            var path = Path.Combine(Define.BaseDirectory, GenInfo.DllRelPath);
            var assembly = Assembly.LoadFrom(path);
            return assembly.GetTypes();
#endif
        }

        public string NameSpace => GenInfo.NameSpace;

        protected string GeneratePath => Path.Combine(Define.BaseDirectory, GenInfo.GeneratePath);

        protected string GenerateFilePath => Path.Combine(GeneratePath, GenInfo.GenerateFileName);


        protected virtual void CustomRegisterTypes(){ }
        public virtual string prefix => "\t\t\t";

        protected virtual void ReflectRegisterTypes(){
            Type[] types = null;
            HashSet<Type> allTypes = new HashSet<Type>();
            types = GetTypes();
            var interfaceName = GenInfo.InterfaceName;
            foreach (var t in types) {
                if (!allTypes.Add(t)) continue;
                if (CanAddType(t)) {
                    RegisterType(t);
                }
            }
        }


        public bool CanAddType(Type t){
            if (t.IsInterface) return false;
            if (CodeGenerator.HasAttribute(t, GenInfo.IgnoreTypeAttriName)) {
                return false;
            }


            var allInterfaces = t.GetInterfaces();
            var interfaces = allInterfaces.Where((_t) => _t.Name.Equals(GenInfo.InterfaceName)).ToArray();
            if (interfaces.Length > 0) {
                return true;
            }

            return false;
        }
#if UNITY_5_3_OR_NEWER
        private const string SPLITE_CHAR = " ";
#else
        private const char SPLITE_CHAR = ' ';
#endif
        public virtual void GenerateCodeNodeData(bool isRefresh, params Type[] types){
            var ser = new CodeGenerator();
            ser.GenInfo = GenInfo;
            foreach (var handler in GenInfo.FileHandlerInfo.TypeHandler) {
                handler.Init(this);
            }

            var extensionStr = GenTypeCode(ser, new TypeHandler(
                this, GenInfo.FileHandlerInfo.TypeHandler,
                String.Join(SPLITE_CHAR, GenInfo.FileHandlerInfo.ClsCodeTemplate)
            ));
            var registerStr = GenRegisterCode(ser);
            var finalStr = GenFinalCodes(extensionStr, registerStr, isRefresh);
            SaveFile(isRefresh, finalStr);
        }

        protected string GenRegisterCode(CodeGenerator gen){
            var allGentedTypes = gen.AllGeneratedTypes;
            var prefix = "";
            var RegisterCode = GenInfo.FileHandlerInfo.RegisterCode;
            if (string.IsNullOrEmpty(RegisterCode)) return string.Empty;
            allGentedTypes.Sort((a, b) => { return GetTypeName(a).CompareTo(GetTypeName(b)); });
            StringBuilder sb = new StringBuilder();
            foreach (var t in allGentedTypes) {
                var clsFuncName = GetTypeName(t);
                var nameSpace = GetNameSpace(t);
                sb.AppendLine(string.Format(RegisterCode, prefix, clsFuncName, nameSpace));
            }

            return sb.ToString();
        }

        string GenFinalCodes(string extensionStr, string RegisterStr, bool isRefresh){
            string fileContent = string.Join(" ", GenInfo.FileHandlerInfo.FileContent);
            return fileContent
                    .Replace("#NAMESPACE", NameSpace)
                    .Replace("//#DECLARE_BASE_TYPES", RegisterStr)
                    .Replace("//#TYPES_EXTENSIONS", extensionStr)
                ;
        }


        public string GenTypeCode(CodeGenerator gen, ITypeHandler typeHandler, params Type[] types){
            List<Type> allTypes = new List<Type>();
            allTypes.AddRange(types);
            var registerTypes = GetNeedSerilizeTypes();
            allTypes.AddRange(registerTypes);
            return gen.GenTypeCode(typeHandler, allTypes.ToArray());
        }

        public void BuildProject(){
            if (!string.IsNullOrEmpty(GenInfo.ProjectFilePath)) {
                Utils.ExecuteCmd("msbuild /property:Configuration=Debug /p:WarningLevel=0 /verbosity:minimal "
                                 + GenInfo.ProjectFilePath, Define.BaseDirectory);
            }
        }

        public void HideGenerateCodes(bool isSave = true){
            var path = GenerateFilePath;
            if (!File.Exists(path)) return;
            var lines = System.IO.File.ReadAllLines(path);
            lines[0] = lines[0].Replace("//#define", "#define");
            System.IO.File.WriteAllLines(path, lines);
            if (isSave) {
#if UNITY_EDITOR
                AssetDatabase.ImportAsset(path);
                AssetDatabase.Refresh();
                UnityEngine.Debug.Log("Done");
#endif
            }
        }


        protected void SaveFile(bool isRefresh, string finalStr){ //save to file
            //Debug.LogError(GeneratePath);
            if (!Directory.Exists(GeneratePath)) {
                Directory.CreateDirectory(GeneratePath);
            }

            System.IO.File.WriteAllText(GenerateFilePath, finalStr);
            if (isRefresh) {
#if UNITY_EDITOR
                //EditorUtility.OpenWithDefaultApp(GenerateFilePath);
                AssetDatabase.Refresh();
                UnityEngine.Debug.Log("Done");
#endif
            }
        }

        protected virtual Type[] GetNeedSerilizeTypes(){
            needNameSpaceTypes.Clear();
            togenCodeTypes.Clear();
            CustomRegisterTypes();
            ReflectRegisterTypes();
            var list = togenCodeTypes.ToList();
            list.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
            return list.ToArray();
        }


        protected void RegisterType(Type type){
            togenCodeTypes.Add(type);
        }

        protected void RegisterBaseType(Type type){
            var types = ReflectionUtility.GetSubTypes(type);
            foreach (var t in types) {
                RegisterType(t);
            }
        }

        protected void RegisterTypeWithNamespace(Type type){
            needNameSpaceTypes.Add(type);
        }

        protected void RegisterBaseTypeWithNamespace(Type type){
            var types = ReflectionUtility.GetSubTypes(type);
            foreach (var t in types) {
                RegisterTypeWithNamespace(t);
            }
        }


        protected bool IsNeedNameSpace(Type t){
            return needNameSpaceTypes.Contains(t);
        }


        public string GetNameSpace(Type type){
            return type.Namespace;
        }

        public string GetTypeName(Type type, bool isWithNameSpaceIfNeed = true){
            var str = type.ToString();
            if (IsNeedNameSpace(type)) {
                return str.Replace("+", ".");
            }
            else {
                return str.Substring(str.LastIndexOf(".") + 1).Replace("+", ".");
            }
        }

        public string GetFuncName(Type type, bool isWithNameSpaceIfNeed = true){
            var str = type.ToString();
            if (IsNeedNameSpace(type)) {
                return str.Replace(".", "").Replace("+", "");
            }
            else {
                return str.Substring(str.LastIndexOf(".") + 1).Replace("+", "");
            }
        }
    }
};