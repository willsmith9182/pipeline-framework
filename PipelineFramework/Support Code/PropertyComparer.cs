using System.Collections.Generic;
using System.Reflection;
using Pipeline.Attributes;

namespace Pipeline.Support_Code
{
    public class PropertyComparer : IComparer<PropertyInfo>
    {
        int IComparer<PropertyInfo>.Compare(PropertyInfo x, PropertyInfo y)
        {
            var ret = 0;

            var orderX = 0;
            var orderY = 0;

            var attrX = x.GetCustomAttributes(typeof (PipelineEventAttribute), true);
            var attrY = y.GetCustomAttributes(typeof (PipelineEventAttribute), true);

            if (attrX.Length > 0)
            {
                var attr = (PipelineEventAttribute) attrX[0];
                orderX = attr.Order;
            }

            if (attrY.Length > 0)
            {
                var attr = (PipelineEventAttribute) attrY[0];
                orderY = attr.Order;
            }

            if (orderX < orderY)
                ret = -1;
            else if (orderX > orderY)
                ret = 1;

            return ret;
        }
    }
}