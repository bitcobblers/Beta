// using FluentAssertions;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Beta.Tests.Demos;
//
// public class CalculatorDemo : BetaTest
// {
//     protected override void ConfigureContainer(IServiceCollection services)
//     {
//         services.AddSingleton<Calculator>();
//     }
//
//     protected override void DefineTests()
//     {
//         BasicTest("Addition Tests (basic)", AdditionInput, (test, input) =>
//         {
//             // Arrange.
//             var calculator = test.Requires<Calculator>();
//
//             // Act.
//             int result = calculator!.Add(input.A, input.B);
//
//             // Assert.
//             result.Should().Be(input.Expected);
//         });
//
//         GuidedTest("Addition Tests (guided)", AdditionInput, (test, input) =>
//         {
//             test
//                 .Arrange(test.Requires<Calculator>)
//                 .Act(calc => calc!.Add(input.A, input.B))
//                 .Assert(result =>
//                 {
//                     result.Should().Be(input.Expected);
//                 });
//         });
//     }
//
//     private static IEnumerable<Input> AdditionInput
//     {
//         get
//         {
//             return new Input[]
//             {
//                 new(1, 2, 3),
//                 new(2, 3, 5)
//             };
//         }
//     }
//
//     private record Input(int A, int B, int Expected);
//
//     public class Calculator
//     {
//         public int Add(int a, int b) => a + b;
//     }
// }