/*****
 * 
 * Name: EventLog
 * 
 * Date Created: 2015-01-29
 * 
 * Original Team: Gameplay
 * 
 * This class will handle the logging of events as they are run
 * 
 * Change Log
 * 
 * Name        	Date 		Comment
 * ----------	----------	--------------------------------------------  	
 * M.Schumacher	2015-01-29	Initial Commit. Created initial field and
 * 							LogEvent function
 * B. Croft		2015-01-30	Changed outputFile to outputFileName
 * 							(System.IO will handle file pointers)
 * 							Wrote initial body for LogEvent method
 * B. Croft		2015-01-30	Changed to static, no longer derives from
 * 							MonoBehaviour.  Deleted Start and Update,
 * 							added Init.
 * M.Schumacher	2015-02-11	Changed a few logging details 
 * T. Brennan	2015-02-17	Refactored
 */

using UnityEngine;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

public class EventLog {
   
	/**
	 * The name of the file the log is added to
	 */
	public static string OutputFileName;
	
	/**
	 * The name of the file the log is added to for the current turn
	 */
	public static string OutputFileNameCurrentTurn;
   

	/**
	 * Set up the logger.  Write the file
	 * 
	 * @param outputFileName 
	 *		the name of the log file
	 */
	public static void Init (string outputFileName) {

		OutputFileName = outputFileName;
		
		OutputFileNameCurrentTurn = outputFileName.Substring(0, outputFileName.Length - 4) + "CurrentTurn.txt";

		File.WriteAllText(OutputFileName,"");

		File.AppendAllText(OutputFileName,"Event Logging for all turns\n\n");
		
		File.WriteAllText(OutputFileNameCurrentTurn,"");

		File.AppendAllText(OutputFileNameCurrentTurn,"Event Logging for the current turn\n\n");

	}
	
	public static void StartNewTurn(){
		File.WriteAllText(OutputFileNameCurrentTurn,"Event Logging for the current turn\n\n");
	}

	public static string FormatEventString(string[] teams,float timestamp,string message) {
		string outstr = "[Admin,"; //hardcode admin team in all events
		foreach(string team in teams) {
			outstr += team + ",";
		}
		outstr += "] [ " + timestamp + " ] " + message;

		return outstr;
	}
	
	public static void LogManual(string m){
	
		File.AppendAllText(OutputFileName,"-");
		
		//File.AppendAllText(OutputFileName,RemoveBetween(m,'<','>'));
		File.AppendAllText(OutputFileName,m);
		
		File.AppendAllText(OutputFileName,"\n\n");
		
		File.AppendAllText(OutputFileNameCurrentTurn,"-");
		
		//File.AppendAllText(OutputFileNameCurrentTurn,RemoveBetween(m,'<','>'));
		File.AppendAllText(OutputFileNameCurrentTurn,m);
		
		File.AppendAllText(OutputFileNameCurrentTurn,"\n\n");
	}

	/**
	 * Writes an event to a log file
	 */
	public static void LogEvent(GEvent e){
		
		File.AppendAllText(OutputFileName,"-");
		
		//File.AppendAllText(OutputFileName,RemoveBetween(e.ToString(),'<','>'));
		File.AppendAllText(OutputFileName,e.ToString());
		
		File.AppendAllText(OutputFileName,"\n\n");

		File.AppendAllText(OutputFileNameCurrentTurn,"-");
		
		//File.AppendAllText(OutputFileNameCurrentTurn,RemoveBetween(e.ToString(),'<','>'));
		File.AppendAllText(OutputFileNameCurrentTurn,e.ToString());

		File.AppendAllText(OutputFileNameCurrentTurn,"\n\n");

	}
	
	public static string ReadLogFile(string teamname){
		string[] text = File.ReadAllLines(OutputFileNameCurrentTurn);
		
		return ParseLogFile(text, teamname);
		
	}
	
	public static string ReadLogFileFull(string teamname){
		string[] text = File.ReadAllLines(OutputFileName);
		
		return ParseLogFile(text, teamname);
		
	}
	
	public static string ParseLogFile(string[] fileContents, string teamName){
		
		string output = "";
		
		for(int i = 0; i < fileContents.Length; i++){
			if(isRelevantToTeam(fileContents[i], teamName)){
				output+= "-";
				output+= ParseOneLine(fileContents[i]);
				output+= "\n\n";
			}
		}
		
		return(output);
	}
	
	private static bool isRelevantToTeam(string lineContents, string teamName){
		if(lineContents.Contains(teamName)){
			return true;
		}
		return false;
	}
	
	private static string GetTeamColor(string teamName) {
		Color c = Team.GetTeam(teamName).GetTeamColor();
		return ((int)(c.r)).ToString("X2") + ((int)(c.g)).ToString("X2") + ((int)(c.b)).ToString("X2")+ "FF";
	}
	
	/** Removes all the stuff between two specified characters
	 * Taken from http://stackoverflow.com/a/1359521/893324
	 */
	private static string RemoveBetween(string s, char begin, char end)
	{
		Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
		return regex.Replace(s, string.Empty);
	}
	
	private static string replaceColor(string workingString){
		int openBracket = workingString.IndexOf("{");
		
		if(openBracket == -1){
			return null;
		}

        string after = workingString.Substring(openBracket);

		int openName = after.IndexOf("(") + openBracket;
		int closeName = after.IndexOf(")") + openBracket - openName;
		
		//get team name
		string teamName = workingString.Substring(openName + 1, closeName - 1);
		Debug.Log(teamName);
		
		//add color tag
		string leftSide = workingString.Substring(0, openBracket);
		string rightSide = workingString.Substring(closeName + openBracket + 2);
		workingString = leftSide + "<color=#" + GetTeamColor(teamName) + ">"+ rightSide;
		
		//add close to color tag
		int closeBracket = workingString.IndexOf("}");
		leftSide = workingString.Substring(0, closeBracket);
		rightSide = workingString.Substring(closeBracket + 1);
		workingString = leftSide + "</color>" + rightSide;
		
		return workingString;
	}
	
	private static string ParseOneLine(string lineContents){
		int rightBracket = lineContents.IndexOf("]");
		rightBracket++;
		rightBracket++;
		string workingString = lineContents.Substring(rightBracket);
		
		rightBracket = workingString.IndexOf("]");
		rightBracket++;
		rightBracket++;
		workingString = workingString.Substring(rightBracket);
		
		string updated = replaceColor(workingString);
		while(updated != null){
			workingString = updated;
			updated = replaceColor(workingString);
		}
		
		return workingString;
	}
   
}
