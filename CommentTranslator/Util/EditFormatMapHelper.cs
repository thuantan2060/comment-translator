using Microsoft.VisualStudio.Text.Classification;
using System.Windows.Media;

namespace CommentTranslator.Util
{
    public static class EditFormatMapHelper
    {
        public static Typeface GetTypeface(this IEditorFormatMap formatMap)
        {
            Typeface typeface = null;
            var resources = formatMap.GetProperties("Plain Text");
            
            if (resources.Contains("Typeface"))
            {
                typeface = resources["Typeface"] as Typeface;
            }

            return typeface;
        }

        public static double GetFontSize(this IEditorFormatMap formatMap)
        {
            var fontSize = 10d;
            var resources = formatMap.GetProperties("Plain Text");

            if (resources.Contains("FontRenderingSize"))
            {
                fontSize = (double)resources["FontRenderingSize"];
            }

            return fontSize;
        }
    }
}
