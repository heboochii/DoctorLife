using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace PediatricSim
{
    public class Window : MonoBehaviour
    {
        private static readonly string[] PatientsNames =
        {
            "Amanda Smith",
            "John Williams"
        };
        #region Singleton

        private static Window m_Singleton;

        public static Window Singleton
        {
            get
            {
                return m_Singleton;
            }
        }

        #endregion

        #region MonoBehaviour
        void Awake()
        {
            m_Singleton = this;
            this.transform.localPosition = new Vector3(this.transform.position.x, 1302f, this.transform.position.z); // to move the window up. so it wouldnt be seen
        }

        #endregion

        #region Functions
        public void OpenThisWindow()
        {
            if (this.gameObject.name == "PatientWindow")
            {
                Database dtb = GameObject.Find("Database").GetComponent<Database>(); // to find the database object
                string names = "";
                for (int i = 0; i <= dtb.Level; i++) // loop through levels 
                {
                    switch (i)
                    {
                        case 0:
                            names = "Amanda Smith";
                            break;
                        case 1:
                            names = "John Williams";
                            break;
                    }
                    this.transform.GetChild(i + 1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Patient " + (i + 1) + "\n" + names; // set the Patienti+1 text to the name
                }
            }
            StartCoroutine(MoveWindow(false));
        }

        public void CloseThisWindow(bool bypass = false)
        {
            if(this.gameObject.name == "EndTestWindow")
            {
                if (!bypass) return;
            }
            Debug.Log("Calledclose");
            StartCoroutine(MoveWindow(true)); // to start the coroutine
        }

        public IEnumerator MoveWindow(bool Close)
        {
            int stepsleft = 40; // to define how many steps left

            while (stepsleft != 0)
            {
                if (Close) this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.25f, this.transform.position.z); // if it is set to close, it moves up
                else this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.25f, this.transform.position.z); // if it is set to open, it moves down
                stepsleft--; //  minus the steps left
                yield return new WaitForEndOfFrame(); //  waiting for the end of the frame
            }
        }
        #endregion
    }

}
