using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace srs_server
{
    // Holds information about an instruction to be executed
    public class Instruction
    {
        // Probably a shit method of doing this
        // Flags for sending
        private bool isSending;
        private bool isQuestion;
        private bool isSendingToAll;

        // Flags for deleting
        private bool isDeleting;

        // Flags for modifying
        private bool isModifying;
        // Misc
        private bool isResponseTimeUp;   // If flagged stops the server processing new responses

        private string mQuestionName;

        public string QuestionName
        {
            get { return mQuestionName; }
            set { mQuestionName = value; }
        }

        public bool IsQuestion
        {
            get { return isQuestion; }
            set { isQuestion = value; }
        }
        public bool IsSending
        {
            get { return isSending; }
            set { isSending = value; }
        }
        public bool IsSendingToAll
        {
            get { return isSendingToAll; }
            set { isSendingToAll = value; }
        }

        public bool IsDeleting
        {
            get { return isDeleting; }
            set { isDeleting = value; }
        }
        public bool IsModifying
        {
            get { return isModifying; }
            set { isModifying = value; }
        }
        public bool IsResponseTimeUp
        {
            get { return isResponseTimeUp; }
            set { isResponseTimeUp = value; }
        }
        public Instruction()
        {
            isSending = false;
            isQuestion = false;
            isSendingToAll = false;
            isResponseTimeUp = false;
            isModifying = false;
        }
    }

    // Parser is extremely simple and has been designed as I go along. A lot of functionality is hardcoded 
    // and the method is likely not an efficient one.
    public class InstructionParser
    {
        QuestionManager mQuestionManager;
        AnswerManager mAnswerManager;

        UserManager mUserManager;
        string[] mInstructionArray = new string[8];     // Holds all the commands, currently 8 max

        public InstructionParser(IOProcessor prIOProcessor)
        {
            mQuestionManager = prIOProcessor.QuestionManager;
            mUserManager = prIOProcessor.UserManager;
            mAnswerManager = prIOProcessor.AnswerManager;
        }

        // Start parsing the string
        // First pass looks for each command
        public void ParseString(string prInstructionString)
        {
            Instruction iNewInstructions = new Instruction();

            // Split the string into an array
            mInstructionArray = prInstructionString.Split(',');

            foreach (string iCommand in mInstructionArray)
            {
                switch (iCommand.ToUpper())
                {
                    case "SEND":
                        iNewInstructions.IsSending = true;
                        break;
                    case "QUESTION":
                        iNewInstructions.IsQuestion = true;
                        break;
                    case "ALL":
                        iNewInstructions.IsSendingToAll = true;
                        break;
                    case "TIMEUP":
                        iNewInstructions.IsResponseTimeUp = true;
                        break;
                    case "DELETE":
                        iNewInstructions.IsDeleting = true;
                        break;
                    case "MODIFY":
                        iNewInstructions.IsModifying = true;
                        break;
                    default:
                        iNewInstructions.QuestionName = iCommand.Replace("\"", "");
                        break;
                }
            }
            ExecuteInstructions(iNewInstructions);
        }

        // Executes the instructions
        private void ExecuteInstructions(Instruction prInstructions)
        {
            // Sending instructions
            if (prInstructions.IsSending)
            {
                SendingInstructions(prInstructions);
            }
            else if (prInstructions.IsDeleting)
            {
                DeleteInstructions(prInstructions);
            }
            else if (prInstructions.IsResponseTimeUp)
            {
                StopReceivingResponses();
            }
        }

        // Instructions related to sending stuff
        private void SendingInstructions(Instruction prInstructions)
        {
            question iQuestionToSend;

            // Send a question and get ready to recieve responses
            if (prInstructions.QuestionName != "")
            {
                iQuestionToSend = mQuestionManager.GetQuestionByName(prInstructions.QuestionName);

                if (iQuestionToSend != null)
                {
                    if (prInstructions.IsSendingToAll)
                    {
                        mAnswerManager.ClearAnswerList();           // Clear existing answers
                        mUserManager.SetQuestions(iQuestionToSend); // Queue the question to be sent
                        mAnswerManager.ReceiveResponses = true;     // Allow responses to be recieved
                    }
                }
            }
        }

        // Instructions for deleting a question
        private void DeleteInstructions(Instruction prInstructions)
        {
            question iQuestionToDelete;

            if (prInstructions.QuestionName != "")
            {
                iQuestionToDelete = mQuestionManager.GetQuestionByName(prInstructions.QuestionName);

                if (iQuestionToDelete != null)
                {
                    mQuestionManager.DeleteQuestion(iQuestionToDelete);
                }
            }

            mQuestionManager.SendQuestionListToTutors();
        }

        // Stop the server from receiving answers from students
        private void StopReceivingResponses()
        {
            mAnswerManager.ReceiveResponses = false;
        }
    }

    public class IOProcessor
    {
        MessageLogger mMessageLogger;
        QuestionManager mQuestionManager;
        UserManager mUserManager;
        AnswerManager mAnswerManager;
        chatMessage mNewChatMessage;
        ChatManager mChatManager;
        Encryption mEncryption;

        DictionarySerialiserMethods mSerialiserMethods;

        public UserManager UserManager
        {
            get { return mUserManager; }
            set { mUserManager = value; }
        }
        public QuestionManager QuestionManager
        {
            get { return mQuestionManager; }
            set { mQuestionManager = value; }
        }
        public AnswerManager AnswerManager
        {
            get { return mAnswerManager; }
            set { mAnswerManager = value; }
        }
        public IOProcessor(MessageLogger prMessageLogger, UserManager prUserManager, 
            QuestionManager prQuestionManager, AnswerManager prAnswerManager, ChatManager prChatManager)
        {
            mMessageLogger = prMessageLogger;
            mUserManager = prUserManager;
            mQuestionManager = prQuestionManager;
            mAnswerManager = prAnswerManager;
            mChatManager = prChatManager;
            mEncryption = new Encryption();
            mSerialiserMethods = new DictionarySerialiserMethods();
        }

        public string RemoveFrontCharacters(string prString, int prNumToRemove)
        {
            int iCount = 0;
            int iStringLength = prString.Length;
            char[] iStringArray = new char[300];
            //string iProcessedString;

            for (int i = 0; i < iStringLength; i++)
            {
                if (i >= prNumToRemove)
                {
                    if (prString[i] != ';')
                    {
                        iStringArray[iCount] = prString[i];
                        iCount++;
                    }
                    else
                    {
                        // iProcessedString = new string(iStringArray);
                        iStringArray[iCount] = prString[i];
                        return new string(iStringArray);
                    }
                }
            }
            return new string(iStringArray);
        }

        public void ParseNewString(string prString, userDetails prUser)
        {
            string iContents = GetDataContents(prString);

            switch (iContents)
            {
                case "i":
                    ProcessInstructionString(RemoveFrontCharacters(prString, 2), prUser);
                    break;
                case "c":
                    ProcessChatString(prString, prUser);
                    break;
                case "Q":   // Old question parsing, to be removed
                    ProcessQuestionString(RemoveFrontCharacters(prString, 2), prUser);
                    break;
                case "q":
                    ProcessNewQuestion(prString);
                    break;
                case "A":
                    ProcessAnswerString(RemoveFrontCharacters(prString, 2), prUser);
                    break;
                case "u":
                    ProcessUserDetails(prString, prUser);
                    break;
                case "U":
                    ProcessUserDetailsByString(RemoveFrontCharacters(prString, 2), prUser);
                    break;
            }
        }

        // Work out what the data is
        private string GetDataContents(string prData)
        {
            //string iDataString = new string(Encoding.ASCII.GetChars(prData)).Replace("\0", "");
            char[] iDataTypeArray = new char[2];
            string iDataType;

            // Loop through the first 2 chars
            for (int i = 0; i < 2; i++)
            {
                iDataTypeArray[i] = prData[i];
            }

            iDataType = new string(iDataTypeArray);

            // XML
            if (iDataType == "<?")
            {
                if (Regex.IsMatch(prData, "transferrableUserDetails"))
                {
                    return "u";
                }
                else if (Regex.IsMatch(prData, "question"))
                {
                    return "q";
                }
                else if (Regex.IsMatch(prData, "ChatMessage")) 
                {
                    return "c";
                }
                else
                {
                    return "?";
                }
            }
            // String based input
            else
            {
                string iFlag = prData[0].ToString() + prData[1].ToString();
                switch (iFlag)
                {
                    case "I;":      // Instructions
                        return "i";
                    case "Q;":      // Old question, to be removed
                        return "Q";
                    case "A;":      // Old answer, to be removed
                        return "A";
                    case "U;":
                        return "U"; // Old user details, to be removed
                }
            }
            return "?";
        }

        // Interprets instructions sent from command line
        private void ProcessInstructionString(string prString, userDetails prUser)
        {
            InstructionParser iParser = new InstructionParser(this);

            string iInstructions;
            string iResult = "";
            char[] iStringArray = new char[prString.Length];

            for (int i = 0; i < prString.Length; i++)
            {
                if (prString[i] != ';')
                {
                    iStringArray[i] = prString[i];
                }
            }

            iInstructions = new string(iStringArray).Replace("\0", "");

            iParser.ParseString(iInstructions);

            // TODO: Proper parsing.
            // List all questions
            if (iInstructions == "list questions" || iInstructions == "list q")
            {
                mMessageLogger.NewMessage("Questions:", mMessageLogger.MESSAGE_SERVER);

                if (mQuestionManager.QuestionList.Count > 0)
                {
                    for (int q = 1; q <= mQuestionManager.QuestionList.Count; q++)
                    {
                        iResult += mQuestionManager.QuestionList[q].Question;
                        mMessageLogger.NewMessage(iResult, mMessageLogger.MESSAGE_COMMAND);
                        iResult = "";
                    }
                }
            }
            // List all users
            if (iInstructions == "list users" || iInstructions == "list u")
            {
                if (mUserManager.UsersOnline.Count > 0)
                {
                    mMessageLogger.NewMessage("Connected Users:", mMessageLogger.MESSAGE_COMMAND);

                    for (int q = 1; q <= mUserManager.UsersOnline.Count; q++)
                    {
                        iResult += mUserManager.UsersOnline[q].Username;
                        mMessageLogger.NewMessage(iResult, mMessageLogger.MESSAGE_COMMAND);
                        iResult = "";
                    }
                }
                else
                {
                    mMessageLogger.NewMessage("No users connected", mMessageLogger.MESSAGE_COMMAND);
                }
            }
        }

        private void ProcessChatString(string prString, userDetails prUser)
        {
            string iChatString = prString.Replace("\0", "");
            ChatMessage iNewMessageReceived = mSerialiserMethods.ConvertStringToChatMessage(iChatString);

            mMessageLogger.ChatMessageQueue.Enqueue(iNewMessageReceived);
            mMessageLogger.NewMessage(iNewMessageReceived.From + ": " + iNewMessageReceived.Message, mMessageLogger.MESSAGE_CHAT);
        }

        // Old question parsing, to be removed
        private void ProcessQuestionString(string prString, userDetails prUser)
        {
            bool iQuestionFound = false;
            bool iQuestionTypeFound = false;
            char[] iQuestionString = new char[50];
            char[] iQuestionType = new char[2];
            int iCurrPos = 0;

            question iNewQuestion = new question();

            for (int i = 0; i < prString.Length; i++)
            {
                if (prString[i] != ';')
                {
                    if (!iQuestionTypeFound)
                    {
                        if (prString[i] != '|')
                        {
                            iQuestionType[iCurrPos] = prString[i];
                            iCurrPos++;
                        }
                        else
                        {
                            iQuestionTypeFound = true;
                            iCurrPos = 0;
                        }
                    }
                    else
                    {
                        if (!iQuestionFound)
                        {
                            if (prString[i] != '|')
                            {
                                iQuestionString[iCurrPos] = prString[i];
                                iCurrPos++;
                            }
                            else
                            {
                                iQuestionFound = true;
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            if (mQuestionManager.IsListEmpty())
            {
                iNewQuestion.QuestionID = 1;
            }
            else
            {
                iNewQuestion.QuestionID = mQuestionManager.GetNewQuestionID();
            }

            iNewQuestion.Question = new string(iQuestionString).Replace("\0", "");
            iNewQuestion.QuestionType = new string(iQuestionType);
            iNewQuestion.QuestionString = "Q;" + mQuestionManager.InsertQuestionNumber(prString.Replace("\0", ""), 1);

            mQuestionManager.AddNewQuestion(iNewQuestion);
            mMessageLogger.NewMessage(iNewQuestion.Question + " added", mMessageLogger.MESSAGE_QUESTION);
        }

        // New implementation of question input using XML serialisation
        private void ProcessNewQuestion(string prQuestionArray)
        {
            question iNewQuestion = mSerialiserMethods.ConvertStringToQuestion(prQuestionArray);

            // Set the question ID
            if (mQuestionManager.QuestionList.Count > 0)
                iNewQuestion.QuestionID = mQuestionManager.QuestionList.Last().Key + 1;
            else
                 iNewQuestion.QuestionID = 0;

            // Determine whether the question is being added or modified
            // TODO: Implement a better system to do this
            if (mQuestionManager.IsQuestionNameInUse(iNewQuestion.Question))
            {
                // Remove the old question and add the new one
                mQuestionManager.RemoveQuestion(iNewQuestion.Question);
                mQuestionManager.AddNewQuestion(iNewQuestion);
            }
            mQuestionManager.AddNewQuestion(iNewQuestion);
            mMessageLogger.NewMessage(iNewQuestion.Question + " added", mMessageLogger.MESSAGE_QUESTION);

        }

        private void ProcessAnswerString(string prString, userDetails prUser)
        {
            char[] iAnswerArray = new char[150];        // The max char size for all answers is currently 150
            char[] iQuestionIDArray = new char[5];      // allow for up to 99999 ID's
            bool iQuestionIDFound = false;
            bool iAnswerFound = false;
            int iCurrPos = 0;
            Answer iNewAnswer = new Answer();

            if (mAnswerManager.ReceiveResponses)
            {
                for (int i = 0; i < prString.Length; i++)
                {
                    if (prString[i] != ';')
                    {
                        if (!iQuestionIDFound)
                        {
                            if (prString[i] != '|')
                            {
                                iQuestionIDArray[iCurrPos] = prString[i];
                                iCurrPos++;
                            }
                            else
                            {
                                iQuestionIDFound = true;
                                iCurrPos = 0;
                            }
                        }
                        else
                        {
                            if (!iAnswerFound)
                            {
                                if (prString[i] != '|')
                                {
                                    iAnswerArray[iCurrPos] = prString[i];
                                    iCurrPos++;
                                }
                                else
                                {
                                    iAnswerFound = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                iNewAnswer.Username = prUser.Username;
                iNewAnswer.AnswerString = new string(iAnswerArray).Replace("\0", "");
                iNewAnswer.QuestionID = Convert.ToInt32(new string(iQuestionIDArray).Replace("\0", ""));
                iNewAnswer.AnswerSent = false;

                mAnswerManager.AddAnswer(iNewAnswer);

                mMessageLogger.NewMessage(prUser.Username + " answered " + iNewAnswer.AnswerString + " for "
                    + mQuestionManager.GetQuestionStringByID(iNewAnswer.QuestionID), mMessageLogger.MESSAGE_ANSWER);
            }
            else
            {
                mMessageLogger.NewMessage("Response received after Time", mMessageLogger.MESSAGE_SERVER);
            }
        }

        // Old string based user string
        private void ProcessUserDetailsByString(string prString, userDetails prUser)
        {
            int iPos = 0;
            int iCurrPos = 0;
            int iSection = 0;

            char[] iUsernameArray, iDeviceOSArray, iUserRoleArray;

            iUsernameArray = new char[40];
            iDeviceOSArray = new char[15];
            iUserRoleArray = new char[8];

            // Changed to allow for easier addition of information
            for (int i = 0; i < prString.Length; i++)
            {
                if (prString[i] != ';')
                {
                    switch (iSection)
                    {
                        case 0:     // User Role
                            if (prString[i] != '|')
                            {
                                iUserRoleArray[iCurrPos] = prString[i];
                                iCurrPos++;
                            }
                            else
                            {
                                iCurrPos = 0;
                                iSection++;
                            }
                            break;
                        case 1:     // Username
                            if (prString[i] != '|')
                            {
                                iUsernameArray[iCurrPos] = prString[i];
                                iCurrPos++;
                            }
                            else
                            {
                                iSection++;
                                iCurrPos = 0;
                            }
                            break;
                        case 2:     // Device OS
                            if (prString[i] != '|')
                            {
                                iDeviceOSArray[iCurrPos] = prString[i];
                                iCurrPos++;
                            }
                            else
                            {
                                iSection++;
                                iCurrPos = 0;
                            }
                            break;
                    }
                }
                else
                {
                    break;
                }
            }
            prUser.UserRole = new string(iUserRoleArray).Replace("\0", "");
            prUser.DeviceOS = new string(iDeviceOSArray).Replace("\0", "");
            prUser.Username = new string(iUsernameArray).Replace("\0", "");
            prUser.CurrQuestion = new question();
            if (prUser.UserRole != "Tutor")
            {
                mMessageLogger.NewMessage(prUser.Username + " joined the server", mMessageLogger.MESSAGE_STUDENT);
            }
            else
            {
                mMessageLogger.NewMessage(prUser.Username + " joined the server", mMessageLogger.MESSAGE_TUTOR);
            }

            //mUserManager.AddNewUser(prUser);
            //mQuestionManager.SendQuestionListToTutors();

            RespondToConnection(prUser);
        }

        // New XML serialisation method for processing user details
        private void ProcessUserDetails(string prString, userDetails prUser)
        {
            transferrableUserDetails iUserDetailsFromClient = mSerialiserMethods.ConvertStringToUserDetails(prString);

            prUser.UserRole = iUserDetailsFromClient.UserRole;
            prUser.DeviceOS = iUserDetailsFromClient.DeviceOS;
            prUser.Username = iUserDetailsFromClient.Username;
            if (iUserDetailsFromClient.Password != "")
                prUser.Password = mEncryption.Decrypt(iUserDetailsFromClient.Password, "P@ssword1");

            prUser.CurrQuestion = new question();

            if (prUser.UserRole != "Tutor")
            {
                mMessageLogger.NewMessage(prUser.Username + " joined the server", mMessageLogger.MESSAGE_STUDENT);
            }
            else
            {
                mMessageLogger.NewMessage(prUser.Username + " joined the server", mMessageLogger.MESSAGE_TUTOR);
            }

            RespondToConnection(prUser);
        }

        private void RespondToConnection(userDetails prUser)
        {
            // Make sure the user name is not in use.
            if (mUserManager.IsUsernameAvailable(prUser.Username))
            {
                // Check if the user is a tutor or student
                if (mUserManager.IsUserATutor(prUser.Username))
                {
                    // Check password
                    if (mUserManager.VerifyPassword(prUser))
                    {
                        prUser.Connected = true;
                        mUserManager.AddNewUser(prUser);
                        mQuestionManager.SendQuestionListToTutors();

                        // Send a response to client
                        SendConnectionResponse(prUser.Client, "TUTORCONNECTED");
                    }
                    else
                    {
                        prUser.Connected = false;
                        // Send a response to client
                        SendConnectionResponse(prUser.Client, "INCORRECTPASS");
                    }
                }
                else
                {
                    prUser.Connected = true;
                    mUserManager.AddNewUser(prUser);
                    mQuestionManager.SendQuestionListToTutors();

                    // Send a response to client
                    SendConnectionResponse(prUser.Client, "STUDENTCONNECTED");
                }

            }
            else
            {
                prUser.Connected = false;
                // Send a response to client
                SendConnectionResponse(prUser.Client, "USERNAMETAKEN");
            }
        }

        // Send connection response to client
        private void SendConnectionResponse(object prClient, string prConnectionMessage)
        {
            NetworkStream iClientStream;
            TcpClient iClient;

            char[] iData = System.Text.Encoding.ASCII.GetChars(System.Text.Encoding.ASCII.GetBytes(prConnectionMessage));
            byte[] iConnectionMessage = System.Text.Encoding.ASCII.GetBytes(iData);

            iClient = (TcpClient)prClient;

            // Make sure the client is still connected
            iClientStream = iClient.GetStream();

            if (iConnectionMessage != null)
            {
                // Seems to have fixed, or greatly reduced the chance that the response gets lost.
                while (true)
                {
                    if (iClientStream.CanWrite)
                    {
                        iClientStream.Write(iConnectionMessage, 0, iConnectionMessage.Length);
                        break;
                    }
                }
            }

            Array.Clear(iConnectionMessage, 0, iConnectionMessage.Length);
        }
    }
}
