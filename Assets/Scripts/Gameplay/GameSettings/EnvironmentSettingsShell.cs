using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class EnvironmentSettingsShell {
    private static readonly string[] _keys = {
                                                 "GlobalSettings",
                                                 "TurnManager",
                                                 "Weather",
                                                 "Teams",
                                                 "Objective"
                                             };
    private static IDictionary _globalSettings;
    private static IDictionary _turnManager;
    private static IDictionary _weather;
    private static IList _teamData;
    private static IDictionary _objective;

    public static void ClearAll() {
        ClearGlobalSettings();
        ClearTurnManager();
        ClearWeather();
        ClearTeamData();
        ClearObjectives();
    }

    public static void DumpAll() {
        DumpGlobalSettings();
        DumpTurnManager();
        DumpWeather();
        DumpTeamData();
        DumpObjectives();
    }

    public static void ClearGlobalSettings() {
        _globalSettings = new Dictionary<string, System.Object>();
    }

    public static void DumpGlobalSettings() {
        ClearGlobalSettings();

        _globalSettings["half_movement"] = GlobalSettings.GetHealthThresholdForHalfMovement();
        _globalSettings["no_aircraft"] = GlobalSettings.GetHealthThresholdForNoAircraftLaunching();
        _globalSettings["no_detector"] = GlobalSettings.GetHealthThresholdForNoDetectors();
        _globalSettings["no_movement"] = GlobalSettings.GetHealthThresholdForNoMovement();
        _globalSettings["no_weapons"] = GlobalSettings.GetHealthThresholdForNoWeapons();
        _globalSettings["retaliation_p"] = GlobalSettings.GetRetaliationPercentChance();
        _globalSettings["retaliation_d"] = GlobalSettings.GetRetaliationDamage();
        _globalSettings["w_index"] = GlobalSettings.GetCurrentWeatherIndex();
    }

    public static void ClearTurnManager() {
        _turnManager = new Dictionary<string, System.Object>();
    }

    public static void DumpTurnManager() {
        ClearTurnManager();

        _turnManager["count"] = TurnManager.TurnCount;
    }

    public static void ClearWeather() {
        _weather = new Dictionary<string, System.Object>();
    }

    public static void DumpWeather() {
        ClearWeather();

        if (Weather.WeatherTypes != null) {
            foreach (int key in Weather.WeatherTypes.Keys) {
                string newKey = Weather.WeatherTypes[key].WeatherType;

                _weather[newKey] = new Dictionary<string, System.Object>();

                ((IDictionary)_weather[newKey])["index"] = key;
                ((IDictionary)_weather[newKey])["movement"] = Weather.WeatherTypes[key].MovementModifier;
                ((IDictionary)_weather[newKey])["visibility"] = Weather.WeatherTypes[key].VisionModifier;
            }
        }
    }

    public static void ClearTeamData() {
        _teamData = new List<System.Object>();
    }

    public static void DumpTeamData() {
        ClearTeamData();

        IDictionary inGameTeams = Team.GetAllTeams();

        if (inGameTeams != null) {
            foreach (int key in inGameTeams.Keys) {
                Team currentTeam = ((Team)inGameTeams[key]);
                string teamName = currentTeam.GetTeamName();
                Color currentTeamColor = currentTeam.GetTeamColor();

                Dictionary<string, System.Object> putTeam = new Dictionary<string, System.Object>();

                putTeam.Add("index", key);
                putTeam.Add("team_name", teamName);
                putTeam.Add("red", currentTeamColor.r);
                putTeam.Add("green", currentTeamColor.g);
                putTeam.Add("blue", currentTeamColor.b);

                _teamData.Add(putTeam);
            }
        }
    }

    public static void ClearObjectives() {
        _objective = new Dictionary<string, System.Object>();
    }

    public static void DumpObjectives() {
        ClearObjectives();
        IList individualObjectKeys = Objective.GetInstance().GetIndividualObjectives();
        IList individualObjects = new List<string>();

        _objective["text"] = Objective.GetInstance().GetObjective();
        foreach (string ob in individualObjectKeys) {
            Dictionary<string, System.Object> item = new Dictionary<string, object>();
            item.Add("value", ob);
            individualObjects.Add(item);
        }
        _objective["ind"] = individualObjects;
    }

    public static IDictionary EnvironmentalSettings {
        get {
            IDictionary _environmentalSettings = new Dictionary<string, System.Object>();

            _environmentalSettings["GlobalSettings"] = _globalSettings;
            _environmentalSettings["TurnManager"] = _turnManager;
            _environmentalSettings["Weather"] = _weather;
            _environmentalSettings["Teams"] = _teamData;
            _environmentalSettings["Objective"] = _objective;

            return _environmentalSettings;
        }
        private set { }
    }

    public static List<string> Keys {
        get {
            return new List<string>(_keys);
        }
        private set { }
    }
}