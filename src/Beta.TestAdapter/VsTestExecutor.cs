//using Microsoft.VisualStudio.TestPlatform.ObjectModel;
//using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

//namespace Beta.TestAdapter;

//[ExtensionUri(ExecutorUri)]
//public class VsTestExecutor : VsTestAdapter, ITestExecutor
//{
//    public const string ExecutorUri = "executor://BetaTestExecutor/v1";

//    /// <inheritdoc />
//    public void RunTests(IEnumerable<TestCase>? tests,
//                         IRunContext? runContext,
//                         IFrameworkHandle? frameworkHandle)
//    {
//        Reset(runContext, frameworkHandle);
//        PrintBanner();
//    }

//    /// <inheritdoc />
//    public void RunTests(IEnumerable<string>? sources,
//                         IRunContext? runContext,
//                         IFrameworkHandle? frameworkHandle)
//    {
//        Reset(runContext, frameworkHandle);
//        PrintBanner();
//    }

//    /// <inheritdoc />
//    public void Cancel()
//    {
//    }
//}