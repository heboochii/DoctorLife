using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace PediatricSim
{
    static class Constants
    {


    }
    public class Database : MonoBehaviour
    {

        #region FlashCards

        public readonly string[,] FlashCardsText =
        {
            {   "VSD - Ventricular Septal Defect",
                "Pathophysiology of VSD– They are located in the membranous or muscular portion of the ventricular septum and vary in size.",
                "Cause of VSD– Congenital: Secondary to acute myocardial infarction (MI)",
                "Risk Factor– Coronary artery disease and hypertension (post-MI)",
                "Complications– Endocarditis, Heart failure, Pneumonia, Aortic insufficiency, Right ventricular outflow obstruction, Cardiogenic shock, Pulmonary hypertension"},

            {   "Physical Findings– Pallor, Clubbing, Cyanosis, Tachypnea, Tachycardia, Elevated jugular venous pressure",
                "Treatment(General)– Intra-aortic balloon counter pulsation to stabilize a patient with a post-MI VSD before surgery, Watch and see approach if the defect is small",
                "Treatment(Diet)– Low-sodium diet, Fluid restriction",
                "Treatment(Medication)– Digoxin, Diuretics, Angiotensin-converting enzyme inhibitors",
                "Diagnosing – X-Ray and ECG"},

            {   "AST - Atrial Septal Defect",
                "Pathophysiology of VSD– They are located in the membranous or muscular portion of the ventricular septum and vary in size.",
                "Cause of VSD– Congenital: Secondary to acute myocardial infarction (MI)",
                "Risk Factor– Coronary artery disease and hypertension (post-MI)",
                "Complications– Endocarditis, Heart failure, Pneumonia, Aortic insufficiency, Right ventricular outflow obstruction, Cardiogenic shock, Pulmonary hypertension"},

            {   "VSD - Ventricular Septal Defect",
                "Pathophysiology of VSD– They are located in the membranous or muscular portion of the ventricular septum and vary in size.",
                "Cause of VSD– Congenital: Secondary to acute myocardial infarction (MI)",
                "Risk Factor– Coronary artery disease and hypertension (post-MI)",
                "Complications– Endocarditis, Heart failure, Pneumonia, Aortic insufficiency, Right ventricular outflow obstruction, Cardiogenic shock, Pulmonary hypertension"}
        };


        #endregion

        #region Test

        public readonly string[,] TestQuestions =
        {
            {
                "What does VSD stand for?",
                "Where is VSD located?",
                "What is the cause of VSD?",
                "What is the risk factor of VSD?",
                "What are the complications?"
            },
            {
                "What are physical findings of VSD?",
                "What is the general treatment of VSD?",
                "What is the diet threatment of VSD?",
                "What is the medication treatment of VSD?",
                "How do you diagnose VSD?"
            },
            {
                "What does ASD stand for?",
                "Where is VSD located?",
                "What is the cause of VSD?",
                "What is the risk factor of VSD?",
                "What are the complications?"
            },
            {
                "What are physical findings of VSD?",
                "What is the general treatment of VSD?",
                "What is the diet threatment of VSD?",
                "What is the medication treatment of VSD?",
                "How do you diagnose VSD?"
            }
        };

        public readonly string[,] RightAnswer =
        {
            {
                "Ventricular Septal Defect",
                "Muscular portion of the VS",
                "Congenital",
                "Hypertension",
                "Heart failure"
            },
            {
                "Pallor, Clubbing, Cyanosis",
                "Intra-aortic balloon counter pulsation",
                "Low-sodium diet",
                "Digoxin",
                "X-Ray and ECG"
            }
        };

        public readonly string[,] OtherAnswers1 =
        {
            {
                "Ventricle septum disct",
                "Atrial septum",
                "Compulsive",
                "Stroke",
                "Nerve damage"
            },
            {
                "Increased thirst",
                "Cardiac catheterization",
                "Vegetable diet",
                "amoxicillin",
                "Temperature test + Blood pressure test"
            }
        };

        public readonly string[,] OtherAnswers2 =
        {
            {
                "Ventricular septum diagnosis",
                "Left ventricle",
                "Pathological",
                "Heart attack",
                "Skin conditions"
            },
            {
                "Irritability",
                "Echocardiogram",
                "High-sodium diet",
                "Cafalexin",
                "Blood test"
            }
        };

        #endregion 

        #region Main

        public int nexttest;
        public bool CanTakeTest;

        public int MainProgress;
        public string Name;
        private int TestsPassed;
        private int VitalsTook;
        public int Level;
        public int Sound;

        public bool Lab; 

        public MainManager MainInst;

        public Sprite SoundOn;
        public Sprite SoundOff;

        public int LastLevel = 1;



        public static readonly int[] MaxLevelProgress =
        {
            17,
            0
        };

        public void Awake()
        {
            if (!PlayerPrefs.HasKey("Name")) MainManager.Singleton.OpenWindow(GameObject.Find("EnterName").GetComponent<Window>());

            Name = PlayerPrefs.GetString("Name");
            TestsPassed = PlayerPrefs.GetInt("TestsPassed");
            VitalsTook = PlayerPrefs.GetInt("VitalsTook");
            nexttest = PlayerPrefs.GetInt("NextTest");
            Level = PlayerPrefs.GetInt("Level");
            Sound = PlayerPrefs.GetInt("Sound");


            for(int i = 0;i<=5;i++)
            {
                UpdateStats(i);
            }
        }

        public void AddStats(int which)
        {
            switch (which)
            {
                case 0:
                    MainProgress++;
                    UpdateStats(1);
                    break;
                case 1:
                    TestsPassed++;
                    nexttest++;
                    CanTakeTest = false;
                    PlayerPrefs.SetInt("TestsPassed", TestsPassed);
                    PlayerPrefs.SetInt("NextTest", nexttest);
                    AddStats(0);
                    UpdateStats(3);
                    break;
                case 2:
                    VitalsTook++;
                    PlayerPrefs.SetInt("VitalsTook", VitalsTook);
                    UpdateStats(4);
                    break;
                case 3:
                    Level++;
                    PlayerPrefs.SetInt("Level", Level);
                    UpdateStats(2);
                    break;
                case 4:
                    if (Sound == 0) Sound = 1;
                    else Sound = 0;
                    PlayerPrefs.SetInt("Sound", Sound);
                    UpdateStats(5);
                    break;

            }
        }

        public void UpdateStats(int which)
        {
            GameObject StatsObject = GameObject.Find("StatsWindow");
            switch (which)
            {
                case 0:
                    StatsObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Name : Dr. " + Name;
                    break;
                case 1:
                    StatsObject.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Progress : Level " + (Level+1);
                    float value = (float)MainProgress / MaxLevelProgress[Level] * 100;
                    Debug.Log(value);
                    StatsObject.transform.GetChild(4).GetComponent<Slider>().value = value;

                    break;
                case 2:
                    StatsObject.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "Patients had : " + Level;
                    break;
                case 3:
                    StatsObject.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = "Tests passed : " + TestsPassed;
                    break;
                case 4:
                    StatsObject.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = "Vitals took : " + VitalsTook;
                    break;
                case 5:
                    if (Sound == 1)
                    {
                        AudioListener.pause = false;
                        GameObject.Find("SoundButton").GetComponent<SpriteRenderer>().sprite = SoundOn;
                    }
                    else
                    {
                        AudioListener.pause = true;
                        GameObject.Find("SoundButton").GetComponent<SpriteRenderer>().sprite = SoundOff;
                    }
                    break;
            }

        }
        #endregion 
    }
}

