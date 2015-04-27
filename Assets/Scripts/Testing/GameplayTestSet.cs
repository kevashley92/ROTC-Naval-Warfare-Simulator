
using System.Collections.Generic;
using UnityEngine;

public class GameplayTestSet : MonoBehaviour {
	
	TestSuite suite;
	
	void Start() {
		suite = new TestSuite();
		suite.AddTest(new GameplayTestSetUnitTest());
	}
	
	public void GUIcallCodeToRunTests(){
		IList<TestResult> results = suite.RunTests();
		//Debug.Log(TestSuite.DisplayResults(results));
	}

	public class GameplayTestSetUnitTest : UnitTest {

		protected int i { get; set; }

		public override void SetUp() {
			base.SetUp();
			i = 0xABC1;
		}
		
		protected TestResult TestAttacking(){
			//spawn two units
			
			//have one attack the other
			
			//end turn
			
			//check damage and ammo
			
			if(false /*it works*/){
				return new TestResult(
					1,
					true,
					"GameplayTestSetUnitTest",
					"TestAttacking",
					"Attacking worked"
					);
			}
			else{
				return new TestResult(
					1,
					false,
					"GameplayTestSetUnitTest",
					"TestAttacking",
					"not implemented"
					);
			}
	
		}
		
		protected TestResult TestMovement(){
			
			//spawn one unit
			
			//have move it
			
			//end turn
			
			//check transform
			
			if(false /*it works*/){
				return new TestResult(
					1,
					true,
					"GameplayTestSetUnitTest",
					"TestMovement",
					"movement worked"
					);
			}
			else{
				return new TestResult(
					1,
					false,
					"GameplayTestSetUnitTest",
					"TestMovement",
					"not implemented"
					);
			}
	
		}
		
		protected TestResult TestWeather(){
			
			if(GlobalSettings.GetCurrentWeatherIndex() != 0 ){
				return new TestResult(
					1,
					false,
					"GameplayTestSetUnitTest",
					"TestWeather",
					"weather not starting at expected value"
					);
			}
			
			GlobalSettings.Instance ().throwWeatherChangeEvent (1);
			
			EventManager.Instance.HandleEvents(0);
			
			if(GlobalSettings.GetCurrentWeatherIndex() != 1 ){
				return new TestResult(
					1,
					false,
					"GameplayTestSetUnitTest",
					"TestWeather",
					"weather not changing correctly"
					);
			}
			
			return new TestResult(
				1,
				true,
				"GameplayTestSetUnitTest",
				"TestWeather",
				"Successful"
				);
	
		}

		public override System.Collections.Generic.IList<TestResult> RunAllTests() {
			IList<TestResult> ret = new List<TestResult>();

			ret.Add(TestMovement());
			ret.Add(TestAttacking());
			ret.Add(TestWeather());

			return ret;
		}
	}

}
