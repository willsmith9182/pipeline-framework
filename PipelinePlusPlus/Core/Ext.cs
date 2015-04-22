namespace PipelinePlusPlus.Core
{
    public static class Ext
    {
        // this means that people don't need to instansiate the Step before they try and register modules. 
        // infact they can't the ctor is internal to prevent anyone creating steps. they jsut define, the pipeline will take care of the rest. 
        // this'll look like an instance method but it'll create the instance if it doesn't exist. 
        public static void RegisterModule<TContext>(this PipelineStep<TContext> step, IPipelineModule<TContext> module)
            where TContext : PipelineContext
        {
            if (step == null)
            {
                step = new PipelineStep<TContext>();
            }
            step.RegisterModule(module);
        }
    }
}