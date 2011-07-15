using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Threading;

namespace SRS_Server.Net
{
    [Serializable()]
    // This is cut down version of the userDetails, containing only the important information
    public class transferrableUserDetails
    {
        private string mUsername;
        private string mPassword;
        private string mDeviceOS;
        private string mUserRole;

        public string Username
        {
            get { return mUsername; }
            set { mUsername = value; }
        }
        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }
        public string DeviceOS
        {
            get { return mDeviceOS; }
            set { mDeviceOS = value; }
        }
        public string UserRole
        {
            get { return mUserRole; }
            set { mUserRole = value; }
        }
    }

    public class userDetails
    {

        private string mUsername;
        private string mPassword;
        private string mDeviceOS;
        private string mUserRole;
        private string mClass;
        private object mClient;

        // Question
        private question mCurrQuestion;
        private string mCurrQuestionString;     // Question in string format
        private string mChatMessage;

        private bool mUserListRequested;
        private bool mQuestionListRequested;
        private bool mConnected;
        public string Username
        {
            get { return mUsername; }
            set { mUsername = value; }
        }
        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }
        public string DeviceOS
        {
            get { return mDeviceOS; }
            set { mDeviceOS = value; }
        }
        public string UserRole
        {
            get { return mUserRole; }
            set { mUserRole = value; }
        }

        public string Class
        {
            get { return mClass; }
            set { mClass = value; }
        }  

        public object Client
        {
            get { return mClient; }
            set { mClient = value; }
        }

        public question CurrQuestion
        {
            get { return mCurrQuestion; }
            set { mCurrQuestion = value; }
        }

        public string ChatMessage
        {
            get { return mChatMessage; }
            set { mChatMessage = value; }
        }

        public string CurrQuestionString
        {
            get { return mCurrQuestionString; }
            set { mCurrQuestionString = value; }
        }

        public bool UserListRequested
        {
            get { return mUserListRequested; }
            set { mUserListRequested = value; }
        }

        public bool QuestionListRequested
        {
            get { return mQuestionListRequested; }
            set { mQuestionListRequested = value; }
        }

        public bool Connected
        {
            get { return mConnected; }
            set { mConnected = value; }
        }

        //Serialization function.
        // This may not be needed anymore, was used at some point in testing.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {

            info.AddValue("UserName", mUsername);
            info.AddValue("DeviceOS", mDeviceOS);
            info.AddValue("UserRole", mUserRole);
            info.AddValue("Client", mClient);
            info.AddValue("CurrentQuestion", mCurrQuestion);
        }
    }

    // Holds details about each tutor
    public class TutorDetails
    {
        private string mName;
        private string mPassword;
        private string mClass;
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }
        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }
        public string Class
        {
            get { return mClass; }
            set { mClass = value; }
        }
    }

    [Serializable()]
    public class UserManager
    {
        private Object mLock = new Object();
        private Mutex mThreadMutex = new Mutex();

        public Mutex ThreadMutex
        {
            get { return mThreadMutex; }
            set { mThreadMutex = value; }
        }

        private MessageLogger mMessageLogger;
        private XmlHandler mXmlHandler;

        private int mMaxUserKey;

        public int MaxUserKey
        {
            get { return mMaxUserKey; }
            set { mMaxUserKey = value; }
        }

        private Dictionary<int, userDetails> mUsersOnline;
        private Dictionary<int, TutorDetails> mTutors;

        public Dictionary<int, TutorDetails> Tutors
        {
            get { return mTutors; }
            set { mTutors = value; }
        }

        public Dictionary<int, userDetails> UsersOnline
        {
            get 
            { 
                lock (mLock) { return mUsersOnline; }
            }
            set
            {
                lock (mLock)
                { mUsersOnline = value; }
            }
        }

        private Dictionary<int, transferrableUserDetails> mUserDetailsToTransfer;

        public Dictionary<int, transferrableUserDetails> UserDetailsToTransfer
        {
            get { return mUserDetailsToTransfer; }
            set { mUserDetailsToTransfer = value; }
        }

        public UserManager(MessageLogger prMessageLogger)
        {
            mUsersOnline = new Dictionary<int, userDetails>();
            mMessageLogger = prMessageLogger;

            mXmlHandler = new XmlHandler();
        }

        public bool LoadTutors(string prFilename)
        {
            // Attempt to load settings from file
            mTutors = mXmlHandler.LoadUserSettings(prFilename);
            if (mTutors != null)
            {
                return true;
            }
            else
                return false;
        }

        public string ConvertQuestionToString(question prQuestion)
        {
            string iQuestionString;

            iQuestionString = "Q;" + prQuestion.QuestionID.ToString() + "|" + 
                prQuestion.QuestionType + "|" + prQuestion.Question + "|";

            // Some questions may not have answers
            if (prQuestion.PossibleAnswers != null)
            {
                foreach (string iPosAnswer in prQuestion.PossibleAnswers)
                {
                    iQuestionString += iPosAnswer;
                    if (iPosAnswer != "")
                    {
                        iQuestionString += ",";
                    }
                    else
                    {
                        iQuestionString = iQuestionString.Substring(0, iQuestionString.Length - 1);
                        break;
                    }
                }
            }

            iQuestionString += "|" + prQuestion.Answer + ";";

            return iQuestionString;
        }

        // Sets a question for each user
        public void SetQuestions(question prQuestion)
        {
            int i = 0;
            

            if (mUsersOnline.Count > 0)
            {
                while (i <= mMaxUserKey)
                {
                    if (mUsersOnline.ContainsKey(i))
                    {
                        // If the user is on an iOS device, iPod Touch/iPhone/iPad, the question needs to be
                        // sent using old string format; due to platforms lack of serialisation libraries.
                        if (mUsersOnline[i].DeviceOS == "iOS")
                        {
                            mUsersOnline[i].CurrQuestionString = ConvertQuestionToString(prQuestion);
                        }
                        else
                        {
                            lock (mLock)
                            {
                                mUsersOnline[i].CurrQuestion = new question();
                                mUsersOnline[i].CurrQuestion.Question = prQuestion.Question;
                                mUsersOnline[i].CurrQuestion.QuestionID = prQuestion.QuestionID;
                                mUsersOnline[i].CurrQuestion.QuestionType = prQuestion.QuestionType;
                                mUsersOnline[i].CurrQuestion.PossibleAnswers = prQuestion.PossibleAnswers;
                                mUsersOnline[i].CurrQuestion.Answer = prQuestion.Answer;
                            }
                        }
                    }
                    i++;
                }
            }
        }

        // Get the largest key in the user dictionary
        // Should really find a more efficient method
        private int GetLargestKeyUserDict()
        {
            int iCurrentLargestKey = 0;
            foreach(KeyValuePair<int, userDetails> iUserDetails in mUsersOnline)
            {
                if (iUserDetails.Key > iCurrentLargestKey)
                {
                    iCurrentLargestKey = iUserDetails.Key;
                }
            }
            return iCurrentLargestKey;
        }

        // Copy the important details from userDetails to transferrableUserDetails dictionaries
        private void ProcessUserDetails()
        {
            mUserDetailsToTransfer = new Dictionary<int, transferrableUserDetails>();
            transferrableUserDetails iTempDetails = new transferrableUserDetails();
            int i = 0;      // Current item to add

            // Loop through dictionary
            foreach (KeyValuePair<int, userDetails> iUserDetails in mUsersOnline)
            {
                iTempDetails = new transferrableUserDetails();
                iTempDetails.Username = iUserDetails.Value.Username;
                iTempDetails.DeviceOS = iUserDetails.Value.DeviceOS;
                iTempDetails.UserRole = iUserDetails.Value.UserRole;

                mUserDetailsToTransfer.Add(i, iTempDetails);
                i++;
            }
        }

        // Send the current user details to tutors
        public void SendUserListToTutors()
        {
            ProcessUserDetails();
            int i = 0;
            int iNumUsers = 0;

            iNumUsers = mMaxUserKey;

            // Hack to fix the bug that stops the server from sending userlist update 
            //when there there's only 1 user online.
            if (mUsersOnline.Count == 1)
            {
                i = mUsersOnline.First().Key;
            }

            while (i <= iNumUsers)
            {
                // Only send the list to a tutor
                if (mUsersOnline.ContainsKey(i))
                {
                    if (mUsersOnline[i].UserRole == "Tutor")
                    {
                        lock (mLock)
                        {
                            mUsersOnline[i].UserListRequested = true;
                        }
                    }
                }
                i++;
            }
            
        }

        // Check if the tutors list contains the provided name
        public bool IsUserATutor(string prUsername)
        {
            if (mTutors != null)
            {
                foreach (KeyValuePair<int, TutorDetails> iTutors in mTutors)
                {
                    if (iTutors.Value.Name == prUsername)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Check the password against a user
        public bool VerifyPassword(userDetails prUser)
        {
            foreach (KeyValuePair<int, TutorDetails> iUser in mTutors)
            {
                // Find the username
                if (iUser.Value.Name == prUser.Username)
                {
                    // Check the pass
                    if (iUser.Value.Password == prUser.Password)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Get number of users online
        public int GetNumOfUsers()
        {
            int iCount;
            //mThreadMutex.WaitOne();
            iCount = mUsersOnline.Count();
            //mThreadMutex.ReleaseMutex();

            return iCount;
        }

        // Gets a new free ID
        public int GetNewUserID(int prExtraIndex)
        {
            if (mUsersOnline.Count == 0)
                return 0;
            else
                return mUsersOnline.Last().Key + 1 + prExtraIndex;
        }
        
        public void AddNewUser(userDetails prUser)
        {
            int iNewID = GetNewUserID(0);
            int iExtra = 0;

            // Make sure the generated ID is not already in use
            while (true)
            {
                if (mUsersOnline.ContainsKey(iNewID))
                {
                    iExtra++;
                    iNewID = GetNewUserID(iExtra);
                }
                else
                    break;
            }

            mUsersOnline.Add(iNewID, prUser);

            // Update max user key
            mMaxUserKey = GetLargestKeyUserDict();

            SendUserListToTutors();
        }

        // Add a new tutor to the list
        public void AddTutor(TutorDetails prTutor)
        {
            if (mTutors == null)
                mTutors = new Dictionary<int, TutorDetails>();

            if (mTutors.Count == 0)
                mTutors.Add(0, prTutor);
            else
                mTutors.Add(mTutors.Last().Key + 1, prTutor);
        }
        
        // Remove a tutor from the list
        public void RemoveTutor(string prTutorName)
        {
            int iTutorID = 0;

            foreach (KeyValuePair<int, TutorDetails> iTutor in mTutors)
            {
                if (iTutor.Value.Name == prTutorName)
                {
                    iTutorID = iTutor.Key;
                    break;
                }
            }

            mTutors.Remove(iTutorID);
        }

        // Gets the tutor object for the provided name
        public TutorDetails GetTutorDetails(string prName)
        {
            foreach (KeyValuePair<int, TutorDetails> iTutor in mTutors)
            {
                if (iTutor.Value.Name == prName)
                    return iTutor.Value;
            }
            return null;
        }

        // Remove a user 
        public void RemoveUser(TcpClient prClient)
        {
            // Find the key
            int iKey = 0;

            foreach (KeyValuePair<int, userDetails> iUser in mUsersOnline)
            {
                if (iUser.Value.Client == prClient)
                {
                    iKey = iUser.Key;
                    break;
                }
            }

            //mThreadMutex.WaitOne();
            lock (mLock)
            {
                mUsersOnline.Remove(iKey);
            }
            

            //mThreadMutex.ReleaseMutex();
            
        }

        // Kick all connected users from server
        public void KickUsers()
        {
            //mThreadMutex.WaitOne();
            lock (mLock)
            {
                mUsersOnline.Clear();
            }
            //mThreadMutex.ReleaseMutex();
        }

        // Gets the username for the given client
        public string GetUsernameByClient(TcpClient prClient)
        {
            foreach (KeyValuePair<int, userDetails> iUser in mUsersOnline)
            {
                if (iUser.Value.Client == prClient)
                {
                    return iUser.Value.Username;
                }
            }
            return "Unknown";
        }
        
        // Gets the user details for the given client
        public userDetails GetUserByClient(TcpClient prClient)
        {
            foreach (KeyValuePair<int, userDetails> iUser in mUsersOnline)
            {
                if (iUser.Value.Client == prClient)
                {
                    return iUser.Value;
                }
            }
            return new userDetails();
        }

        // Check if the username is available
        public bool IsUsernameAvailable(string prUsername)
        {
            foreach (KeyValuePair<int, userDetails> iUser in mUsersOnline)
            {
                if (iUser.Value.Username == prUsername)
                {
                    return false;
                }
            }
            return true;
        }
        
        //Serialisation function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {

            info.AddValue("UserList", mUsersOnline);
        }
    }
}
