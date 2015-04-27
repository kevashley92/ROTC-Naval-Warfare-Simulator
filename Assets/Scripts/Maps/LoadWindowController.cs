using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Maps {
    public class LoadWindowController : MapDebuggableBehaviour {
        private ILoad loadObject;
        private List<string> fileNames;
        public ToggleGroup fileList;
        public GameObject errorMessage;
        public GameObject fileItemPrefab;

        void Start() { }
        void Update() { }

        public void Activate(List<string> fileNames, ILoad loadObject) {
            if (loadObject == null)
                throw new ArgumentException();

            this.loadObject = loadObject;

            gameObject.SetActive(true);
            errorMessage.SetActive(false);

            if (fileNames != null) {
                foreach (string fileName in fileNames) {
                    GameObject fileToggle = (GameObject)GameObject.Instantiate
                        (fileItemPrefab, fileList.transform.position, fileList.transform.rotation);
                    fileToggle.name = fileName;
                    fileToggle.GetComponentInChildren<Text>().text = fileName;
                    fileToggle.transform.SetParent(fileList.transform);
                    fileToggle.transform.localScale = new Vector3(1, 1, 1);
                    fileToggle.GetComponent<Toggle>().group = fileList;
                    // create prefab here
                }
            }
            MapMovementDriver.MovementEnabled = false;
        }

        public void Load() {
            var iterator = fileList.ActiveToggles().GetEnumerator();
            if (iterator.MoveNext()) {
                var active = iterator.Current;
                loadObject.Load(active.name);
                Close();
            } else {
                errorMessage.SetActive(true);
            }
        }

        private void DestroyChildren() {
            foreach (Transform child in fileList.transform) {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void Close() {
            gameObject.SetActive(false);
            DestroyChildren();
            MapMovementDriver.MovementEnabled = true;
        }
    }
}
