using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PipelinePlusPlus.Definition;
using PipelinePlusPlus.EventArgs;

namespace PipelinePlusPlus.Core
{
    public interface IPipelineModuleManager
    {
        IPipelineModuleManager WithModuleInitializingHandler(EventHandler<PipelineModuleInitializingEventArgs> initilaizingHandler);
        IPipelineModuleManager WithModuleInitializedHandler(EventHandler<PipelineModuleInitializedEventArgs> initilaizedHandler);
        void RegisterDynamicModules<TPipeline>(TPipeline pipeline, IEnumerable<ModuleConfig> dynamicModules) where TPipeline : PipelineSteps;
        void RegisterModules<TPipeline>(TPipeline pipeline, IEnumerable<IPipelineModule<TPipeline>> modules) where TPipeline : PipelineSteps;
    }

    public class PipelineModuleMananger : IPipelineModuleManager
    {
        private EventHandler<PipelineModuleInitializedEventArgs> _moduleInitialized;
        private EventHandler<PipelineModuleInitializingEventArgs> _moduleInitializing;

        public IPipelineModuleManager WithModuleInitializingHandler(EventHandler<PipelineModuleInitializingEventArgs> initilaizingHandler)
        {
            _moduleInitializing += initilaizingHandler;
            return this;
        }

        public IPipelineModuleManager WithModuleInitializedHandler(EventHandler<PipelineModuleInitializedEventArgs> initilaizedHandler)
        {
            _moduleInitialized += initilaizedHandler;
            return this;
        }

        public void RegisterDynamicModules<TPipeline>(TPipeline pipeline, IEnumerable<ModuleConfig> dynamicModules) where TPipeline : PipelineSteps
        {
            
        }

        public void RegisterModules<TPipeline>(TPipeline pipeline, IEnumerable<IPipelineModule<TPipeline>> modules) where TPipeline : PipelineSteps
        {
            
        }
    }
}
