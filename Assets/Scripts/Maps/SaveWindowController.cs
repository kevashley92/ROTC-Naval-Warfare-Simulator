using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Maps {
    public class SaveWindowController : MapDebuggableBehaviour {
        public GameObject errorMessage;
        public InputField textBox;
        private ISave saveObject;

        public void Activate(string fileName, ISave saveObject) {
            if (saveObject == null)
                throw new ArgumentException();

            this.saveObject = saveObject;

            if (fileName == null)
                fileName = "";

            gameObject.SetActive(true);
            errorMessage.SetActive(false);

            textBox.text = fileName;

            MapMovementDriver.MovementEnabled = false;
        }

        public void Save() {
            string fileName = textBox.text;
            if (fileName == null || fileName.Length == 0) {
                errorMessage.SetActive(true);
            } else {
                saveObject.Save(fileName);
                Close();
            }
        }

        public void Close() {
            gameObject.SetActive(false);
            MapMovementDriver.MovementEnabled = true;
        }
    }
}
