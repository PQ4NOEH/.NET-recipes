using System;
using System.Text;
using Alba.CsCss.Style;

namespace Alba.CsCss.Altea
{
    /// <summary>
    /// Class with methods for modifying stylesheets with custom rules.
    /// </summary>
    /// <remarks>
    /// Forked by ALTEA
    /// </remarks>
    public class CssModifier : CssLoader
    {
        /// <summary>
        /// Updates all url tokens with custom Action specified, so
        /// different modifications can be made when needed.
        /// </summary>
        /// <param name="aInput">CSS stylesheet</param>
        /// <param name="baseUrl">Absolute URL of the stylesheet</param>
        /// <param name="updater">function for modifying URLs</param>
        /// <returns>modified CSS stylesheet</returns>
        public string ModifyUris(string aInput, Uri baseUrl, Func<string, string> updater)
        {
            StringBuilder aBuffer = new StringBuilder();

            CssScanner lexer = new CssScanner(aInput, 1);
            lexer.SetErrorReporter(new ErrorReporter(lexer, null, this, null));
            CssToken token = new CssToken();
            while (lexer.Next(token, true))
            {
                if (token.mType == CssTokenType.URL)
                {
                    aBuffer.Append(updater(token.mIdentStr));
                }
                else
                {
                    token.AppendToString(aBuffer);
                }
            }

            return aBuffer.ToString();
        }
    }
}
