using System;
using PipelinePlusPlus.Core.Context;
using PipelinePlusPlus.Core.DynamicConfig;
using PipelinePlusPlus.Core.EventArgs;
using PipelinePlusPlus.Core.Modules.Mananger;
using PipelinePlusPlus.Core.Steps;

namespace PipelinePlusPlus.Core.Discovery
{
    internal class DiscoveryFactory : IDiscoveryFactory
    {
        private readonly IDynamicModuleConfig _dynamicModuleConfig;
        private readonly IPipelineModuleManager _moduleManager;
        // non ioc based. 
        // ncrunch: no coverage start
        public DiscoveryFactory() : this(new DynamicModuleConfig(), new PipelineModuleMananger())
        {
        }

        // ncrunch: no coverage end
        public DiscoveryFactory(IDynamicModuleConfig dynamicModuleConfig, IPipelineModuleManager moduleManager)
        {
            _dynamicModuleConfig = dynamicModuleConfig;
            _moduleManager = moduleManager;
        }

        public IPipelineDiscovery<TPipeline, TContext> GetDiscovery<TPipeline, TContext>(TPipeline pipelineSteps, EventHandler<PipelineModuleInitializedEventArgs> moduleInitializedHandler, EventHandler<PipelineModuleInitializingEventArgs> moduleInitializingHandler) where TContext : PipelineStepContext where TPipeline : PipelineSteps
        {
            return new PipelineDiscovery<TPipeline, TContext>(_dynamicModuleConfig, _moduleManager, pipelineSteps, moduleInitializedHandler, moduleInitializingHandler);
        }
    }
}