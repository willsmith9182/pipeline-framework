﻿using PipelinePlusPlus.Core.Modules;

namespace PipelineFramework.Tests.TestData.Builder
{
    internal class ParamlessModule : PipelineModule<TestPipeline, TestPipelineStepContext>
    {
        public ParamlessModule()
            : base("ParamlessModule")
        {
        }

        public override void ExecuteModule(TestPipelineStepContext cxt)
        {
            // do nothing.
        }

        public override void Register(TestPipeline pipeline)
        {
            // does not register
        }
    }
}
