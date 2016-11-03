using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml;

namespace TouchHandle.myTool
{
    public class myKeyMap
    {
        public XmlDocument myXml = new XmlDocument();
        public string myFile = "";
        public List<Dictionary<string, Key>> myKeyMaps = new List<Dictionary<string, Key>>();
        public Dictionary<int, string> myListMapNames = new Dictionary<int, string>();
        //public List<string> myListMapNames = new List<string>();


        /// <summary>
        /// load the casefile to XmlDocument
        /// </summary>
        /// <param name="tempFileName">file path</param>
        /// <returns>is succees</returns>
        public bool LoadFile(string tempFileName)
        {
            try
            {
                myXml.Load(tempFileName);
            }
            catch (Exception ex)
            {
                myTool.ErrorLog.PutInLogEx(ex.Message);
                return false;
            }
            myFile = tempFileName;
            return true;
        }

        /// <summary>
        /// creat Maps in my database
        /// </summary>
        /// <returns>is ok</returns>
        public bool loadKeyMaps()
        {
            try
            {
                XmlNodeList xmlMaps = myXml.ChildNodes[1].ChildNodes[0].ChildNodes;
                foreach (XmlNode tempNode in xmlMaps)
                {
                    Dictionary<string, Key> tempKeyMapDictionary = new Dictionary<string, Key>();
                    fillMyDictionary(tempKeyMapDictionary);
                    foreach (XmlNode tempKeyValueNode in tempNode.ChildNodes)
                    {
                        tempKeyMapDictionary.myAdd(tempKeyValueNode.Name, KeyInterop.KeyFromVirtualKey(int.Parse(tempKeyValueNode.InnerText)));
                    }
                    myKeyMaps.Add(tempKeyMapDictionary);
                    string tempMapName = tempNode.Attributes["name"].Value;
                    myListMapNames.Add(myListMapNames.Count, tempMapName);
                    //myListMapNames.Add(tempMapName);
                }
                return true;
            }
            catch(Exception ex)
            {
                myRuntimeConfiguration.myNowError = ex.Message;
                myTool.ErrorLog.PutInLogEx(ex.Message);
                return false;
            }
            
        }

        /// <summary>
        /// sava now key map in Dictionary (myKeyMaps)
        /// </summary>
        /// <param name="yourIndex">the myKeyMaps index</param>
        /// <param name="yourName">myKeyMaps name</param>
        public void savaNowKeyMap(int yourIndex,string yourName)
        {
            myListMapNames[yourIndex] = yourName;

            myKeyMaps[yourIndex]["key_up"] = myRuntimeConfiguration.myUserKey_Up;
            myKeyMaps[yourIndex]["key_down"] = myRuntimeConfiguration.myUserKey_Down;    
            myKeyMaps[yourIndex]["key_left"] = myRuntimeConfiguration.myUserKey_Left;       
            myKeyMaps[yourIndex]["key_right"] = myRuntimeConfiguration.myUserKey_Right;     
            myKeyMaps[yourIndex]["key_ok"] = myRuntimeConfiguration.myUserKey_Ok;       
            myKeyMaps[yourIndex]["key_A"] = myRuntimeConfiguration.myUserKey_A;   
            myKeyMaps[yourIndex]["key_B"] = myRuntimeConfiguration.myUserKey_B;          
            myKeyMaps[yourIndex]["key_C"] = myRuntimeConfiguration.myUserKey_C;    
            myKeyMaps[yourIndex]["key_D"] = myRuntimeConfiguration.myUserKey_D;


            /*
            foreach (string tempKeyName in  (myKeyMaps[yourIndex].Keys))
            {
                switch (tempKeyName)
                {
                    case "key_up":
                        myKeyMaps[yourIndex][tempKeyName] = myRuntimeConfiguration.myUserKey_Up;
                        break;
                    case "key_down":
                        myKeyMaps[yourIndex][tempKeyName] = myRuntimeConfiguration.myUserKey_Down;
                        break;
                    case "key_left":
                        myKeyMaps[yourIndex][tempKeyName] = myRuntimeConfiguration.myUserKey_Left;
                        break;
                    case "key_right":
                        myKeyMaps[yourIndex][tempKeyName] = myRuntimeConfiguration.myUserKey_Right;
                        break;
                    case "key_ok":
                        myKeyMaps[yourIndex][tempKeyName] = myRuntimeConfiguration.myUserKey_Ok;
                        break;
                    case "key_A":
                        myKeyMaps[yourIndex][tempKeyName] = myRuntimeConfiguration.myUserKey_A;
                        break;
                    case "key_B":
                        myKeyMaps[yourIndex][tempKeyName] = myRuntimeConfiguration.myUserKey_B;
                        break;
                    case "key_C":
                        myKeyMaps[yourIndex][tempKeyName] = myRuntimeConfiguration.myUserKey_C;
                        break;
                    case "key_D":
                        myKeyMaps[yourIndex][tempKeyName] = myRuntimeConfiguration.myUserKey_D;
                        break;
                    default:
                        myTool.ErrorLog.PutInLogEx(tempKeyName + "未发现匹配项");
                        break;
                }
            }
            */
        } 

