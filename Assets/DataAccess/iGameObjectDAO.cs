using UnityEngine;
using System.Collections.Generic;

public interface iGameObjectDAO {


    void SaveOne(string name, Dictionary<string, System.Object> toSave);

	void SaveAll();
	
	void AddToSaveList(string name, Dictionary<string,System.Object> toSave);

    Dictionary<string, System.Object> LoadOne(string name);

    List<string> GetAllNames();

    Dictionary<string, System.Object> LoadDefault();

}
