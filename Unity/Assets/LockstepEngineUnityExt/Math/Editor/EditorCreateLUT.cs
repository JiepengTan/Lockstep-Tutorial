#if UNITY_EDITOR
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Lockstep.Math
{
    public static class EditorCreateLUT
    {
	    [MenuItem("LPEngine/Math/CreateLUTAsin")]
        static void  CreateLUTAsin()
        {
            string fileName = Application.dataPath + "/Lockstep.Math/LUT/LUTAsin.cs";
            const int count = 1024;
            const int percision = 10000;
            string content = @"using System;
using Lockstep.Math;
namespace Lockstep.Math
{
	public static class LUTAsin
	{
		public static readonly int COUNT;
		public static readonly int HALF_COUNT;
		public static readonly int[] table;
		static LUTAsin()
		{
			COUNT = $COUNT_VAL;
			HALF_COUNT = COUNT >> 1;
			table = new int[]
			{
$ALL_VALUES
			};
		}
	}
}";
            
            //public static LFloat LMath.Asin(LFloat val)
            //{
            //    int num = (int) (val._val * (long) LUTAsin.HALF_COUNT / LFloat.Precision) +
            //              LUTAsin.HALF_COUNT;
            //    num = Mathf.Clamp(num, 0, LUTAsin.COUNT);
            //    return new LFloat(true,(long) LUTAsin.table[num] / 10);
            //}
            StringBuilder sb = new StringBuilder();
            //
            string prefix = "\t\t\t\t";
            var step = 2.0f / count;
            for (int i = 0; i <= count; i++)
            {
	            //-1~1
	            int val = (int) (Mathf.Asin(Mathf.Clamp(-1.0f + step * i,-1f,1f)) * percision);
	            if (i == count)
	            {
		            sb.Append(prefix + val.ToString());
	            }
	            else
	            {
		            sb.AppendLine(prefix + val + ",");
	            }
            }
            
            content = content.Replace("$COUNT_VAL",count.ToString())
	            .Replace("$ALL_VALUES", sb.ToString());
            //save to files
            File.WriteAllText(fileName,content);
			AssetDatabase.Refresh();
        }
    }
}
#endif