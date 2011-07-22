using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace srs_server
{
    public class XmlHandler
    {
        Encryption mEncryption;

        public XmlHandler()
        {
            mEncryption = new Encryption();
        }

        // Saves the tutors list to file
        public void SaveTutorList(Dictionary<int, TutorDetails> prTutors)
        {
            // Create a new file
            XmlTextWriter iTextWriter = new XmlTextWriter("tutors.xml", null);
            // Opens the document
            iTextWriter.WriteStartDocument();

            // Comments
            iTextWriter.WriteComment("Enrolled Tutors for SRS Tutor Client");
            iTextWriter.WriteComment("THIS IS A TEMPORARY STORAGE LOCATION");

            // Write first element
            iTextWriter.WriteStartElement("Tutors");
            iTextWriter.WriteStartElement("r", "RECORD", "urn:record");

            foreach (KeyValuePair<int, TutorDetails> iTutor in prTutors)
            {
                iTextWriter.WriteStartElement("Tutor", "");

                // Username
                iTextWriter.WriteStartElement("UserName", "");
                iTextWriter.WriteString(iTutor.Value.Name);
                iTextWriter.WriteEndElement();

                // Password
                iTextWriter.WriteStartElement("Password", "");
                // Encrypt the password
                iTextWriter.WriteString(mEncryption.Encrypt(iTutor.Value.Password, "P@ssword1"));
                iTextWriter.WriteEndElement();

                // Server Address
                iTextWriter.WriteStartElement("Class", "");
                iTextWriter.WriteString(iTutor.Value.Class);
                iTextWriter.WriteEndElement();

                iTextWriter.WriteEndElement();
            }

            // Ends the document.
            iTextWriter.WriteEndDocument();

            // Close the writer
            iTextWriter.Close();
        }

        // Attempt to load the users settings
        // This code is very messy, need to clean up the variable names
        public Dictionary<int, TutorDetails> LoadUserSettings(string prFilename)
        {
            if (File.Exists(prFilename))
            {
                XmlDocument iXmlDocument = new XmlDocument();
                Dictionary<int, TutorDetails> iTutors = new Dictionary<int, TutorDetails>();
                TutorDetails iTutor;
                Encryption iEncryption = new Encryption();

                iXmlDocument.Load(prFilename);

                //Make sure document has been loaded
                if (iXmlDocument != null)
                {
                    XmlNodeList iTutorsXML = iXmlDocument.ChildNodes;
                    XmlNodeList iTutorDetails;
                    foreach (XmlNode iNode in iTutorsXML)
                    {
                        if (iNode.InnerText != "version=\"1.0\"")
                        {
                            iTutorDetails = iNode.ChildNodes;
                            foreach (XmlNode iTutorInfo in iTutorDetails)
                            {
                                foreach (XmlNode iTutorDetailsXML in iTutorInfo)
                                {
                                    iTutor = new TutorDetails();
                                    iTutor.Name = iTutorDetailsXML.ChildNodes[0].InnerText;
                                    // Decrypt the password
                                    iTutor.Password = iEncryption.Decrypt(iTutorDetailsXML.ChildNodes[1].InnerText, "P@ssword1");
                                    iTutor.Class = iTutorDetailsXML.ChildNodes[2].InnerText;

                                    if (iTutors.Count == 0)
                                        iTutors.Add(0, iTutor);
                                    else
                                        iTutors.Add(iTutors.Count + 1, iTutor);
                                }
                            }
                        }
                    }
                }
                return iTutors;
            }
            return null;
        }
    }
}
