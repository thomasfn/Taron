using System;
using System.Collections.Generic;

namespace Taron.Translator
{
    /// <summary>
    /// Represents options used to translate
    /// </summary>
    public sealed class TranslateOptions
    {
        private HashSet<IModelTranslator> translators;

        /// <summary>
        /// Gets a set of default translation options
        /// </summary>
        public static TranslateOptions Default
        {
            get
            {
                TranslateOptions opts = new TranslateOptions();
                opts.AddTranslator(new PrimitiveTranslator());
                return opts;
            }
        }

        /// <summary>
        /// Initialises a new instance of the TranslateOptions class
        /// </summary>
        public TranslateOptions()
        {
            // Init
            translators = new HashSet<IModelTranslator>();
        }

        /// <summary>
        /// Adds the specified translator
        /// </summary>
        /// <param name="translator"></param>
        public void AddTranslator(IModelTranslator translator)
        {
            translators.Add(translator);
        }

        /// <summary>
        /// Finds a translator capable of translating the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="matchCap"></param>
        /// <returns></returns>
        internal IModelTranslator FindTranslator(Type type, TranslateCapability matchCap)
        {
            // Note: this does not deal with conflicts
            foreach (IModelTranslator translator in translators)
            {
                if ((translator.CanTranslate(type) & matchCap) == matchCap)
                    return translator;
            }
            return null;
        }

    }
}
