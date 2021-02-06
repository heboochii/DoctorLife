using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PediatricSim
{

    public class MainMessages : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject LastMessage;

        private static readonly float OffSetX = -0.041f;

        public Database DatabaseInstance;

        public GameObject MessagePrefab;
        public GameObject ThreeButtonsPrefab;
        public GameObject TwoButtonsPrefab;
        public GameObject Parents;

        public GameObject End4;
        public GameObject End2;

        public int CheckWhich=-1;
        private int ButtonsUntilContinue;

        private float nextcordy = 1.01858f;
        private bool IgnoreGetY;
        private bool AddButton;
        private GameObject LastEndQuestion;
        private int rightanswer;
        private int WhichQuestion;
        private int correctanswers;

        private void Awake()
        {
            LastMessage = GameObject.Find("LastMessage");
        }

        #region Functions

        [System.Obsolete]
        private void ClearChat(int howmany = 0)
        {
            GameObject deleteparent = Parents.transform.GetChild(1).gameObject;

            int childs = deleteparent.transform.GetChildCount();

            if(howmany == 0)
            {
                for(int i = 1;i<childs;i++)
                {
                    Destroy(deleteparent.transform.GetChild(i).gameObject);
                }
            }
            else
            {
                for (int i = childs-1; i >= childs-howmany-1; i--)
                {
                    Destroy(deleteparent.transform.GetChild(i).gameObject);
                }
            }
        }
        public void GetY()
        {
            if (LastMessage == null)
            {
                nextcordy = 1.01858f;
                return;
            }

            Text LastText = LastMessage.GetComponent<Text>();
            int linecount = LastText.cachedTextGenerator.lines.Count;

            float pref = linecount * 0.0114f;
   
            nextcordy = LastMessage.transform.localPosition.y - pref;


        }

        

        public void AddMessage(string answer = null)
        {




            if (answer == "NullThis") answer = null; 

            if (answer == null) answer = PickText();

            if (answer == "DontAdd") return;

            if (IgnoreGetY) IgnoreGetY = false;
            else GetY();

            GameObject temp;
            temp = Instantiate(MessagePrefab, new Vector3(0.0f,0.0f,0.0f), Quaternion.identity);

            Transform parents = Parents.transform;

            LastMessage = temp;


            LastMessage.GetComponent<Text>().text = answer;


            LastMessage.transform.SetParent(parents.GetChild(1));
            LastMessage.transform.localPosition = new Vector3(OffSetX, nextcordy, 0.39f);
            LastMessage.transform.localScale = new Vector3(0.004879634f, 0.0004198003f, 1.0f);


            if (AddButton)
            {
                AddButton = false;
                AddChatButtons();
            }
            

            DatabaseInstance.AddStats(0);


        }

        private void AddChatButtons()
        {
            IgnoreGetY = true;
            GetY();
            nextcordy -= 0.0534f;
            GameObject Buttons;
            switch (DatabaseInstance.MainProgress)
            {
                case 5:
                    Buttons = Instantiate(ThreeButtonsPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);

                    Buttons.transform.SetParent(this.transform.GetChild(1));
                    Buttons.transform.localPosition = new Vector3(0.0f, nextcordy, 16.1f);

                    Buttons.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Check temperature";
                    Buttons.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Check pulse rate";
                    Buttons.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Check blood pressure";

                    ButtonsUntilContinue = 3;
                    
                    break;
                case 9:
                    Buttons = Instantiate(TwoButtonsPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
                    
                    Buttons.transform.SetParent(this.transform.GetChild(1));
                    Buttons.transform.localPosition = new Vector3(0.0f, nextcordy, 16.1f);

                    Buttons.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Perform X-Ray";
                    Buttons.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Perform ECG";

                    ButtonsUntilContinue = 2;
                    break;

            }
            this.transform.GetChild(0).GetComponent<UIButton>().interactable = false;
            nextcordy -= 0.0134f;
        }

        public void ChatButton(GameObject button)
        {
            if (CheckWhich >= 0) return;

            button.GetComponent<ButtonInMain>().interactable = false;
            button.GetComponent<Image>().color = Color.grey;

            int which = (int)char.GetNumericValue(button.name[button.name.Length - 1]);
            int MainPrgs = DatabaseInstance.MainProgress;


            switch (DatabaseInstance.Level)
            {
                case 0:
                    switch(which)
                    {
                        case 1:
                            if (MainPrgs >= 6 && MainPrgs <= 8)
                            {
                                AddMessage("⦁	Blood pressure: 140mmHg / 90mmHg – HIGH");
                                DatabaseInstance.AddStats(2);
                                
                            }
                            else if (MainPrgs >= 10 && MainPrgs <= 11)
                            {
                                AddMessage("Xray taken, results are in the patient lab results section");
                                AddContinueMessage("To continue, please check the Xray results", 0);
                            }
                            break;
                        case 2:
                            if (MainPrgs >= 6 && MainPrgs <= 8)
                            {
                                AddMessage("⦁	Temperature: 37°C – NORMAL ");
                                DatabaseInstance.AddStats(2);
                                
                            }
                            else if (MainPrgs >= 10 && MainPrgs <= 11)
                            {
                                AddMessage("ECG taken, results are in the patient lab results section");
                                AddContinueMessage("To continue, please check the ECG results", 1);
                            }
                            break;
                        case 3:
                            if (MainPrgs >= 6 && MainPrgs <= 8)
                            {
                                AddMessage("⦁	Pulse rate: 110 BPM – HIGH ");
                                DatabaseInstance.AddStats(2);
                                
                            }
                            break;
                    }
                    break;
            }

            ButtonsUntilContinue--;
            if(ButtonsUntilContinue==0)
            {
                if (DatabaseInstance.Level == 0 && DatabaseInstance.MainProgress == 9 && DatabaseInstance.nexttest == 0) AddContinueMessage("You have to take the Deck 1 Flashcards test",3); // to add it here, because I want it right after the second button pressed.
                if(CheckWhich==-1)this.transform.GetChild(0).GetComponent<UIButton>().interactable = true;
            }

        }

        public void RemoveContinueMessage()
        {
            CheckWhich = -1;
            this.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            if(ButtonsUntilContinue == 0) this.transform.GetChild(0).GetComponent<UIButton>().interactable = true;
        }

        private void AddContinueMessage(string message,int which) //0-2 - results, 3 - test
        {
            if(which >= 0 && which<=2)
            {
                CheckWhich = which;
            }
            if(which == 3)
            {
                DatabaseInstance.CanTakeTest = true;
            }
            this.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = message;
            this.transform.GetChild(0).GetComponent<UIButton>().interactable = false;
        }
        #region TextPicker
        private string PickText()
        {
            string rtstring = "Thanks for playing Level 1.";
            string Name = DatabaseInstance.Name;
            switch(DatabaseInstance.Level)
            {
                case 0:
                    switch(DatabaseInstance.MainProgress)
                    {
                        case 0:
                            rtstring = "Patient: Amanda Smith\nAmanda is a 18 year old female who has been feeling unwell lately and decides to go to the hospital for a checkup.";
                            break;
                        case 1:
                            rtstring = "Doctor " + Name + " : (Introduces self) Hi I am Dr. " + Name + " and I will be your doctor today.How have you been feeling?";
                            break;
                        case 2:
                            rtstring = "Amanda: Hello, Dr. " + Name + "! I have been feeling under the weather lately";
                            break;
                        case 3:
                            rtstring = "Doctor " + Name + ": Can you tell me about your symptoms?";
                            break;
                        case 4:
                            rtstring = "Amanda: I feel fatigued, tired all time and a little breathless.";
                            break;
                        case 5:
                            rtstring = "Doctor " + Name + ": Okay let’s begin with taking your vitals";
                            AddButton = true;
                            break;
                        case 9:
                            rtstring = "Doctor " + Name + ": Your pulse and blood pressure are very high. It could be serious or it could be nothing. But to be on the safe side, I would like you to get an x-ray and an ECG done.";
                            AddButton = true;
                            break;
                        case 12:
                            rtstring = "Doctor " + Name + ": Amanda, based on your x-ray results you have something called a ventricular heart defect. It is a congenital heart disease. It means that you have a hole between the right and left pumping chambers of the heart.";
                            break;
                        case 13:
                            rtstring = "Doctor " + Name + ": Normally the chambers of the heart are completely separated from each other by a wall called atrial and ventricular septum. But in your case, there are holes in the ventricular septum. VSD is the most common heart defect and it treatable.";
                            if(DatabaseInstance.nexttest == 1) AddContinueMessage("You have to take the Deck 2 Flashcards test", 3);
                            break;
                        case 14:
                            rtstring = "Doctor " + Name + ": As a patient of VSD, you need to watch you eat. Because of the high blood pressure you will need to follow a low sodium diet and incorporate green into your diet.";
                            break;
                        case 15:
                            rtstring = "Doctor " + Name + ": You also need to exercise daily as that will help you improve the strength of your heart. I will also prescribe medicines; digoxin will help with the irregular heartbeat and diuretics will help to increase the amount of water and salt expelled from the body as urine.";
                            break;
                        case 16:
                            rtstring = "Amanda: Thank you, Dr " + Name + ".";
                            break;
                        case 17:
                            rtstring = "DontAdd";
                            StartEndTest();
                            break;

                    }
                    break;
                


            }

            return rtstring;
        }


        #endregion
        #endregion

        #region EndTestFunctions

        private void StartEndTest()
        {
            this.transform.GetChild(0).GetComponent<UIButton>().interactable = false;
            this.GetComponent<ScrollRect>().enabled = false;
            correctanswers = 0;
            WhichQuestion = 0;
            PickQuestion();
            GameObject.Find("MainManager").GetComponent<MainManager>().OpenWindow(GameObject.Find("EndTestWindow").GetComponent<Window>());
            // String("Thanks for playing level 1. To play level 2, press Continue");
        }

        private void AddQuestion(bool type, string Question,string Answer1,string Answer2,string Answer3,string Answer4,int rightansw)
        {
            LastEndQuestion = GameObject.Find("EndTestWindow");


            LastEndQuestion.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Question;

            LastEndQuestion.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Answer1;
            LastEndQuestion.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = Answer2;


            if (!type)
            {
                LastEndQuestion.transform.GetChild(3).gameObject.SetActive(true);
                LastEndQuestion.transform.GetChild(4).gameObject.SetActive(true);
                LastEndQuestion.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = Answer3;
                LastEndQuestion.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>().text = Answer4;
            }
            else
            {
                LastEndQuestion.transform.GetChild(3).gameObject.SetActive(false);
                LastEndQuestion.transform.GetChild(4).gameObject.SetActive(false);
            }

            rightanswer = rightansw;
        }

        public void AnswerEndQuestion(int guess)
        {

            Debug.Log(guess + " a " + rightanswer + " c " + correctanswers);

            if (guess == rightanswer) correctanswers++;

            Debug.Log(guess + " a " + rightanswer + " c " + correctanswers);

            PickQuestion();
        }

        private void StopEndTest()
        {
            this.GetComponent<ScrollRect>().enabled = true;
            ClearChat();
            nextcordy = 1.01858f;
            this.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
            if (correctanswers >= 6)
            {
                
                AddMessage("Congratulations, you answered " + correctanswers + " / 10 questions correctly and passed the level.");
                DatabaseInstance.MainProgress = 0;
                DatabaseInstance.AddStats(3);
                if(DatabaseInstance.Level < DatabaseInstance.LastLevel)
                {
                    AddMessage("You completed the last level in the game.");
                }
                else this.transform.GetChild(0).GetComponent<UIButton>().interactable = true;
            }
            else
            {
                ClearChat();
                AddMessage("Sorry, but you failed the test with a score of " + correctanswers + " / 10 ");
                DatabaseInstance.MainProgress = 0;
                this.transform.GetChild(0).GetComponent<UIButton>().interactable = true;
            }


            GameObject.Find("MainManager").GetComponent<MainManager>().CloseCurrentWindow(true);
        }

        private void PickQuestion()
        {
            WhichQuestion++;

            switch(DatabaseInstance.Level)
            {
                case 0:
                    switch(WhichQuestion)
                    {
                        case 1:
                            AddQuestion(false, "What was Amanda Smith diagnosed with?", "Ventricular Septal Defect", "Spina Bifida", "Atrial Septal Defect", "Fever", 0);
                            break;
                        case 2:
                            AddQuestion(true, "True or False: Amanda’s had a fever.", "True", "False", "", "", 1);
                            break;
                        case 3:
                            AddQuestion(false, "Amanda was prescribed with:", "Digoxin", "Low sodium diet", "Exercise", "All of the above", 3);
                            break;
                        case 4:
                            AddQuestion(true, "True or False: The only way to treat VSD is surgery.", "True", "False", "", "", 0);
                            break;
                        case 5:
                            AddQuestion(true, "Ventricular Septal Defect is the most common heart defect", "True", "False", "", "", 0);
                            break;
                        case 6:
                            AddQuestion(false, "VSD are holes in the", "Right ventricle", "Atrial septum", "Ventricular septum", "Left ventricle", 2);
                            break;
                        case 7:
                            AddQuestion(false, "VSD stands for:", "Ventricle septum disc", "Ventricular septal defect", "Vents septs d", "Ventricular septum diagnosis", 1);
                            break;
                        case 8:
                            AddQuestion(true, "True or False: Tachycardia is a symptom of VSD", "True", "False", "", "", 0);
                            break;
                        case 9:
                            AddQuestion(false, "Amanda’s blood pressure was", "140mmHg / 90mmHg", "180mmhg / 60mmHg", "120mmHg / 100mmHg", "90mmHh / 70mmHg", 0);
                            break;
                        case 10:
                            AddQuestion(true, "Amanda’s pulse rate was", "78 BPM", "110 BPM", "72 BPM", "120 BPM", 1);
                            break;
                        case 11:
                            StopEndTest();
                            break;

                    }
                    break;
            }
        }





        #endregion

    }
}
