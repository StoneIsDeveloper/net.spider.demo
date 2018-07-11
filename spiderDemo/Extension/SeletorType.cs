using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spiderDemo.Extension
{
    /// <summary>
    /// 查询器类型
    /// </summary>
    [Flags]
    public enum SelectorType
    {
        /// <summary>
        /// XPath
        /// </summary>
        XPath,
        /// <summary>
        /// Regex
        /// </summary>
        Regex,
        /// <summary>
        /// Css
        /// </summary>
        Css,
        /// <summary>
        /// JsonPath
        /// </summary>
        JsonPath,
        /// <summary>
        /// Enviroment
        /// </summary>
        Enviroment
    }
}
