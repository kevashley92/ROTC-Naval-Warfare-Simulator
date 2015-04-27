/**
 *
 */

using System.Collections.Generic;

public class TestSuite {

	protected System.Collections.Generic.IList<UnitTest> Tests { get; set; }

	public TestSuite() {
		Tests = new List<UnitTest>();
	}

	public System.Collections.Generic.IList<TestResult> RunTests() {
		List<TestResult> results = new List<TestResult>();
		foreach (UnitTest test in Tests) {
			results.AddRange(test.RunAllTests());
		}
		return results;
	}

	public void AddTest(UnitTest test) {
		if (test != null) {
			Tests.Add(test);
		}
	}

	public static string DisplayResults(IList<TestResult> results) {
		List<TestResult> passing = GetPassingTestResults(results);
		List<TestResult> failing = GetFailingTestResults(results);
		System.Text.StringBuilder ret = new System.Text.StringBuilder();

		int classNameLength = 20;
		int methodNameLength = 35;

		ret.AppendLine("--------------------------------------------------------------------------------------------------------------------");
		ret.AppendLine("FAILING");
		ret.AppendLine("--------------------------------------------------------------------------------------------------------------------");
		ret.AppendLine("SEVERITY   CLASS               METHOD                             COMMENT");
		ret.AppendLine("====================================================================================================================");
		foreach (TestResult result in failing) {
			ret.Append(System.String.Format("{0:D2}         ", result.Severity));
			ret.Append(TrimStringToLength(result.ClassName, classNameLength, true));
			ret.Append(TrimStringToLength(result.MethodName, methodNameLength, true));
			ret.AppendLine(result.Message);
		}
		ret.AppendLine();
		ret.AppendLine("--------------------------------------------------------------------------------------------------------------------");
		ret.AppendLine("PASSING");
		ret.AppendLine("--------------------------------------------------------------------------------------------------------------------");
		ret.AppendLine("SEVERITY   CLASS               METHOD                             COMMENT");
		ret.AppendLine("====================================================================================================================");
		foreach (TestResult result in passing) {
			ret.Append(System.String.Format("{0:D2}         ", result.Severity));
			ret.Append(TrimStringToLength(result.ClassName, classNameLength, true));
			ret.Append(TrimStringToLength(result.MethodName, methodNameLength, true));
			ret.AppendLine(result.Message);
		}

		return ret.ToString();
	}

	public static string TrimStringToLength(string str, int len, bool right) {
		if (str.Length > len) {
			return str.Substring(0, len);
		} else {
			if (right) {
				return str.PadRight(len, ' ');
			} else {
				return str.PadLeft(len, ' ');
			}
		}
	}

	public static List<TestResult> GetPassingTestResults(IList<TestResult> results) {
		List<TestResult> ret = new List<TestResult>();
		foreach (TestResult result in results) {
			if (result.Success) {
				ret.Add(result);
			}
		}
		return ret;
	}

	public static List<TestResult> GetFailingTestResults(IList<TestResult> results) {
		List<TestResult> ret = new List<TestResult>();
		foreach (TestResult result in results) {
			if (!result.Success) {
				ret.Add(result);
			}
		}
		return ret;
	}

}
