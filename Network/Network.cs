using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace SRS_Server.Net
{
    public class Network
    {
        const int mPort = 8000;

        public int Port
        {
            get { return mPort; }
        }

        public Thread ServerThread
        {
            get { return mServerThread; }
            set { mServerThread = value; }
        }

        public string ServerIPAddress
        {
            get { return mIPAddress; }
            set { mIPAddress = value; }
        }

        string mIPAddress;

        private string mInstructionStringToSend;

        public string InstructionStringToSend
        {
            get { return mInstructionStringToSend; }
            set { mInstructionStringToSend = value; }
        }

        private Object mThreadLock = new Object();
        private int mCurrentNoUsers = 0;
        private TcpListener mListener;
        private Thread mOutputThread;
        public Thread OutputThread
        {
            get { return mOutputThread; }
            set { mOutputThread = value; }
        }
        private Thread mServerThread;
        Dictionary<TcpClient, Thread> mClients;

        userDetails mNewUser;
        IOProcessor mIOProcessor;
        MessageLogger mMessageLogger;
        QuestionManager mQuestionManager;
        AnswerManager mAnswerManager;

        UserManager mUserManager;

        private volatile bool mShouldStopThread;

        public QuestionManager QuestionManager
        {
            get { return mQuestionManager; }
            set { mQuestionManager = value; }
        }

        public void StopServer()
        {
            try
            {
                mShouldStopThread = true;

              
                if (mServerThread != null)
                    if (mServerThread.IsAlive)
                        mServerThread.Join(1000);

                if (mOutputThread != null)
                    if (mOutputThread.IsAlive)
                        mOutputThread.Join(1000);

                if (mListener != null)
                {
                    mListener.Stop();
                    mListener = null;
                }

                // Stop each client thread
                StopAllClients();
            }
            catch (System.Exception ex)
            {
                mMessageLogger.NewMessage(ex.Message, mMessageLogger.MESSAGE_ERROR);
            }
        }

        public void StopAllClients()
        {
            List<TcpClient> iClientsList = mClients.Keys.ToList();
            List<Thread> iClientThreadList = mClients.Values.ToList();

            // End each client thread
            for (int i = 0; i < mClients.Count; i++)
            {
                TcpClient iClient = iClientsList[i];
                Thread iClientThread = iClientThreadList[i];

                UserDisconnected(iClient);

                mMessageLogger.NewMessage(mUserManager.GetUsernameByClient(iClient) + " was kicked from the server.", mMessageLogger.MESSAGE_SERVER);
                //mUserManager.RemoveUser(iClient);
                //iClientThread.Join(1);
            }

            // Clear client list
            mClients.Clear();
            mMessageLogger.NewMessage("Client connections stopped!", mMessageLogger.MESSAGE_SERVER);
        }

        public Network(MessageLogger prMessageLogger, UserManager prUserManager, QuestionManager prQuestionManager, 
            AnswerManager prAnswerManager, IOProcessor prIOProcessor)
        {
            mIOProcessor = prIOProcessor;
            mMessageLogger = prMessageLogger;
            mQuestionManager = prQuestionManager;
            mUserManager = prUserManager;
            mAnswerManager = prAnswerManager;

            // Decide on an IP to use
            //IPHostEntry ipEntry = Dns.GetHostByName(Dns.GetHostName());
            IPAddress[] addr = Dns.GetHostAddresses(Dns.GetHostName());//ipEntry.AddressList;

            foreach (IPAddress iIP in addr)
            {
                if (iIP.ToString().Length <= 15)
                {
                    mIPAddress = iIP.ToString();
                }
            }

            //mListener = new TcpListener(IPAddress.Any, mPort);
            mClients = new Dictionary<TcpClient, Thread>();

            mServerThread = new Thread(new ThreadStart(Listen));

            mOutputThread = new Thread(new ThreadStart(SendOutput));
            mOutputThread.Start();
        }

        // Determines if the string is a valid IP 
        // Source: http://www.dreamincode.net/code/snippet1379.htm
        private bool IsValidIP(string prAddress)
        {
            //create an IPAddress variable, TryParse
            //requires an "out" value that is of
            //the type IPAddress
            IPAddress iIP;
            //boolean variable to hold the status
            bool iValid = false;
            //check to make sure an ip address was provided
            if (string.IsNullOrEmpty(prAddress))
            {
                //address wasnt provided so return false
                iValid = false;
            }
            else
            {
                //use TryParse to see if this is a
                //valid ip address. TryParse returns a
                //boolean based on the validity of the
                //provided address, so assign that value
                //to our boolean variable
                iValid = IPAddress.TryParse(prAddress, out iIP);
            }
            //return the value
            return iValid;
        }

        public void StartServer()
        {
            // TODO: Work out the proper way to start or restart the server
            if (mServerThread.ThreadState == ThreadState.Stopped)
            {
                mServerThread = new Thread(new ThreadStart(Listen));
            }

            mServerThread.Start();
        }

        public void Listen()
        {
            try
            {
                mListener = new TcpListener(IPAddress.Parse(mIPAddress), mPort);
                mListener.Start();

                while (!mShouldStopThread)
                {
                    System.Net.Sockets.TcpClient iClient = mListener.AcceptTcpClient();

                    // Create a new thread
                    Thread iClientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    if (!mClients.ContainsKey(iClient))
                        mClients.Add(iClient, iClientThread);      // Add thread to client list
                    iClientThread.Start(iClient);
                    Thread.Sleep(10);
                }
            }
            catch (System.Exception ex)
            {
            	
            }
        }

        private void SendOutput()
        {
            String iString;
            TcpClient iClient;
            NetworkStream iClientStream;
            DictionarySerialiserMethods iDictionarySerialiser = new DictionarySerialiserMethods();
            userDetails iUser;
            ChatMessage mMessageToSend = new ChatMessage();
            int iUserCount = 0;

            while (!mShouldStopThread)
            {
                if (!mMessageLogger.IsMessageQueueEmpty())
                {
                    mMessageToSend = mMessageLogger.GetNextMessageToSend();
                }

                // See if there's data to send
                // Possibly inefficient?, need to fix
                iUserCount = mUserManager.UsersOnline.Count;
                bool iDetailsLogged = false;

                // Wait for the mutex
                mUserManager.ThreadMutex.WaitOne();

                // This section of code is in need of a clean up, lots of bandaid fixes and nested if's
                if (iUserCount > 0)
                {
                    for (int i = 0; i <= mUserManager.MaxUserKey; i++)
                    {
                        // If only 1 client remaining
                        lock (mThreadLock)
                        {
                            if (mUserManager.UsersOnline.Count == 1)
                            {
                                i = mUserManager.UsersOnline.First().Key;
                            }
                        }

                        // To prevent a known bug the code is wrapped in a try catch 
                        try
                        {
                            // Send chat message
                            if (mMessageToSend != null)
                            {
                                if (mMessageToSend.Message != null)
                                {
                                    iUser = mUserManager.UsersOnline[i];
                                    iClient = (TcpClient)iUser.Client;

                                    // Make sure the client is still connected
                                    if (iClient.Connected == true)
                                        iClientStream = iClient.GetStream();
                                    else
                                        break;

                                    byte[] iChatMessage = iDictionarySerialiser.ConvertChatMessageToByteArray(mMessageToSend);

                                    if (iChatMessage != null)
                                    {
                                        iClientStream.Write(iChatMessage, 0, iChatMessage.Length);
                                    }
                                }
                            }

                            // Check the key is available
                            if (mUserManager.UsersOnline.ContainsKey(i))
                            {
                                iUser = mUserManager.UsersOnline[i];
                                iClient = (TcpClient)iUser.Client;

                                // Make sure the client is still connected
                                if (iClient.Connected == true)
                                    iClientStream = iClient.GetStream();
                                else
                                    break;


                                if (mUserManager.UsersOnline.Count > 0)
                                {
                                    // Send the user list 
                                    if (mUserManager.UsersOnline[i].UserListRequested == true)
                                    {
                                        byte[] iUserList = iDictionarySerialiser.ConvertUsersOnlineToByteArray(mUserManager.UserDetailsToTransfer);

                                        if (iUserList != null)
                                        {
                                            iClientStream.Write(iUserList, 0, iUserList.Length);
                                        }

                                        Array.Clear(iUserList, 0, iUserList.Length);

                                        // Trying to fix a slight bug where message is printed twice
                                        if (!iDetailsLogged)
                                        {
                                            mMessageLogger.NewMessage("Sent User Details to " + iUser.Username, mMessageLogger.MESSAGE_SERVER);
                                            iDetailsLogged = true;
                                        }

                                        mUserManager.UsersOnline[i].UserListRequested = false;

                                        // To fix the bug where the server can "flood" the client with xml data from several lists
                                        // This is a really bad way of fixing the issue as it adds a lot of latency to logging in
                                        // This was increased from 100 because it was causing issues on higher latency connects such as wireless
                                        Thread.Sleep(500);

                                    }
                                    // Send question list
                                    else if (mUserManager.UsersOnline[i].QuestionListRequested == true)
                                    {
                                        byte[] iQuestionList = iDictionarySerialiser.ConvertQuestionDictToByteArray(mQuestionManager.QuestionList);

                                        if (iQuestionList != null)
                                        {
                                            iClientStream.Write(iQuestionList, 0, iQuestionList.Length);
                                        }

                                        Array.Clear(iQuestionList, 0, iQuestionList.Length);

                                        mMessageLogger.NewMessage("Sent User Details to " + iUser.Username, mMessageLogger.MESSAGE_SERVER);
                                        mUserManager.UsersOnline[i].QuestionListRequested = false;

                                        // To fix the bug where the server can "flood" the client with xml data from several lists
                                        // Allows the client to process the current stream
                                        Thread.Sleep(100);
                                    }

                                    // Send a question
                                    else if (mUserManager.UsersOnline[i].CurrQuestion != null)
                                    {
                                        lock (mThreadLock)
                                        {
                                            if (mUserManager.UsersOnline[i].CurrQuestion.Question != null)
                                            {
                                                byte[] iQuestionArray = iDictionarySerialiser.ConvertQuestionToByteArray(mUserManager.UsersOnline[i].CurrQuestion);

                                                if (iQuestionArray != null)
                                                {
                                                    iClientStream.Write(iQuestionArray, 0, iQuestionArray.Length);
                                                }
                                                Array.Clear(iQuestionArray, 0, iQuestionArray.Length);

                                                mUserManager.UsersOnline[i].CurrQuestion.Question = null;
                                                mUserManager.UsersOnline[i].CurrQuestion.QuestionID = 0;
                                                mUserManager.UsersOnline[i].CurrQuestion.PossibleAnswers = null;
                                                mUserManager.UsersOnline[i].CurrQuestion.Answer = null;
                                                mUserManager.UsersOnline[i].CurrQuestion.QuestionType = null;

                                            }
                                        }

                                        // To fix the bug where the server can "flood" the client with xml data from several lists
                                        // Allows the client to process the current stream
                                        Thread.Sleep(100);
                                    }

                                    // Send a question using the older string format, for iOS devices
                                    if (mUserManager.UsersOnline[i].CurrQuestionString != null)
                                    {
                                        string iQuestionString = mUserManager.UsersOnline[i].CurrQuestionString;

                                        if (iQuestionString != "")
                                        {
                                            UTF8Encoding iEncoding = new UTF8Encoding();
                                            byte[] iQuestionStringArray = iEncoding.GetBytes(iQuestionString);

                                            if (iQuestionStringArray != null)
                                            {
                                                iClientStream.Write(iQuestionStringArray, 0, iQuestionStringArray.Length);
                                            }

                                            Array.Clear(iQuestionStringArray, 0, iQuestionStringArray.Length);
                                            mUserManager.UsersOnline[i].CurrQuestionString = "";
                                            // To fix the bug where the server can "flood" the client with xml data from several lists
                                            // Allows the client to process the current stream
                                            Thread.Sleep(100);
                                        }
                                    }

                                    // Send an Answer to Tutor
                                    // Need to make this more efficient when dealing with large numbers of answers
                                    if (mAnswerManager.AnswersList.Count > 0)
                                    {
                                        if (mUserManager.UsersOnline[i].UserRole == "Tutor")
                                        {
                                            int iPos = 0;
                                            foreach (KeyValuePair<int, Answer> iAnswer in mAnswerManager.AnswersList)
                                            {
                                                if (iAnswer.Value.AnswerSent == false)
                                                {
                                                    byte[] iQuestionArray = iDictionarySerialiser.ConvertAnswerToByteArray(iAnswer.Value);

                                                    if (iQuestionArray != null)
                                                        iClientStream.Write(iQuestionArray, 0, iQuestionArray.Length);

                                                    Array.Clear(iQuestionArray, 0, iQuestionArray.Length);

                                                    mAnswerManager.AnswersList[iPos].AnswerSent = true;
                                                }
                                                iPos++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                mMessageToSend = null;      // Clear the current chat message

                // Let other threads access the data
                mUserManager.ThreadMutex.ReleaseMutex();
                Thread.Sleep(10);
                }
            }
        

        private void HandleClientComm(object prClient)
        {
            TcpClient iTCPClient = (TcpClient)prClient;
            NetworkStream iClientStream = iTCPClient.GetStream();

            //mNewUser = mUserManager.GetUserByClient(iTCPClient);

            //if (mNewUser == null)
            mNewUser = new userDetails();

            byte[] iMessage = new byte[4096];
            int iBytesRead = 0;
            string iString;

            while (!mShouldStopThread)
            {
                iBytesRead = 0;

                try
                {
                    if (iClientStream != null)
                        iBytesRead = iClientStream.Read(iMessage, 0, 4096);
                    else
                        break;
                }
                catch (System.Exception ex)
                {
                    if (mClients.ContainsKey(iTCPClient))
                        UserDisconnected(iTCPClient);
                    break;
                }

                if (iBytesRead == 0)
                {
                    // Client has disconnected
                    // Make sure they haven't been kicked already
                    if (mClients.ContainsKey(iTCPClient))
                        UserDisconnected(iTCPClient);
                    iClientStream = null;
                    break;
                }

                // Get the users details
                mNewUser = mUserManager.GetUserByClient(iTCPClient);

                if (mNewUser.Client == null)
                {
                    mNewUser.Client = (TcpClient)prClient;
                }


                iString = new string(Encoding.ASCII.GetChars(iMessage));

                mIOProcessor.ParseNewString(iString, mNewUser);
                iString = "";
                Array.Clear(iMessage, 0, iMessage.Length);

                Thread.Sleep(10);
            }
        }

        private void UserDisconnected(TcpClient prClient)
        {

            mMessageLogger.NewMessage(mUserManager.GetUsernameByClient(prClient) + " left the server.", mMessageLogger.MESSAGE_SERVER);
            mUserManager.RemoveUser(prClient);

            if (prClient.Connected)
            {
                NetworkStream iStream = prClient.GetStream();
                iStream.Dispose();
            }
            
            mClients[prClient].Join(1);
            mClients.Remove(prClient);
            // Update all tutor clients
            mUserManager.SendUserListToTutors();
        }
    }
}
