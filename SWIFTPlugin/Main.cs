using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SWIFT.HumanifyMessage.PluginInfrastructure;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using SWIFT.HumanifyMessage;

namespace SWIFT.HumanifyMessage
{
    public static class Main
    {
        internal const string PluginName = "SWIFT";
        static string iniFilePath = null;
        static bool someSetting = false;
        static frmMyDlg frmMyDlg = null;
        static int idMyDlg = -1;
        static Bitmap tbBmp = Properties.Resources.star;
        static Bitmap tbBmp_tbTab = Properties.Resources.star_bmp;
        static Icon tbIcon = null;
        static IScintillaGateway editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());
        static INotepadPPGateway notepad = new NotepadPPGateway();
        static SWIFTInformation swiftInfo = new SWIFTInformation();
        static Utils utils = new Utils();

        static string version = "0.2";
        static string date = "09.24.2018";

        public static void OnNotification(ScNotification notification)
        {
            // This method is invoked whenever something is happening in notepad++
            // use eg. as
            // if (notification.Header.Code == (uint)NppMsg.NPPN_xxx)
            // { ... }
            // or
            //
            // if (notification.Header.Code == (uint)SciMsg.SCNxxx)
            // { ... }
        }

        internal static void CommandMenuInit()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");
            someSetting = (Win32.GetPrivateProfileInt("SomeSection", "SomeKey", 0, iniFilePath) != 0);

            PluginBase.SetCommand(1, "Humanify MT950", MT950, new ShortcutKey(false, true, true, Keys.N));
            PluginBase.SetCommand(1, "---", null); idMyDlg = 1;
            PluginBase.SetCommand(2, "Version", Version);
        }

        internal static void SetToolBarIcon()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = tbBmp.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idMyDlg]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        internal static void Version()
        {
            string message = "Version: " + version + "\n" + "Last Updated: " + date;
            MessageBox.Show(message);
        }

        internal static void MT950()
        {
            try
            {
                // select all content in active page
                editor.SelectAll();
                int length = editor.GetLength();
                string activePageText = editor.GetText(length + 1);
                string activeTextFlattened = Regex.Replace(activePageText, @"\r\n?|\n", "");
                string modifiedResults = null;
                HashSet<string> unqiueResults = new HashSet<string>();

                // grab each message
                Regex mainSwiftContentPattern = new Regex(@"{.*?(-})+{.*?(:}})");
                //Regex swiftContentPatternWithoutFooter = new Regex(@"{.*?(-})");

                var swiftMessages = mainSwiftContentPattern.Matches(activeTextFlattened);
                foreach (Match swiftMessage in swiftMessages)
                {
                    // new match clear unique results tracker
                    unqiueResults.Clear();

                    //rename fields from :60: to opening_balance for example
                    foreach (var swiftField in swiftInfo.swiftMessageFields)
                    {
                        try
                        {
                            // using swiftMessageField.Value as regex, find match in string
                            string rawMessageField = utils.returnFirstStringMatch(swiftField.Value, swiftMessage.ToString());

                            // only work with unique results since dictionaries contain duplicate regexs
                            if (!unqiueResults.Contains(rawMessageField))
                            {
                                string modifiedMainMessageField = null;
                                try // replace field ID with human readable name
                                {
                                    string humanReadableFieldName = swiftInfo.swiftMessageNames[swiftField.Key];
                                    modifiedMainMessageField = rawMessageField.Replace(swiftField.Key, humanReadableFieldName);
                                }
                                catch //otherwise leave the field ID in tact
                                {
                                    modifiedMainMessageField = rawMessageField;
                                }

                                // only work with lines that actually contain anything
                                if (modifiedMainMessageField.Length > 0)
                                {
                                    unqiueResults.Add(rawMessageField);

                                    string subFieldDetails = utils.buildSwiftSubFieldOutput(rawMessageField, swiftField.Key);

                                    if (":5" == swiftField.Key)
                                    {
                                        modifiedResults = modifiedResults + "-}\n" + modifiedMainMessageField + "\n";
                                    }
                                    else
                                    {
                                        modifiedResults = modifiedResults + modifiedMainMessageField + subFieldDetails;
                                    }

                                }
                            }
                            else
                            {
                                MessageBox.Show("Message Field " + swiftField.Value + " not unique " + swiftField);
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Error converting " + swiftField.Key);
                        }
                    }
                }

                editor.SelectAll();
                editor.ReplaceSel(modifiedResults);

            }
            catch
            {
                MessageBox.Show("Error reading text on current page.\n Are you sure this is a MT950?", "Mismatch Error");
            }
        }

        private static void returnFirstStringMatch(string value, string v)
        {
            throw new NotImplementedException();
        }

        private static string IndexReplace(int index, int length, string reformatted)
        {
            throw new NotImplementedException();
        }

        internal static void PluginCleanUp()
        {
            Win32.WritePrivateProfileString("SomeSection", "SomeKey", someSetting ? "1" : "0", iniFilePath);
        }

    }
}