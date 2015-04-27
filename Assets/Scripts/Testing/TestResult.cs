
public class TestResult : System.IComparable<TestResult> {

	/**
	 * Severity level of this particular test result.
	 * 0 is the highest severity and should stop everything. (use sparingly)
	 * 1 is very high
	 * Reasonably use up to ~5
	 * If ever need to, will use enum later
	 */
	public uint Severity { get; set; }

	public bool Success { get; set; }

	/** Name of the class being tested. */
	public string ClassName { get; set; }

	/** Name of the method being tested. */
	public string MethodName { get; set; }

	/** Message to accompany unit test, useful to hint what is being tested */
	public string Message { get; set; }

	/** Constructor */
	public TestResult(uint severity, bool success, string className,
			string methodName, string message) {
		Severity = severity;
		Success = success;
		ClassName = className;
		MethodName = methodName;
		Message = message;
	}

	/**
	 * Retrieve useful information about this test result, specifically intended
	 * for printing later as part of a list of results.
	 */
	public override string ToString() {
		return System.String.Format(
				"TestResult: [{0} {1}:{2} : ({4}){3}]",
				Success, ClassName, MethodName, Message, Severity);
	}

	/**
	 * Allow for the test results to be sorted by significance.
	 * Successful tests are not as important to see as failures; otherwise, rate
	 * by severity.
	 */
	public int CompareTo(TestResult other) {
		if (Success && !other.Success) {
			return 10;
		}
		if (other.Success && !Success) {
			return -10;
		}
		return ((int)this.Severity) - ((int)other.Severity);
	}


}
