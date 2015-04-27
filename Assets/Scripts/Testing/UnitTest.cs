/**
 * Base class to be a unit test for a class or feature.
 */
public abstract class UnitTest {

	public UnitTest() {
		SetUp();
	}

	/**
	 * Perform any necessary initializations within a class to help successfully
	 * run a unit test (suite).
	 */
	public virtual void SetUp() {}

	/**
	 * Clean up any lose ends when running the tests within this unit test.
	 * This can include things like freeing up memory (if C# weren't garbage-
	 * collected), or closing connections if cross-network activity is required.
	 */
	public virtual void CleanUp() {}

	/**
	 * Run all tests within this UnitTest object.
	 * Must be implemented at a lower level because it cannot be determined
	 * which methods are to be called (at least not without reflection, and I
	 * don't really want to deal with learning and debugging that too.
	 * I think it's a bad enough idea to write my own testing framework, but
	 * what needs doing must be done. 
	 */
	public abstract System.Collections.Generic.IList<TestResult> RunAllTests();

	/**
	 * Really you can name this whatever, the only requirement is to return a
	 * TestResult. It is recommended to not make this public simply because no
	 * other classes need to call it and if it is actually time-critical or
	 * order-critical, allowing others to touch would be bad.
	 * Too scrambled-brain to make sense.
	 */
	protected virtual TestResult ExampleTestMethodSuccess() {
		return new TestResult(1, true, "UnitTest", "ExampleTestMethodSuccess",
				"successful execution");
	}

	protected virtual TestResult ExampleTestMethodFailure() {
		return new TestResult(2, false, "UnitTest", "ExampleTestMethodFailure",
				"failed test execution example");
	}

}
