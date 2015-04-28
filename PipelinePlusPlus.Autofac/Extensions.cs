using System;
using System.Collections.Generic;
using Autofac;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Exceptions;
using PipelinePlusPlus.Core.Modules;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Autofac
{
    // pipeline plus plus autofac integration library
    // split builder out of core into seperate library. 
    // provide a binding for IBuilder to inject builder where it's needed. 
    // provide a differnt implementation of builder that registeres modules against the ioc container. 
    // modules can only be registered if they implement IPipelineModule
    // dynamic modules must implement AutofacPipelineModule 
    //              :inherits from IOCModuleBase<TKernel>
    // blaaaaaaaaah blah blah. 

    public static class Extensions
    {
        public static IPipelineBuilder<TPipeline, TContext> RegisterPipeline<TPipeline, TContext>(this ContainerBuilder containerBuilder)
        {
            return null;
        }
    }

    internal abstract class IocPipelineBuiler<TKernel>
    {
        protected readonly TKernel Kernel;

        protected IocPipelineBuiler(TKernel kernel)
        {
            Kernel = kernel;
        }
    }

    internal class AutofacPipelineBuilder : IocPipelineBuiler<ContainerBuilder>
    {
        public AutofacPipelineBuilder(ContainerBuilder kernel)
            : base(kernel)
        {
        }
    }

    public interface IPipelineBuilder<TPipeline, TContext>
    {
        IPipelineBuilder<TPipeline, TContext> OnModuleInitialize(Action<object, PipelineModuleInitializingEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnModuleInitialized(Action<object, PipelineModuleInitializedEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnModuleInitialize(Action<PipelineModuleInitializingEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnModuleInitialized(Action<PipelineModuleInitializedEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectue(Action<object, PipelineEventFiringEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectue(Action<PipelineEventFiringEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectued(Action<object, PipelineEventFiredEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnPipeLineStageExectued(Action<PipelineEventFiredEventArgs> del);
        IPipelineBuilder<TPipeline, TContext> OnPipelineError(Func<PipelineException, bool> del);
    }


    public abstract class DependentModuleBase<TKernel>
    {
        public abstract void RegisterModuleDependencies(TKernel kernel);
    }

    public abstract class AutofacPipelineModule : DependentModuleBase<ContainerBuilder>
    {
    }
}
