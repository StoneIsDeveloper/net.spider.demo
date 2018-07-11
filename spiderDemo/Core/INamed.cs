using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace spiderDemo.Core
{
    /// <summary>
    /// 名称接口定义
    /// </summary>
    public interface INamed
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
    }

    /// <summary>
    /// 名称的抽象
    /// </summary>
    public abstract class Named : INamed
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        public Named()
        {
            //var type = GetType();
            //var nameAttribute = type.GetCustomAttribute<TaskName>();
            //Name = nameAttribute != null ? nameAttribute.Name : type.Name;
        }
    }
}
