using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spiderDemo.Extension.Model.Attibute
{
    /// <summary>
    /// 实体选择器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntitySelector : Selector
    {
        /// <summary>
        /// 从最终解析到的结果中取前 Take 个实体
        /// </summary>
        public int Take { get; set; } = -1;

        /// <summary>
        /// 设置 Take 的方向, 默认是从头部取
        /// </summary>
        public bool TakeFromHead { get; set; } = true;
    }
}
