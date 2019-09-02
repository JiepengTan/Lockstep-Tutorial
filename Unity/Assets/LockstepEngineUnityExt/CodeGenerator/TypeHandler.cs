using System;
using System.Collections.Generic;

namespace Lockstep.CodeGenerator {
    public class TypeHandler : ITypeHandler {
        protected List<IFiledHandler> _filedHandlers = new List<IFiledHandler>();
        protected ICodeHelper _helper;
        protected string _clsCodeTemplate = "";

        public IFiledHandler[] GetFiledHandlers(){
            return _filedHandlers.ToArray();
        }

        public TypeHandler(ICodeHelper helper, FiledHandler[] handlers, string clsCodeTemplate){
            this._helper = helper;
            _filedHandlers.AddRange(handlers);
            this._clsCodeTemplate = clsCodeTemplate;
        }



        public bool CanAddType(Type t){
            return _helper.CanAddType(t);
        }

        public string DealType(Type t, List<string> filedsStrs){
            var nameSpace = _helper.GetNameSpace(t);
            var clsTypeName = _helper.GetTypeName(t);
            var compName = clsTypeName.Replace("Component", "");
            var clsFuncName = _helper.GetFuncName(t);
            var codeTemplate = _clsCodeTemplate;
            int idx = 0;
            var str = codeTemplate
                    .Replace("#NameSpace", nameSpace)
                    .Replace("#ClsName", clsTypeName)
                    .Replace("#CompName", compName)
                    .Replace("#ClsFuncName", clsFuncName)
                ;

            for (int i = 0; i < filedsStrs.Count; i++) {
                var tag = "#TYPE_HANDLE" + i;
                str = str.Replace(tag, filedsStrs[i]);
            }

            return str;
        }
    }
}