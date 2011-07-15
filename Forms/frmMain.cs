using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using System.Net.Sockets;
using System.IO;

namespace SRS_Server.Net
{
    public partial class frmMain : Form
    {
        User mUser;
        IOProcessor mIOProcessor;
        MessageLogger mMessageLogger;
        UserManager mUserManager;
        QuestionManager mQuestionManager;
        AnswerManager mAnswerManager;
        ChatManager mChatManager;

        Network mNetwork;

        DictionarySerialiserMethods mDictionarySerialiser;

        bool mRunning;

        System.Timers.Timer mTimer;

        delegate void SetTextCallback(string text);
        delegate void SetUsersCallback(string prText);
        delegate void AddQuestionCallback(string prText);
        delegate int GetSelectedIndexCallback();
        delegate void ToggleKickUserBtnCallback(bool prToggle);

        public frmMain()
        {
            InitializeComponent();
            
            mMessageLogger = new MessageLogger();
            mChatManager = new ChatManager();
            mUserManager = new UserManager(mMessageLogger);
            mQuestionManager = new QuestionManager(mUserManager);
            mAnswerManager = new AnswerManager();
            mIOProcessor = new IOProcessor(mMessageLogger, mUserManager, 
                mQuestionManager, mAnswerManager, mChatManager);

            //mIOProcessor = new IOProcessor(mMessageLogger, mUserManager);
            mNetwork = new Network(mMessageLogger, mUserManager, mQuestionManager, mAnswerManager, mIOProcessor);
            
            // Load the registered tutors
            if (!mUserManager.LoadTutors("tutors.xml"))
            {
                mMessageLogger.NewMessage("Can't load file \"tutors.xml\" or the contents in it. " +
                    "Please use the admin tool to recreate it", mMessageLogger.MESSAGE_ERROR);
            }

            mUser = new User();

            mRunning = true;

            mDictionarySerialiser = new DictionarySerialiserMethods();

            // Attempt to load questions from the questions.xml file
            if (LoadQuestions("questions.xml") == false)
            {
                // Add some demo questions if none could be loaded from a file
                question iDemoQuestion = new question();

                // Multi-choice
                iDemoQuestion.QuestionID = 0;
                iDemoQuestion.Question = "How many days in a week?";
                iDemoQuestion.QuestionType = "MC";
                iDemoQuestion.PossibleAnswers = new string[4] { "Ten", "Three", "Seven", "Nine" };
                iDemoQuestion.Answer = "Seven";
                mQuestionManager.AddNewQuestion(iDemoQuestion);

                // True/False
                iDemoQuestion = new question();
                iDemoQuestion.QuestionID = 1;
                iDemoQuestion.Question = "Fire burns?";
                iDemoQuestion.QuestionType = "TF";
                iDemoQuestion.PossibleAnswers = new string[4] { "True", "False", "", "" };
                iDemoQuestion.Answer = "True";
                mQuestionManager.AddNewQuestion(iDemoQuestion);

                // Short Answer
                iDemoQuestion = new question();
                iDemoQuestion.QuestionID = 2;
                iDemoQuestion.Question = "What is 5x5?";
                iDemoQuestion.QuestionType = "SA";
                iDemoQuestion.PossibleAnswers = new string[4] { "", "", "", "" };
                iDemoQuestion.Answer = "";
                mQuestionManager.AddNewQuestion(iDemoQuestion);
            }
            mTimer = new System.Timers.Timer(1000);
            mTimer.Elapsed += new ElapsedEventHandler(TimeElapsed);
            mTimer.Enabled = true;
        }

        // Load questions list from disk
        private bool LoadQuestions(string prFilename)
        {
            if (File.Exists(prFilename))
            {
                mQuestionManager.QuestionList = mDictionarySerialiser.LoadQuestionsFromDisk(prFilename);

                if (mQuestionManager.QuestionList != null)
                    return true;
            }
            return false;
        }

