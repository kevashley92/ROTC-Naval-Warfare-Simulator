using UnityEngine;
using System.Collections;

public interface iFileLoader {
	string Read();
	void Write(string json);
	string ReadDefault();
}
