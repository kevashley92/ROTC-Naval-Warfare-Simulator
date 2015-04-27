using UnityEngine;
using System.Collections;
using System.IO;

public class DAOFactory {
	
	private static DAOFactory dao;
	
	private DAOFactory(){}
	
	public static DAOFactory GetFactory()
	{
		if(dao == null)
		{
			dao = new DAOFactory();
		}
		return dao;
	}
	
	public SurfaceDAO GetSurfaceDAO()
	{
		return new SurfaceDAO(new FileLoader("data" + Path.DirectorySeparatorChar + "surface"  + Path.DirectorySeparatorChar + "saves", "surface"), new FileLoader("data"  + Path.DirectorySeparatorChar + "surface"  + Path.DirectorySeparatorChar + "defaults", "defaults"));
	}

   public SubSurfaceDAO GetSubSurfaceDAO() 
	{
		return new SubSurfaceDAO(new FileLoader("data"  + Path.DirectorySeparatorChar + "subsurface"  + Path.DirectorySeparatorChar + "saves", "subsurface"), new FileLoader("data"  + Path.DirectorySeparatorChar + "subsurface"  + Path.DirectorySeparatorChar + "defaults", "dedfaults"));
	}

	public AirDAO GetAirDAO()
	{
		return new AirDAO(new FileLoader("data"  + Path.DirectorySeparatorChar + "air"  + Path.DirectorySeparatorChar + "saves", "air"), new FileLoader("data"  + Path.DirectorySeparatorChar + "air"  + Path.DirectorySeparatorChar + "defaults", "defaults"));
	}

	public WeaponDAO GetWeaponDAO()
	{
		return new WeaponDAO(new FileLoader("data"  + Path.DirectorySeparatorChar + "weapons"  + Path.DirectorySeparatorChar + "saves", "weapons"), new FileLoader("data"  + Path.DirectorySeparatorChar + "weapons"  + Path.DirectorySeparatorChar + "defaults", "defaults"));
	}
	
	public MarineDAO GetMarineDAO()
	{
		return new MarineDAO(new FileLoader("data"  + Path.DirectorySeparatorChar + "marines"  + Path.DirectorySeparatorChar + "saves", "marines"), new FileLoader("data"  + Path.DirectorySeparatorChar + "marines"  + Path.DirectorySeparatorChar + "defaults", "defaults"));
	}
	
	public EnvironmentVariableDAO GetEvironmentVariableDAO()
	{
		return new EnvironmentVariableDAO(new FileLoader("data"  + Path.DirectorySeparatorChar + "environmentVariables"  + Path.DirectorySeparatorChar + "saves"  + Path.DirectorySeparatorChar + "environmentVariables", "environmentVariables"), new FileLoader("data"  + Path.DirectorySeparatorChar + "environmentVariables"  + Path.DirectorySeparatorChar + "defaults", "defaults"));
	}

	public LogFileDAO GetLogFileDAO()
	{

		return new LogFileDAO(new LogFileLoader("data" + Path.DirectorySeparatorChar + "logs", "logs"));

	}

	public WeatherDAO GetWeatherDAO()
	{

		return new WeatherDAO(new FileLoader("data" + Path.DirectorySeparatorChar + "environmentVariables" + Path.DirectorySeparatorChar + "saves", "weather"));

	}
	
	public SurfaceDAO GetSurfaceScenarioDAO(string scenarioName)
	{
		return new SurfaceDAO(new FileLoader("data" + Path.DirectorySeparatorChar + "gameSaves"  + Path.DirectorySeparatorChar + scenarioName + Path.DirectorySeparatorChar + "navy" + Path.DirectorySeparatorChar + "surface", "surface"), new FileLoader("data"  + Path.DirectorySeparatorChar + "surface"  + Path.DirectorySeparatorChar + "defaults", "defaults"));
	}
	
	
	public SubSurfaceDAO GetSubSurfaceScenarioDAO(string scenarioName) 
	{
		return new SubSurfaceDAO(new FileLoader("data"  + Path.DirectorySeparatorChar + "gameSaves"  + Path.DirectorySeparatorChar + scenarioName + Path.DirectorySeparatorChar + "navy" + Path.DirectorySeparatorChar + "subsurface", "subsurface"), new FileLoader("data"  + Path.DirectorySeparatorChar + "subsurface"  + Path.DirectorySeparatorChar + "defaults", "defaults"));
	}
	
	public AirDAO GetAirScenarioDAO(string scenarioName)
	{
		return new AirDAO(new FileLoader("data"  + Path.DirectorySeparatorChar + "gameSaves" + Path.DirectorySeparatorChar + scenarioName + Path.DirectorySeparatorChar + "navy"  + Path.DirectorySeparatorChar + "air", "air"), new FileLoader("data"  + Path.DirectorySeparatorChar + "air"  + Path.DirectorySeparatorChar + "defaults", "defaults"));
	}
	
	public MarineDAO GetMarineScenarioDAO(string scenarioName)
	{
		return new MarineDAO(new FileLoader("data"  + Path.DirectorySeparatorChar + "gameSaves"  + Path.DirectorySeparatorChar + scenarioName + Path.DirectorySeparatorChar + "marines", "marines"), new FileLoader("data"  + Path.DirectorySeparatorChar + "marines"  + Path.DirectorySeparatorChar + "defaults", "defaults"));
	}
	
	public EnvironmentVariableDAO GetEvironmentVariableScenarioDAO(string scenarioName)
	{
		return new EnvironmentVariableDAO(new FileLoader("data"  + Path.DirectorySeparatorChar + "gameSaves" + Path.DirectorySeparatorChar + scenarioName + Path.DirectorySeparatorChar + "EnvironmentVariables", "environmentVariables"), new FileLoader("data"  + Path.DirectorySeparatorChar + "environmentVariables"  + Path.DirectorySeparatorChar + "defaults", "defaults"));
	}
	
	public MapDAO GetMapScenarioDAO(string scenarioName)
	{
		return new MapDAO("data"  + Path.DirectorySeparatorChar + "gameSaves" + Path.DirectorySeparatorChar + scenarioName);
	}
}
