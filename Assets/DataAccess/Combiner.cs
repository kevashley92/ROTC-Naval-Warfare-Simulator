using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Combiner
{
	
	public static Dictionary<string, System.Object> Combine(Dictionary<string, System.Object> suspectVersion, Dictionary<string, System.Object> defaultsVersion)
	{
		Dictionary<string, System.Object> toReturn = new Dictionary<string, System.Object>();
		
		List<string> toCheck = new List<string>(suspectVersion.Keys);
		
		List<string> references = new List<string>(defaultsVersion.Keys);
		
		foreach (string reference in references)
		{
			if (toCheck.Contains(reference))
			{
				if(suspectVersion[reference] is Dictionary<string, System.Object>)
				{
					if (defaultsVersion[reference] is Dictionary<string, System.Object>)
					{
						toReturn.Add(reference, Combine((Dictionary<string, System.Object>)suspectVersion[reference], (Dictionary<string, System.Object>)defaultsVersion[reference]));
					}
					else
					{
						toReturn.Add(reference, defaultsVersion[reference]);
					}
				}
				else if (suspectVersion[reference] is List<System.Object>)
				{
					if (defaultsVersion[reference] is List<System.Object>)
					{
						toReturn.Add(reference, HandleEmbededArrays((List<System.Object>)suspectVersion[reference], (List<System.Object>)defaultsVersion[reference]));
					}
					else
					{
						toReturn.Add(reference, defaultsVersion[reference]);
					}
				}
				else
				{
					toReturn.Add(reference, suspectVersion[reference]);
				}
			}
			else
			{
				toReturn.Add(reference, defaultsVersion[reference]);
			}
		}
		
		return toReturn;
	}
	
	private static List<System.Object> HandleEmbededArrays(List<System.Object> suspectVersion, List<System.Object> defaultsVersion)
	{
		List<System.Object> toReturn = new List<System.Object>();
		
		if (defaultsVersion.Count == 0)
		{
			toReturn.AddRange(suspectVersion);
		}
		else
		{
			for (int i = 0; i < suspectVersion.Count; i++)
			{
				for (int j = 0; j < defaultsVersion.Count; j++)
				{
					if (SameContent((Dictionary<string, System.Object>)suspectVersion[i], (Dictionary<string, System.Object>)defaultsVersion[j]))
					{
						toReturn.Add(Combine((Dictionary<string, System.Object>)suspectVersion[i], (Dictionary<string, System.Object>)defaultsVersion[j]));
						break;
					}
				}
			}
		}
		
		return toReturn;
	}
	
	private static bool SameContent(Dictionary<string, System.Object> suspect, Dictionary<string, System.Object> defaults)
	{
		List<string> suspectKeys = new List<string>(suspect.Keys);
		List<string> defaultsKeys = new List<string>(defaults.Keys);

		if(defaults.ContainsKey("name"))
		{
			if (defaults["name"].Equals(suspect["name"]))
				return true;
		}
		else if(defaults.ContainsKey("class")){

			if(defaults["class"].Equals(suspect["class"]))
				return true;
		}
		return false;
	}
}