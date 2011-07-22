using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace srs_server
{
    // Currently the answer is only tied to the user by the username
    // further developments may require more data.
    public class Answer
    {
        private int mQuestionID;
        private string mAnswer;
        private string mUsername;
        private bool mAnswerSent;   // Has the answer been sent to the tutor?

        public bool AnswerSent
        {
            get { return mAnswerSent; }
            set { mAnswerSent = value; }
        }

        public string Username
        {
            get { return mUsername; }
            set { mUsername = value; }
        }

        public int QuestionID
        {
            get { return mQuestionID; }
            set { mQuestionID = value; }
        }

        public string AnswerString
        {
            get { return mAnswer; }
            set { mAnswer = value; }
        }
    }

    public class AnswerManager
    {

        private Dictionary<int, Answer> mAnswersList = new Dictionary<int, Answer>();

        private bool mReceiveResponses;     // Tells the server if it can process responses from client

        public bool ReceiveResponses
        {
            get { return mReceiveResponses; }
            set { mReceiveResponses = value; }
        }
        public Dictionary<int, Answer> AnswersList
        {
            get { return mAnswersList; }
            set { mAnswersList = value; }
        }

        public AnswerManager()
        {
            mReceiveResponses = true;
        }

        public void AddAnswer(Answer prAnswer)
        {
            int iNumberToTry;
            bool iAnswerAdded = false;

            if (mAnswersList.Count == 0)
                mAnswersList.Add(0, prAnswer);
            else
            {
                iNumberToTry = mAnswersList.Last().Key;

                while (!iAnswerAdded)
                {
                    if (!mAnswersList.ContainsKey(iNumberToTry))
                    {
                        mAnswersList.Add(iNumberToTry, prAnswer);
                        iAnswerAdded = true;
                    }
                    else
                    {
                        iNumberToTry++;
                    }
                }
                
            }
        }

        // Clear the servers copy version of the answer list
        public void ClearAnswerList()
        {
            if (mAnswersList.Count > 0)
                mAnswersList.Clear();
        }

        private int GetLargestKeyAnswerDict()
        {
            int iCurrentLargestKey = 0;
            foreach (KeyValuePair<int, Answer> iAnswer in mAnswersList)
            {
                if (iAnswer.Key > iCurrentLargestKey)
                {
                    iCurrentLargestKey = iAnswer.Key;
                }
            }
            return iCurrentLargestKey;
        }

        // Delete answers for a certain question
        public void DeleteAnswersForQuestion(int prQuestionID)
        {
            int iLargestKey = GetLargestKeyAnswerDict();

            if (mAnswersList.Count > 0)
            {
                for (int i = 0; i <= iLargestKey; i++)
                {
                    if (mAnswersList[i] != null)
                    {
                        if (mAnswersList[i].QuestionID == prQuestionID)
                        {
                            mAnswersList.Remove(i);
                        }
                    }
                }
            }
        }
    }
}
