using CommentTranslator.Parsers;
using System.Collections.Generic;

namespace CommentTranslator.Util
{
    public class CommentParserHelper
    {
        private static Dictionary<string, ICommentParser> _dictionary = new Dictionary<string, ICommentParser>();

        public static ICommentParser GetCommentParser(string lang)
        {
            if (!_dictionary.ContainsKey(lang))
            {
                _dictionary.Add(lang, CreateCommentParser(lang));
            }

            return _dictionary[lang];
        }

        private static ICommentParser CreateCommentParser(string lang)
        {
            switch (lang.ToLower())
            {
                case "csharp":
                    return new CSharpCommentParser();
                case "c/c++":
                    return new CppCommentParser();
                case "basic":
                    return new VBCommentParser();
                case "css":
                    return new CssCommentParser();
                case "python":
                    return new PythonCommentParser();
                case "jscript":
                case "typescript":
                    return new JavaScriptCommentParser();
                case "xml":
                    return new XmlCommentParser();
                case "xaml":
                    return new XamlCommentParser();
                case "html":
                case "htmlx":
                case "htmlxprojection":
                    return new HtmlCommentParser();
                case "f#":
                    return new FSharpCommentParser();
                case "razorcsharp":
                    return new RazorCommentParser();
            }

            return null;
        }
    }
}