        // Executed each time the timer ticks
        private void TimeElapsed(object sender, ElapsedEventArgs e)
        {
            if (!mMessageLogger.IsLogEmpty())
            {
                if (mMessageLogger.IsNewMsgAvailable())
                    this.SetLogText(mMessageLogger.DisplayLastMessages());
            }

            if (mRunning)
            {
                // Update users online
                if (!this.IsDisposed)
                {
                    this.SetUserText("Users Online: " + mUserManager.GetNumOfUsers().ToString());
                }
                else
                {
                    mRunning = false;
                }

                // Update kick button toggle
                if (mUserManager.GetNumOfUsers() > 0)
                {
                    if (btnKickUsers.Enabled == false)
                        this.ToggleKickUsersButton(true);
                }
                else
                {
                    if (btnKickUsers.Enabled == true)
                        this.ToggleKickUsersButton(false);
                }
            }
        }

        private void ToggleKickUsersButton(bool prToggle)
        {
            if (this.btnKickUsers.InvokeRequired)
            {
                ToggleKickUserBtnCallback k = new ToggleKickUserBtnCallback(ToggleKickUsersButton);
                try
                {
                	this.Invoke(k, new object[] { prToggle });
                }
                catch (System.Exception ex)
                {
                }
            }
            else
            {
                this.btnKickUsers.Enabled = prToggle;
            }
        }

       

        private void SetUserText(string prText)
        {
            if (this.lblUsersOnline.InvokeRequired)
            {
                SetUsersCallback l = new SetUsersCallback(SetUserText);
                try
                {
                    if (!this.IsDisposed)
                        this.Invoke(l, new object[] { prText });
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                this.lblUsersOnline.Text = prText;
            }
        }

        private void SetLogText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.rtbMessageLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetLogText);
                try
                {
                    this.Invoke(d, new object[] { text });
                }
                catch (Exception ex)
                {
                   
                }
            }
            else
            {
                this.rtbMessageLog.Text += text;
                this.rtbMessageLog.SelectionStart = rtbMessageLog.Text.Length;
                this.rtbMessageLog.ScrollToCaret();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!mNetwork.ServerThread.IsAlive)
            {
                mNetwork.StartServer();
                btnStart.Enabled = false;
                btnStop.Enabled = true;

                lblServerStatus.Text = "Online";
                mMessageLogger.NewMessage("Online", mMessageLogger.MESSAGE_SERVER);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (mNetwork.ServerThread.IsAlive)
            {
                mNetwork.StopServer();
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                lblServerStatus.Text = "Offline";
                mMessageLogger.NewMessage("Offline", mMessageLogger.MESSAGE_SERVER);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save Data
            mDictionarySerialiser.SaveQuestionsToDisk("questions.xml", mQuestionManager.QuestionList);

            mRunning = false;
            mTimer.Stop();
            mNetwork.StopServer();
            mNetwork = null;
            mMessageLogger = null;
            mTimer = null;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            txtServerIP.Text = mNetwork.ServerIPAddress;
            txtServerPort.Text = mNetwork.Port.ToString();

            
            if (!mNetwork.ServerThread.IsAlive)
            {
                mNetwork.StartServer();
                btnStart.Enabled = false;
                btnStop.Enabled = true;

                lblServerStatus.Text = "Online";
                mMessageLogger.NewMessage("Online", mMessageLogger.MESSAGE_SERVER);
            }
        }

        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            userDetails iTempAdmin = new userDetails();
            mIOProcessor.ParseNewString("I;" + txtCommandLine.Text, iTempAdmin);
        }

        private void btnKickUsers_Click(object sender, EventArgs e)
        {
            mNetwork.StopAllClients();
        }

        private void btnSendUsers_Click(object sender, EventArgs e)
        {
            mUserManager.SendUserListToTutors();

            mQuestionManager.SendQuestionListToTutors();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void notifier_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void rtbMessageLog_TextChanged(object sender, EventArgs e)
        {
            rtbMessageLog.SelectionStart = rtbMessageLog.Text.Length;
            rtbMessageLog.ScrollToCaret();
        }

        private void btnTutors_Click(object sender, EventArgs e)
        {
            frmTutors iTutorForm = new frmTutors(mUserManager);

            iTutorForm.Show();
        }
    }
}
