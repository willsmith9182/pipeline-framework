using System.Reflection;
using PipelinePlusPlus.Attributes;
using PipelinePlusPlus.Core;

namespace PipelinePlusPlus.Util
{
    public class PipelineStepDefinintion<TContext> where TContext : PipelineContext
    {
        internal PipelineStepDefinintion(PropertyInfo prop, PipelineStepAttribute attr, PipelineAction<TContext> action)
        {
            ActionName = prop.Name;
            Prop = prop;
            Action = action;
            // if we can't find one, then let's jsut defualt to 0. 
            Attr = attr ?? new PipelineStepAttribute(0);
        }

        public PropertyInfo Prop { get; private set; }
        public PipelineStepAttribute Attr { get; private set; }
        public PipelineAction<TContext> Action { get; private set; }
        public string ActionName { get; set; }
    }
}