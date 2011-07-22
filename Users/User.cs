using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace srs_server
{
    class User
    {
        public const int mHeaderLength = 2;
        public const int mMaxBodyLength = 50;

        public int BodyLength
        {
            get { return mBodyLength; }
            set
            {
                if (mBodyLength > mMaxBodyLength)
                    mBodyLength = mMaxBodyLength;
            }  
        }

        public int GetMaxDataLength()
        {
            return mHeaderLength + mMaxBodyLength;
        }

        public char[] Data
        {
            get { return mData; }
            set { mData = value; }
        }

        public string Username
        {
            get { return mUsername; }
            set { mUsername = value; }
        }
        public string DeviceOS
        {
            get { return mDeviceOS; }
            set { mDeviceOS = value; }
        }
        public int UserID
        {
            get { return mUserID; }
            set { mUserID = value; }
        }
        public string UserIP
        {
            get { return mUserIP; }
            set { mUserIP = value; }
        }
        public bool HasUserData
        {
            get { return mHasUserData; }
            set { mHasUserData = value; }
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

        
        //public void encodeHeader()
        //{
        //    string iHeader = "Q;";
        //    //iHeader = string.Format("{0:d4}", mBodyLength);
        //    char[] iHeaderArray = iHeader.ToCharArray();

        //    System.Array.Copy(iHeaderArray, 0, mData, 0, mHeaderLength);
        //}

        private string mUsername;
        private string mDeviceOS;
        private int mUserID;
        private string mUserIP;

        private bool mHasUserData;
        private char[] mData = new char[mHeaderLength + mMaxBodyLength];
        private int mBodyLength;
    }
}
