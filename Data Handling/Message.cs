using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace srs_server
{
    struct chatMessage
    {
        public string sMessage;
        public string sUsername;
    }

    class Message
    {
        public const int mHeaderLength = 2;
        public const int mMaxBodyLength = 512;

        public char[] data()
        {
            return mData;
        }

        //public char[] body()
        //{
        //    return mData + mHeaderLength;
        //}

        public int getHeaderLength()
        {
            return mHeaderLength;
        }

        public bool decodeHeader()
        {
            char[] iHeader = new char[mHeaderLength];
            char[] iHeaderArray = new char[2];
            string iHeaderString = "";

            System.Array.Copy(mData, 0, iHeader, 0, mHeaderLength);

            iHeaderString = Convert.ToString(iHeader);

            mBodyLength = iHeader.Count();
            if (mBodyLength > mMaxBodyLength)
            {
                mBodyLength = 0;
                return false;
            }
            return true;
        }

        public void bodyLength(int prLength)
        {
            mBodyLength = prLength;
            if (mBodyLength > mMaxBodyLength)
                mBodyLength = mMaxBodyLength;
        }

        public void encodeHeader()
        {
            string iHeader = "Q;";
            //iHeader = string.Format("{0:d4}", mBodyLength);
            char[] iHeaderArray = iHeader.ToCharArray();

            System.Array.Copy(iHeaderArray, 0,mData, 0, mHeaderLength);
        }

        public void setData(char[] prData)
        {
            mData = prData;
        }

        public int getMaxHeaderLength()
        {
            return mHeaderLength;
        }

        public int getMaxBodyLength()
        {
            return mMaxBodyLength;
        }

        private char[] mData = new char[mHeaderLength + mMaxBodyLength];
        private int mBodyLength;
    }
}

