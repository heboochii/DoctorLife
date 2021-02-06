using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace PediatricSim
{
    public class MainManager : MonoBehaviour
    {

        #region Singleton

        private static MainManager m_Singleton;

        public static MainManager Singleton
        {
            get
            {
                return m_Singleton;
            }
        }

        #endregion

        #region Variables

        

        

        [Header("Windows")]
        [Space]
        [SerializeField]
        public Window TestWindow;
        [SerializeField]
        public Window TestDoneWindow;
        [SerializeField]
        public Window LabResWindow;
        [SerializeField]
        public Window PatientLabWindow;
        [SerializeField]
        public Window PatientInfoWindow;

        [Header("Sprites")]
        [Space]
        [SerializeField]
        public Sprite[] ResultsImagesAmanda = new Sprite[2];
        public Sprite[] ResultsImagesJohn = new Sprite[3];


        public TextMeshProUGUI FlashText;

        public Window ActiveWindow;
        
  
        public int FlashDeck = -1;
        public int FlashNumber = -1;

        private int currentquestion;
        private int correctquestions;

        private int correctone;
        private int PatientId;

        public Database DatabaseInstance;
        public MainMessages MainMsgInstance;


        private bool CoroutineRunning=false;
  
        #endregion

        #region Monovehaviour
        void Awake()
        {
            m_Singleton = this;
        }

        #endregion

        #region Functions



        #region WindowFunctions
        public void OpenWindow(Window window)
        {
            if (ActiveWindow == window) return; // if active window is the one then open, and to just ignore it
            if(ActiveWindow != null)CloseCurrentWindow(); // if other window is open, its to close it

            ActiveWindow = window; // to set the active window to the one to open
            window.OpenThisWindow(); // to open this window 

            
        }

        
        public void CloseCurrentWindow(bool bypass = false)
        {
            ActiveWindow.CloseThisWindow(bypass); // to close the active window
            ActiveWindow = null; // to set the active window variable to null
        }

        public void SubmitName(TextMeshProUGUI text)
        {
            string input = text.text;
            if (string.IsNullOrEmpty(input)) return;
            if (input.Length > 20) return;


            GameObject.Find("Database").GetComponent<Database>().Name = input;
            GameObject.Find("Database").GetComponent<Database>().UpdateStats(0);

            GameObject.Find("MainManager").GetComponent<MainManager>().CloseCurrentWindow();
            PlayerPrefs.SetString("Name", input);
        }
        #endregion

        #region FlashCardsFunctions
        public void UpdateFlashText()
        {
            if(FlashNumber == 5)
            {
                if (FlashDeck < DatabaseInstance.nexttest) FlashText.text = "You already passed this test, goodjob";
                else if(!DatabaseInstance.CanTakeTest || FlashDeck > DatabaseInstance.nexttest) FlashText.text = "You can't take the test yet, carry on in the story";
                else FlashText.text = "To take the short test, press the right button";
            }
            else FlashText.text = DatabaseInstance.FlashCardsText[FlashDeck, FlashNumber]; // to change the text to the one in databases based on the current deck and number
        }

        public void PlusFlashNumber(bool plus)
        {
            if (plus && FlashNumber == 5)
            {
                if (DatabaseInstance.CanTakeTest && FlashDeck == DatabaseInstance.nexttest) StartTest();
                    
               return;
            }
            if (!plus && FlashNumber == 0) return;


            if(!CoroutineRunning)StartCoroutine(FlashTextCoroutine(plus));
        }

        public void OpenFlashCardDeck(int which)
        {
            FlashDeck = which - 1;  // -1, because arrays are indexed from 0, not from 1
            FlashNumber = 0;
            UpdateFlashText();
            OpenWindow(FlashText.gameObject.transform.parent.GetComponent<Window>()); // instead of holding the window as a public, so can just get it like that
        }

        
        IEnumerator FlashTextCoroutine(bool plus)
        {
            CoroutineRunning = true; // to set, that the coroutine is running, so couldnt start a new one, while the last one is running
            bool disappear = true;
            int movesteps=0;
            GameObject Fgm = FlashText.gameObject; // to take the flashtext game object

            float xmove = Fgm.transform.localScale.x / 20; // to calculate how much should the x step be
            float ymove = Fgm.transform.localScale.y / 20; // to calculate how much should the y step be

            while (disappear)
            {
                movesteps++; // to add to the total steps
                Fgm.transform.localScale = new Vector3(Fgm.transform.localScale.x - xmove, Fgm.transform.localScale.x - ymove, Fgm.transform.position.z); // to transform the scale one step

                if(movesteps == 20)
                {
                    movesteps = 0; // if the total steps are 20, to reset everything
                    disappear = false;

                    if (plus) FlashNumber++; // if to plus and to add to flashnumber
                    else FlashNumber--;
                    UpdateFlashText();
                }

                yield return new WaitForEndOfFrame();
            }

            while (!disappear) // same thing, but backwards
            {
                movesteps++;
                Fgm.transform.localScale = new Vector3(Fgm.transform.localScale.x + xmove, Fgm.transform.localScale.y + ymove, Fgm.transform.position.z);

                if (movesteps == 20)
                {
                    disappear = true;
                }

                yield return new WaitForEndOfFrame();
            }
            CoroutineRunning = false; // to save, that the coroutine isn't running
            yield break;
        }
        #endregion

        #region FlashCardsTestFunctions

        public void StartTest()
        {
            OpenWindow(TestWindow);
            currentquestion = 0;
            correctquestions = 0;

            ShowNextQuestion();
        }

        private void ShowNextQuestion()
        {
            currentquestion++;

            if(currentquestion == 5)
            {
                if(correctquestions >= 3)
                {
                    TestDoneWindow.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Congratulations, you passed the test with " + correctquestions + " / 5 correct questions";
                    MainMsgInstance.RemoveContinueMessage();
                    DatabaseInstance.MainProgress--;
                    DatabaseInstance.AddStats(1);
                }
                else
                {
                    TestDoneWindow.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Sorry, but you failed the test with " + correctquestions + " / 5 correct questions";
                }
                OpenWindow(TestDoneWindow);
                return;
            }

            TestWindow.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = DatabaseInstance.TestQuestions[FlashDeck, currentquestion - 1];// to change the question
            correctone = Random.Range(0,3);
            TestWindow.gameObject.transform.GetChild(correctone + 2).GetChild(0).GetComponent<TextMeshProUGUI>().text = DatabaseInstance.RightAnswer[FlashDeck, currentquestion - 1];

            if(correctone == 0)
            {
                TestWindow.gameObject.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = DatabaseInstance.OtherAnswers1[FlashDeck, currentquestion - 1];
                TestWindow.gameObject.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = DatabaseInstance.OtherAnswers2[FlashDeck, currentquestion - 1];
            }
            else if(correctone == 1)
            {
                TestWindow.gameObject.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = DatabaseInstance.OtherAnswers1[FlashDeck, currentquestion - 1];
                TestWindow.gameObject.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = DatabaseInstance.OtherAnswers2[FlashDeck, currentquestion - 1];
            }
            else
            {
                TestWindow.gameObject.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = DatabaseInstance.OtherAnswers1[FlashDeck, currentquestion - 1];
                TestWindow.gameObject.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = DatabaseInstance.OtherAnswers2[FlashDeck, currentquestion - 1];
            }

        }

        public void AnswerQuestion(int whichone)
        {
            if (whichone == correctone) correctquestions++;
            ShowNextQuestion();
        }

        #endregion

        #region PatientsFunctions

        public void OpenPatient(int id)
        {
            Database dtb = GameObject.Find("Database").GetComponent<Database>();
            int Level = dtb.Level;

            if(id != 999) // 999 indicates, that user can open it from the patient info box
            {
                if (Level < id) return;
                PatientId = id;
            }
            else
            {
                id = PatientId;
                dtb.Lab = true;
            }
            

            int Mp = dtb.MainProgress;
            if (Level > PatientId) Mp = 100;

            if (dtb.Lab)
            {
                switch (id)
                {
                    case 0:
                        PatientLabWindow.transform.GetChild(5).gameObject.SetActive(false); // because third test button is never used with Amanda
                        PatientLabWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Patient Amanda Smith lab results";
                        if (Mp >= 8) PatientLabWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Vitals:\nTemperature: 37°C – NORMAL\nPulse rate: 110 BPM – HIGH\nBlood pressure: 140mmHg / 90mmHg - HIGH";
                        else PatientLabWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
                        if (Mp >= 11)
                        {
                            PatientLabWindow.transform.GetChild(3).gameObject.SetActive(true);
                            PatientLabWindow.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Xray";
                            PatientLabWindow.transform.GetChild(4).gameObject.SetActive(true);
                            PatientLabWindow.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = "ECG";
                        }
                        else
                        {
                            PatientLabWindow.transform.GetChild(3).gameObject.SetActive(false);
                            PatientLabWindow.transform.GetChild(4).gameObject.SetActive(false);
                            break;
                        }
                        break;

                }

                OpenWindow(PatientLabWindow);
            }
            else
            {
                switch(PatientId)
                {
                    case 0:
                        PatientInfoWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Patient Name: Amanda Smith\nGender: Female\nDOB: 21.03.2002 (18 years)\nPhone : +971 55 1234567\nEmail: smith.amanda@gmail.com\nDoctor assigned : " + DatabaseInstance.Name;
                        if (Mp >= 9) PatientInfoWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Diagnosis : Ventricular Septal Defect";
                        if (Mp >= 4)PatientInfoWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Notes & observations : \n'I feel fatigued, tired all time and a little breathless.'";
                        if (Mp >= 15) PatientInfoWindow.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text += "Treatment: low sodium diet, exercise, digoxin, diuretics";

                        break;

                }
                OpenWindow(PatientInfoWindow);
}
        }

        public void OpenResult(int which)
        {
            if (MainMsgInstance.CheckWhich == which) MainMsgInstance.RemoveContinueMessage();
            switch(PatientId)
            {
                case 0:
                    if (which == 0)
                    {
                        LabResWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Patient Amanda Smith Xray results";
                        LabResWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                            "In picture A, posteroanterior exam shows dilation of the main pulmonary artery and the other pulmonary arteries (arrows 1 and 2). The upper and lower(arrows 3 and 4 respectively)  are sharp and dilated indicating shunt vascularity. Arrow 5 indicates narrowing of the superior mediastinum and displacement of the heart toward the left indicate right heart volume loading. In picture B, lateral examination shows filling of the retrosternal space, indicating right heart and main pulmonary artery enlargement.The left bronchus(1 arrow) is displaced, and the inferior retrocardiac space is filled, indicating left atrial and ventricular enlargement";
                        LabResWindow.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = ResultsImagesAmanda[0];
                        break;
                    }
                    else if(which == 1)
                    {
                        LabResWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Patient Amanda Smith ECG results";
                        LabResWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                            "EKG is characterized by left axis deviation due to inferior and posterior displacement of the AV node (anatomical). It may also show right ventricular hypertrophy (due to increased pressure), right atrial enlargement and LVH. A prolonged PR interval (first degree heart block) probably due to abnormal AV node conduction may be present.Chest radiograph shows varying degrees of cardiomegaly and increased pulmonary vascularity. Echocardiography is useful in demonstrating the anatomical lesions and associated abnormalities.It is essential to assess the integrity of the AV valves";
                        LabResWindow.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = ResultsImagesAmanda[1];
                    }
                    break;
                case 1:
                    if (which == 0)
                    {
                        LabResWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Patient Amanda Smith Xray results";
                        LabResWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                            "In picture A, posteroanterior exam shows dilation of the main pulmonary artery and the other pulmonary arteries (arrows 1 and 2). The upper and lower(arrows 3 and 4 respectively)  are sharp and dilated indicating shunt vascularity. Arrow 5 indicates narrowing of the superior mediastinum and displacement of the heart toward the left indicate right heart volume loading. In picture B, lateral examination shows filling of the retrosternal space, indicating right heart and main pulmonary artery enlargement.The left bronchus(1 arrow) is displaced, and the inferior retrocardiac space is filled, indicating left atrial and ventricular enlargement";
                        LabResWindow.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = ResultsImagesAmanda[0];
                        break;
                    }
                    else if (which == 1)
                    {
                        LabResWindow.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Patient Amanda Smith ECG results";
                        LabResWindow.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                            "EKG is characterized by left axis deviation due to inferior and posterior displacement of the AV node (anatomical). It may also show right ventricular hypertrophy (due to increased pressure), right atrial enlargement and LVH. A prolonged PR interval (first degree heart block) probably due to abnormal AV node conduction may be present.Chest radiograph shows varying degrees of cardiomegaly and increased pulmonary vascularity. Echocardiography is useful in demonstrating the anatomical lesions and associated abnormalities.It is essential to assess the integrity of the AV valves";
                        LabResWindow.transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = ResultsImagesAmanda[1];
                    }
                    break;

            }
            OpenWindow(LabResWindow.GetComponent<Window>());
        }

        #endregion
        #endregion
    }
}
