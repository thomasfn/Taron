using System;

using Taron.Model;

namespace Taron.Translator
{
    /// <summary>
    /// Contains utility methods for translating between a Model and a .Net object
    /// </summary>
    public static class Translate
    {
        public static object Deserialise(ValueNode node, Type expected = null, TranslateOptions tOpts = null)
        {
            if (tOpts == null) tOpts = TranslateOptions.Default;
            object obj;
            tOpts.Deserialise(expected, node, out obj);
            return obj;
        }

        public static void Populate(object obj, ValueNode node, TranslateOptions tOpts = null)
        {
            if (tOpts == null) tOpts = TranslateOptions.Default;
            tOpts.Populate(obj, node);
        }

    }
}
