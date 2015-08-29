using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Taron.Translator
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class TaronEnumAttribute : Attribute
    {
        public TaronEnumAttribute(string typename, Type type)
        {
            EnumTranslator.EnumTypes.Add(typename, type);
        }
    }
}
