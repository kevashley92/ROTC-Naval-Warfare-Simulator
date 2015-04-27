
using System.Collections.Generic;

public class Example {

	public static void Main() {
		TestSuite suite = new TestSuite();
		suite.AddTest(new ExampleUnitTest());
		IList<TestResult> results = suite.RunTests();
		System.Console.WriteLine(TestSuite.DisplayResults(results));
	}

	public class ExampleUnitTest : UnitTest {

		protected int i { get; set; }

		public override void SetUp() {
			base.SetUp();
			i = 0xABC1;
		}

		protected override TestResult ExampleTestMethodFailure() {
			TestResult ret = base.ExampleTestMethodFailure();
			ret.ClassName = "ExampleUnitTest";
			return ret;
		}

		protected override TestResult ExampleTestMethodSuccess() {
			TestResult ret = base.ExampleTestMethodSuccess();
			ret.ClassName = "ExampleUnitTest";
			ret.Message = ret.Message; // + "+ i = " + i;
			return ret;
		}

		protected TestResult ExampleTestSetUp() {
			return new TestResult(
					1,
					i == 0xABC1,
					"ExampleUnitTest",
					"ExampleTestSetUp",
					"int i should be set to reflect SetUp being called"
					);
		}

		public override System.Collections.Generic.IList<TestResult> RunAllTests() {
			IList<TestResult> ret = new List<TestResult>();

			ret.Add(ExampleTestMethodSuccess());
			ret.Add(ExampleTestMethodFailure());
			ret.Add(ExampleTestSetUp());

			return ret;
		}
	}

}
