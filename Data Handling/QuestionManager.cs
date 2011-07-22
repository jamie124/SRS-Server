using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Net.Sockets;

namespace srs_server
{
    [Serializable()]
    public class question
    {
        private int mQuestionID;
        private string mQuestion;
        private string mQuestionString;
        private string mQuestionType;
        private string[] mPossibleAnswers;
        private string mAnswer;

        public int QuestionID
        {
            get { return mQuestionID; }
            set { mQuestionID = value; }
        }
        public string Question
        {
            get { return mQuestion; }
            set { mQuestion = value; }
        }
        public string QuestionString
        {
            get { return mQuestionString; }
            set { mQuestionString = value; }
        }
        public string QuestionType
        {
            get { return mQuestionType; }
            set { mQuestionType = value; }
        }
        public string[] PossibleAnswers
        {
            get { return mPossibleAnswers; }
            set { mPossibleAnswers = value; }
        }

        public string Answer
        {
            get { return mAnswer; }
            set { mAnswer = value; }
        }
    }

    [Serializable()]
    public class QuestionManager
    {
        UserManager mUserManger;

        Dictionary<int, question> mQuestionList;

        internal Dictionary<int, question> QuestionList
        {
            get { return mQuestionList; }
            set { mQuestionList = value; }
        }

        private int mLastQuestionAddedToList;

        public QuestionManager(UserManager prUserManager)
        {
            mQuestionList = new Dictionary<int, question>();
            mLastQuestionAddedToList = 0;
            mUserManger = prUserManager;
        }

        // Send question data to tutors
        public void SendQuestionListToTutors()
        {
            int i = 0;
            if (mUserManger.UsersOnline.Count > 0)
            {
                while (i <= mUserManger.MaxUserKey)
                {
                    // Only send the list to a tutor
                    if (mUserManger.UsersOnline.ContainsKey(i))
                    {
                        if (mUserManger.UsersOnline[i].UserRole == "Tutor")
                        {
                            mUserManger.UsersOnline[i].QuestionListRequested = true;
                        }
                    }
                    i++;
                }
            }
        }

        // Send question data to a specific tutor
        public void SendQuestionListToTutor(TcpClient prClient)
        {
            int i = 0;
            if (mUserManger.UsersOnline.Count > 0)
            {
                while (i < mUserManger.MaxUserKey)
                {
                    // Only send the list to a tutor
                    if (mUserManger.UsersOnline.ContainsKey(i))
                    {
                        if (mUserManger.UsersOnline[i].UserRole == "Tutor" && mUserManger.UsersOnline[i].Client == prClient)
                        {
                            mUserManger.UsersOnline[i].QuestionListRequested = true;
                        }
                    }
                    i++;
                }
            }
        }

        // Insert the question number into the string
        public string InsertQuestionNumber(string prQuestionString, int prQuestionNum)
        {
            return prQuestionString.Insert(0, prQuestionNum.ToString() + "|");
        }

        public bool AddNewQuestion(question prQuestion)
        {
            if (!mQuestionList.ContainsValue(prQuestion))
            {
                if(!mQuestionList.ContainsKey(prQuestion.QuestionID))
                    mQuestionList.Add(prQuestion.QuestionID, prQuestion);
                else
                    mQuestionList.Add(mQuestionList.Keys.Last() + 1, prQuestion);
                SendQuestionListToTutors();
                return true;
            }
            else
            {
                return false;
            }
        }

        // Remove the requested question
        public bool RemoveQuestion(string prQuestionName)
        {
            // Get the ID of the question
            int iQuestionID = -1;

            foreach (KeyValuePair<int, question> iQuestion in mQuestionList)
            {
                if (iQuestion.Value.Question == prQuestionName)
                {
                    iQuestionID = iQuestion.Key;
                    break;
                }
            }

            // Remove the question
            if (iQuestionID > -1)
            {
                mQuestionList.Remove(iQuestionID);
                return true;
            }
            else
            {       
                return false;
            }
        }

        // Delete a question from the dictionary
        public bool DeleteQuestion(question prQuestion)
        {
            int iQuestionIndex = 0;

            if (mQuestionList.ContainsValue(prQuestion))
            {
                foreach (KeyValuePair<int, question> iQuestion in mQuestionList)
                {
                    if (iQuestion.Value == prQuestion)
                    {
                        iQuestionIndex = iQuestion.Key;
                        break;
                    }
                }

                mQuestionList.Remove(iQuestionIndex);

                return true;
            }
            else
            {
                return false;
            }
        }

        // Gets the largest key in question dictionary
        private int GetLargestKeyQuestionDict()
        {
            int iCurrentLargestKey = 0;
            foreach (KeyValuePair<int, question> iQuestion in mQuestionList)
            {
                if (iQuestion.Key > iCurrentLargestKey)
                {
                    iCurrentLargestKey = iQuestion.Key;
                }
            }
            return iCurrentLargestKey;
        }

        public int GetNewQuestionID()
        {
            int iID = 0;

            iID = mQuestionList[mQuestionList.Last().Key].QuestionID + 1;
            return iID;
        }

        public bool IsListEmpty()
        {
            if (mQuestionList.Count == 0)
                return true;
            else
                return false;
        }

        public string GetLastQuestionAdded()
        {
            int i = mLastQuestionAddedToList;
           
            string iQuestionType = "";

            if (mQuestionList.ContainsKey(i))
            {
                switch (mQuestionList[i].QuestionType)
                {
                    case "MC":
                        iQuestionType = "Multi-Choice";
                        break;
                    case "SA":
                        iQuestionType = "Short Answer";
                        break;
                    case "TF":
                        iQuestionType = "True/False";
                        break;
                    case "MA":
                        iQuestionType = "Matching";
                        break;
                }

                mLastQuestionAddedToList++;
                return mQuestionList[i].Question + " - " + iQuestionType;
            }
            mLastQuestionAddedToList++;
            return "";
        }

        // Check if a new question is available
        public bool IsNewQuestionAvailable()
        {
            if (GetLargestKeyQuestionDict() >= mLastQuestionAddedToList)
                return true;
            else
                return false;
        }

        // Get the requested question object
        public question GetQuestionByID(int prQuestionID)
        {
            question iQuestion = mQuestionList[prQuestionID];
            return iQuestion;
        }

        // Returns the question id for provided questionname
        public int GetQuestionIDByString(string prQuestionName)
        {
            foreach (KeyValuePair<int, question> iQuestion in mQuestionList)
            {
                if (iQuestion.Value.Question == prQuestionName)
                {
                    return iQuestion.Value.QuestionID;
                }
            }
            // Question not found
            return -1;
        }

        // Checks if the question is already in use
        public bool IsQuestionNameInUse(string prQuestionName)
        {
            foreach (KeyValuePair<int, question> iQuestion in mQuestionList)
            {
                if (iQuestion.Value.Question == prQuestionName)
                    return true;
            }
            return false;
        }

        // Get question by name
        public question GetQuestionByName(string prQuestionName)
        {
            foreach (KeyValuePair<int, question> iQuestion in mQuestionList)
            {
                if (iQuestion.Value.Question == prQuestionName)
                {
                    return iQuestion.Value;
                }
            }
            // Question not found
            return null;
        }

        // Get the the requested question 
        public string GetQuestionStringByID(int prQuestionID)
        {
            return mQuestionList[prQuestionID].Question;
        }

        //Serialisation function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {

            info.AddValue("UserList", mQuestionList);
        }
    }
}
