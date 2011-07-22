using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace srs_server
{
    // A chat message is a short message that is sent to either one or all other clients
    // A future modification would be to support sending to specific clients
    [Serializable()]
    public class ChatMessage
    {
        private string mSendTo;     // Can be either a client name or "ALL"
        private string mFrom;       // Where the message is from
        private string mMessage;    // Message to be sent

        public string SendTo
        {
            get { return mSendTo; }
            set { mSendTo = value; }
        }
        public string From
        {
            get { return mFrom; }
            set { mFrom = value; }
        }
        public string Message
        {
            get { return mMessage; }
            set { mMessage = value; }
        }
    }

    public class ChatManager
    {
        private Queue<ChatMessage> mChatMessages; // Queue to hold messages to be sent

        public Queue<ChatMessage> ChatMessages
        {
            get { return mChatMessages; }
            set { mChatMessages = value; }
        }

        public ChatManager()
        {
            mChatMessages = new Queue<ChatMessage>();
        }
    }
    // Keeping just encase I need it, will be attempting to use an XML serialised format from now on
    //public class ChatMessage
    //{
    //    public const int mHeaderLength = 2;
    //    public const int mMaxBodyLength = 512;

    //    public char[] data()
    //    {
    //        return mData;
    //    }

    //    public int getHeaderLength()
    //    {
    //        return mHeaderLength;
    //    }

    //    public bool decodeHeader()
    //    {
    //        char[] iHeader = new char[mHeaderLength + 1];
    //        string iHeaderStr = new string(iHeader);

    //        return true;
    //    }

    //    public void bodyLength(int prLength)
    //    {
    //        mBodyLength = prLength;
    //        if (mBodyLength > mMaxBodyLength)
    //            mBodyLength = mMaxBodyLength;
    //    }

    //    public void encodeHeader()
    //    {
    //        string iHeader = "Q;";
    //        //iHeader = string.Format("{0:d4}", mBodyLength);
    //        char[] iHeaderArray = iHeader.ToCharArray();

    //        System.Array.Copy(iHeaderArray, 0,mData, 0, mHeaderLength);
    //    }

    //    public void setData(char[] prData)
    //    {
    //        mData = prData;
    //    }

    //    public int getMaxHeaderLength()
    //    {
    //        return mHeaderLength;
    //    }

    //    public int getMaxBodyLength()
    //    {
    //        return mMaxBodyLength;
    //    }

    //    private char[] mData = new char[mHeaderLength + mMaxBodyLength];
    //    private int mBodyLength;
    //}
}

