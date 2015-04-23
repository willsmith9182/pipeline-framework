using System;
using System.Collections.Generic;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.DynamicConfig;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Exceptions;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Modules.Mananger
{
    public class PipelineModuleMananger : IPipelineModuleManager
    {
        public void RegisterDynamicModules<TPipeline, TContext>(
            TPipeline pipeline,
            PipelineDynamicModuleConfig<TContext> pipelineDynamicModuleConfig,
            EventHandler<PipelineModuleInitializingEventArgs> initializingHandler,
            EventHandler<PipelineModuleInitializedEventArgs> initializedHandler)
            where TPipeline : PipelineSteps
            where TContext : PipelineStepContext
        {
            // loop through dynamically loaded modules and create instances of each
            foreach (var module in pipelineDynamicModuleConfig.Modules)
            {
                try
                {
                    var moduleType = Type.GetType(module.Type);

                    if (moduleType == null)
                        throw new PipelineConfigException(
                            string.Format("Module '{0}' unable to load defined type: '{1}'", module.Name, module.Type));

                    if (FireInitializingEvent(pipeline, module, initializingHandler)) continue;

                    var instance = Activator.CreateInstance(moduleType) as DynamicPipelineModule<TPipeline, TContext>;
                    if (instance == null) continue;

                    instance.Register(pipeline, module.Parameters);

                    FireInitializedEvent(pipeline, module, initializedHandler);
                }
                catch (PipelineConfigException e)
                {
                    // log, go bang do what now....
                }
            }
        }

        public void RegisterModules<TPipeline, TContext>(
            TPipeline pipeline,
            IEnumerable<PipelineModule<TPipeline, TContext>> modules,
            EventHandler<PipelineModuleInitializingEventArgs> initializingHandler,
            EventHandler<PipelineModuleInitializedEventArgs> initializedHandler)
            where TPipeline : PipelineSteps
            where TContext : PipelineStepContext
        {
            foreach (var module in modules)
            {
                var modType = module.GetType();

                // fires event and returns flag to skip module or to load module
                if (FireInitializingEvent(pipeline, modType, initializingHandler)) continue;
                try
                {
                    module.Register(pipeline);
                }
                catch (Exception)
                {
                    // if your module fail to bind to the steps, sod it. 
                    // should really store in the pipeline the state of modules loaded and executed
                    //and the modules that fialed to laod along with the modules that failed to execute
                    // attach the exception to the failed to execue module. 
                }

                FireInitializedEvent(pipeline, modType, initializedHandler);
            }
        }

        private bool FireInitializingEvent<TPipeline>(TPipeline pipeline, ModuleConfig module,
            EventHandler<PipelineModuleInitializingEventArgs> handler) where TPipeline : PipelineSteps
        {
            var args = new PipelineModuleInitializingEventArgs(pipeline.PipelineName, module);
            return FireInitializingEvent(handler, args);
        }

        private bool FireInitializingEvent<TPipeline>(TPipeline pipeline, Type moduleType,
            EventHandler<PipelineModuleInitializingEventArgs> handler) where TPipeline : PipelineSteps
        {
            var args = new PipelineModuleInitializingEventArgs(pipeline.PipelineName, moduleType.Name,
                moduleType.AssemblyQualifiedName);
            return FireInitializingEvent(handler, args);
        }

        private bool FireInitializingEvent(EventHandler<PipelineModuleInitializingEventArgs> handler,
            PipelineModuleInitializingEventArgs args)
        {
            if (handler == null) return false;
            handler(this, args);
            return args.Cancel;
        }

        private void FireInitializedEvent<TPipeline>(TPipeline pipeline, ModuleConfig module,
            EventHandler<PipelineModuleInitializedEventArgs> handler) where TPipeline : PipelineSteps
        {
            var args = new PipelineModuleInitializedEventArgs(pipeline.PipelineName, module);
            FireInitializedEvent(handler, args);
        }

        private void FireInitializedEvent<TPipeline>(TPipeline pipeline, Type moduleType,
            EventHandler<PipelineModuleInitializedEventArgs> handler) where TPipeline : PipelineSteps
        {
            var args = new PipelineModuleInitializedEventArgs(pipeline.PipelineName, moduleType.Name,
                moduleType.AssemblyQualifiedName);
            FireInitializedEvent(handler, args);
        }

        private void FireInitializedEvent(EventHandler<PipelineModuleInitializedEventArgs> handler,
            PipelineModuleInitializedEventArgs args)
        {
            if (handler == null) return;
            handler(this, args);
        }
    }
}