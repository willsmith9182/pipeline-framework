using System;
using System.Configuration;
using Pipeline.Configuration;
using Pipeline.EventArgs;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Pipeline
{
    public class Backbone<TEvents> : IBackbone<TEvents>
        where TEvents : PipelineEvents, new()
    {
        protected Backbone(string pipelineName) // the Backbone<T,U> class should be able to call this constructor
        {
            PipelineName = pipelineName;
        }

        protected Backbone(Definition.Pipeline pipeline)
        {
            Pipeline = pipeline;
        }

        protected string PipelineName { get; private set; }
        protected Definition.Pipeline Pipeline { get; private set; }
        public event EventHandler<PipelineModuleInitializingEventArgs> PipelineModuleInitializing;
        public event EventHandler<PipelineModuleInitializedEventArgs> PipelineModuleInitialized;

        public TEvents Initialize()
        {
            var pipelineEvents = new TEvents();

            var pipeline = Pipeline ?? GetPipelineDefinition();

            foreach (var moduleItem in pipeline.Modules)
            {
                var beforeArgs = new PipelineModuleInitializingEventArgs(pipeline.Name, moduleItem.Name);
                var afterArgs = new PipelineModuleInitializedEventArgs(pipeline.Name, moduleItem.Name);

                OnPipelineModuleInitializing(beforeArgs);

                // if cancel is set move to next module
                if (beforeArgs.Cancel) continue;

                // try to resolve type of module from config
                var moduleType = Type.GetType(moduleItem.Type);

                // if module cannot be resolved to a type then move to next module
                if (moduleType == null) continue;

                var module = Activator.CreateInstance(moduleType) as IPipelineModule<TEvents>;

                // if the module type loaded does not implement required interface move to next module
                if (module == null) continue;

                // if cancelled move to next module
                if (beforeArgs.Cancel) continue;

                // actually initialize the module!
                module.Initialize(pipelineEvents, moduleItem.Parameters);

                // call after initialised
                OnPipelineModuleInitialized(afterArgs);

            }

            return pipelineEvents;
        }

        public static TEvents InitializePipeline(string pipelineName)
        {
            return new Backbone<TEvents>(pipelineName).Initialize();
        }

        public static TEvents InitializePipeline(Definition.Pipeline pipeline)
        {
            return new Backbone<TEvents>(pipeline).Initialize();
        }

        protected virtual void OnPipelineModuleInitializing(PipelineModuleInitializingEventArgs e)
        {
            if (PipelineModuleInitializing != null)
                PipelineModuleInitializing(this, e);
        }

        protected virtual void OnPipelineModuleInitialized(PipelineModuleInitializedEventArgs e)
        {
            if (PipelineModuleInitialized != null)
                PipelineModuleInitialized(this, e);
        }

        protected PipelineElement GetPipelineConfigurationElement()
        {
            return GetPipelineConfigurationElement(PipelineName);
        }

        protected PipelineElement GetPipelineConfigurationElement(string pipelineName)
        {
            var section = (PipelineFrameworkConfigurationSection)(ConfigurationManager.GetSection("pipelineFramework"));

            var pipelineElement = section.Pipelines.GetByName(pipelineName);

            if (pipelineElement == null)
                throw new ConfigurationErrorsException(
                    string.Format("Pipeline '{0}' is missing from the pipelineFramework configuration section.",
                        pipelineName));

            return pipelineElement;
        }

        protected Definition.Pipeline GetPipelineDefinition()
        {
            return GetPipelineDefinition(PipelineName);
        }

        protected Definition.Pipeline GetPipelineDefinition(string pipelineName)
        {
            var pipelineElement = GetPipelineConfigurationElement(pipelineName);
            var pipeline = new Definition.Pipeline(pipelineElement.Name)
            {
                InvokeAll = pipelineElement.InvokeAll
            };

            foreach (ProviderSettings item in pipelineElement.Modules)
                pipeline.Modules.Add(item.Name, item.Type, item.Parameters);

            return pipeline;
        }
    }
}