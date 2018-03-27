//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Data Access Application Block
// Tinet Componets
//===============================================================================
// Author: Magno Cardona Heck m@vs.cl
// Component: Globals 
//===============================================================================


using System;

namespace library.CSharpInformation {
    /// <summary>
    /// Emulates Information from VB
    /// </summary>
    public class Information {

        /// <summary>
        /// Is numeric
        /// </summary>
        /// <param name="expression">object expression</param>
        /// <returns>bool if numeric</returns>
        public static bool IsNumeric(object expression) {

            IConvertible convertible=null;
            TypeCode typecode;
            string text;
            double num;
            bool result=false;
            char chr;

            if((expression as IConvertible)!=null) {
                convertible=(IConvertible)expression;
            }

            if(convertible==null) {
                if((expression as char[])!=null) {
                    expression=new string((char[])expression);
                } else {
                    return false;
                }
            }

            typecode=convertible.GetTypeCode();

            if(typecode==TypeCode.String||typecode==TypeCode.Char) {
                text=convertible.ToString(null);

                try {
                    for(int i=0; i<text.Length; i++) {
                        chr=Convert.ToChar(text.Substring(i, 1));

                        if(char.IsNumber(chr)) {
                            result=true;
                        } else if((int)chr>='a'&&(int)chr<='f') {
                            result=true;
                        } else if((int)chr>='A'&&(int)chr<='F') {
                            result=true;
                        } else {
                            result=false;
                        }

                        if(result==false) {
                            break;
                        }
                    }
                } catch(Exception) {
                    return false;
                }

                if(result==false) {
                    result=double.TryParse(text,
                        System.Globalization.NumberStyles.Any, null, out num);
                }
            }

            if(result==false) {
                result=IsNumericTypeCode(typecode);
            }

            return result;
        }

        internal static bool IsNumericTypeCode(TypeCode typeCode) {
            bool result=false;

            switch(typeCode) {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    result=true;
                    break;

                default:
                    result=false;
                    break;
            }

            return result;
        }
    }
}