        /// <summary>
        /// save all key in xml file 
        /// </summary>
        /// <returns>is ok</returns>
        public bool saveAllKeyXml()
        {
            if (myXml==null)
            {
                myRuntimeConfiguration.myNowError = "no file load";
                myTool.ErrorLog.PutInLog("保存了没有加载的文件：no file load");
                return false;
            }
            else
            {
                try
                {
                    XmlNodeList xmlMaps = myXml.ChildNodes[1].ChildNodes[0].ChildNodes;
                    foreach(KeyValuePair<int,string> tempNamesKvp in myListMapNames)
                    {
                        xmlMaps[tempNamesKvp.Key].Attributes["name"].Value = tempNamesKvp.Value;
                        foreach(KeyValuePair<string,Key> tempKeyKvp in myKeyMaps[tempNamesKvp.Key])
                        {
                            if(xmlMaps[tempNamesKvp.Key].SelectSingleNode(tempKeyKvp.Key)!=null)
                            {
                                xmlMaps[tempNamesKvp.Key].SelectSingleNode(tempKeyKvp.Key).InnerText = (KeyInterop.VirtualKeyFromKey(tempKeyKvp.Value)).ToString();
                            }
                            else
                            {
                                XmlElement myNewKeyNode= myXml.CreateElement(tempKeyKvp.Key);
                                myNewKeyNode.InnerText = (KeyInterop.VirtualKeyFromKey(tempKeyKvp.Value)).ToString();
                                xmlMaps[tempNamesKvp.Key].AppendChild(myNewKeyNode);
                            }
                        }
                    } 
                }
                catch(Exception ex)
                {
                    myRuntimeConfiguration.myNowError = ex.Message;
                    myTool.ErrorLog.PutInLogEx(ex.Message);
                    return false;
                }
            }
            myXml.Save(myFile);
            return true;
        }

        /// <summary>
        /// fill the default keyMap
        /// </summary>
        /// <param name="yourDictionary">the Dictionary will fill</param>
        private void fillMyDictionary(Dictionary<string, Key> yourDictionary)
        {
            yourDictionary.myAdd("key_up",Key.Up);
            yourDictionary.myAdd("key_down",Key.Down);
            yourDictionary.myAdd("key_left",Key.Left);
            yourDictionary.myAdd("key_right",Key.Right);
            yourDictionary.myAdd("key_ok",Key.Enter);
            yourDictionary.myAdd("key_A",Key.A);
            yourDictionary.myAdd("key_B",Key.B);
            yourDictionary.myAdd("key_C",Key.C);
            yourDictionary.myAdd("key_D",Key.D);
        }
    }
}
