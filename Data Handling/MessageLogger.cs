using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRS_Server.Net
{
    public class MessageLogger
    {
        // Defines
        const int mMESSAGE_SERVER = 1;
        const int mMESSAGE_TUTOR = 2;
        const int mMESSAGE_STUDENT = 3;

        const int mMESSAGE_CHAT = 4;
        const int mMESSAGE_NETWORK = 5;
        const int mMESSAGE_ERROR = 6;
        const int mMESSAGE_WARNING = 7;
        const int mMESSAGE_QUESTION = 8;
        const int mMESSAGE_ANSWER = 9;
        const int mMESSAGE_COMMAND = 10;

        public int MESSAGE_SERVER
        {
            get { return mMESSAGE_SERVER; }
        }

        public int MESSAGE_TUTOR
        {
            get { return mMESSAGE_TUTOR; }
        }

        public int MESSAGE_STUDENT
        {
            get { return mMESSAGE_STUDENT; }
        }

        public int MESSAGE_CHAT
        {
            get { return mMESSAGE_CHAT; }
        }

        public int MESSAGE_NETWORK
        {
            get { return mMESSAGE_NETWORK; }
        }

        public int MESSAGE_ERROR
        {
            get { return mMESSAGE_ERROR; }
        }

        public int MESSAGE_WARNING
        {
            get { return mMESSAGE_WARNING; }
        }

        public int MESSAGE_QUESTION
        {
            get { return mMESSAGE_QUESTION; }
        }

        public int MESSAGE_ANSWER
        {
            get { return mMESSAGE_ANSWER; }
        } 

        public int MESSAGE_COMMAND
        {
            get { return mMESSAGE_COMMAND; }
        }

        public Queue ChatMessageQueue
        {
            get { return mChatMessageQueue; }
            set { mChatMessageQueue = value; }
        }

        public int LastMessagePosted
        {
            get { return mLastMessagePosted; }
            set { mLastMessagePosted = value; }
        }
        public int CurrentMessageNo
        {
            get { return mCurrentMessageNo; }
            set { mCurrentMessageNo = value; }
        }

        Dictionary<int, string> mMessageLog;            // Store all local server messages
        private Queue mChatMessageQueue;                // Queue to hold messages to be sent

        private int mCurrentMessageNo;
        private int mLastMessagePosted;

        public MessageLogger()
        {
            mMessageLog = new Dictionary<int, string>();
            //mChatMessageQueue = new List<Message>();
            mChatMessageQueue = new Queue();
        }

        public bool IsMessageQueueEmpty()
        {
            if (mChatMessageQueue.Count == 0)
                return true;
            else
                return false;
        }

        // Get any messages in queue and return as a string to be sent to all connected clients
        public string GetMessagesToSend()
        {
            string iMessages = "";
            chatMessage iMessage;

            if (mChatMessageQueue.Count != 0)
            {
                for (int i = 0; i <= mChatMessageQueue.Count; i++)
                {
                    iMessage = (chatMessage)mChatMessageQueue.Dequeue();
                    iMessages += iMessage.sUsername + ": " + iMessage.sMessage + "\n";
                }
            }
            return iMessages;
        }

        // Gets the first message in the queue
        public ChatMessage GetNextMessageToSend()
        {
            ChatMessage iLastMessage = new ChatMessage();

            iLastMessage = (ChatMessage)mChatMessageQueue.Dequeue();

            return iLastMessage;
        }

        public string DisplayLastMessages()
        {
            int iMessagesToReturn = 0;
            int iSize = mMessageLog.Count;
            string iMessages = "";

            iMessagesToReturn = iSize - mLastMessagePosted;

            if (iSize == 1)
            {
                iMessages = mMessageLog[0];
            }
            else
            {
                for (int i = mLastMessagePosted; i < (mLastMessagePosted + iMessagesToReturn); i++)
                {
                    iMessages += mMessageLog[i];
                    //mLastMessagePosted++;
                }
            }

           
            LastMessagePosted = iSize;
            return iMessages;
        }

        public void NewMessage(string prMessage, int prFlag)
        {
            string iMsgToAdd = "";
            int iIndex = 0;

            iIndex = CurrentMessageNo;

            switch (prFlag)
            {
                case 1:
                    iMsgToAdd = "Server: " + prMessage + "\n";
            	    break;
                case 2:
                    iMsgToAdd = "Admin: " + prMessage + "\n";
                    break;
                case 3:
                    iMsgToAdd = "Student: " + prMessage + "\n";
                    break;
                case 4:
                    iMsgToAdd = "Message: " + prMessage + "\n";
                    break;
                case 5:
                    iMsgToAdd = "Network: " + prMessage + "\n";
                    break;
                case 6:
                    iMsgToAdd = "Error: " + prMessage + "!" + "\n";
                    break;
                case 7:
                    iMsgToAdd = "Warning: " + prMessage + "\n";
                    break;
                case 8:
                    iMsgToAdd = "Question: " + prMessage + "\n";
                    break;
                case 9:
                    iMsgToAdd = "Answer: " + prMessage + "\n";
                    break;
                case 10:
                    iMsgToAdd = "> " + prMessage + "\n";
                    break;
            }

            // To stop a bug as well as limit log spam a message must not be a duplicate of the message before it
            // Fix at some point to allow for more accurate results
            string iPreviousMessage;
            mMessageLog.TryGetValue(iIndex - 1, out iPreviousMessage);
            if (iPreviousMessage != iMsgToAdd)
            {
                mMessageLog.Add(iIndex, iMsgToAdd);
                CurrentMessageNo += 1;
            }
        }

        public void ClearAllMessages()
        {
            mMessageLog.Clear();
        }

        public bool IsNewMsgAvailable()
        {
            if (mMessageLog.Count() > mLastMessagePosted)
                return true;
            else
                return false;
        }

        public bool IsLogEmpty()
        {
            if (mMessageLog.Count() == 0)
                return true;
            else
                return false;
        }
    }
}
