using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Kbg.NppPluginNET.PluginInfrastructure;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using Kbg.NppPluginNET;

namespace Kbg.NppPluginNET
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

            PluginBase.SetCommand(0, "MyMenuCommand", myMenuFunction, new ShortcutKey(false, false, false, Keys.None));
            PluginBase.SetCommand(1, "MyDockableDialog", myDockableDialog); idMyDlg = 1;
            PluginBase.SetCommand(2, "MT950", MT950);
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

        internal static void MT950()
        {
            // select all content in active page
            editor.SelectAll();
            int length = editor.GetLength();
            string activePageText = editor.GetText(length + 1);
            string activeTextFlattened = Regex.Replace(activePageText, @"\r\n?|\n", "");
            string modifiedResults = null;
            HashSet<string> unqiueResults = new HashSet<string>();

            // grab each message
            Regex swiftContentPattern = new Regex(@"{.*?(-})");
            //Regex swiftContentPattern = new Regex(@"\{4:(?=.*)[^}]+}");

            // basic header fields
            Regex firstContentPattern = new Regex(@"\{1:(?=.*)[^}]+}");
            Regex secondContentPattern = new Regex(@"\{2:(?=.*)[^}]+}");

            var swiftContentMatches = swiftContentPattern.Matches(activeTextFlattened);
            foreach (Match match in swiftContentMatches)
            {
                unqiueResults.Clear();
                string headerOne = firstContentPattern.Match(match.ToString()).ToString() + "\n";
                string headerTwo = secondContentPattern.Match(match.ToString()).ToString() + "\n";
                modifiedResults = modifiedResults + headerOne + headerTwo;

                //rename fields from :60: to opening_balance for example
                foreach (var swiftMessageField in swiftInfo.swiftMessageFields)
                {
                    try
                    {
                        Regex tempRegex = new Regex(swiftMessageField.Value);
                        string swiftField = tempRegex.Match(match.ToString()).ToString();
                        if (!unqiueResults.Contains(swiftField))
                        {
                            string name = swiftInfo.swiftMessageNames[swiftMessageField.Key];
                            swiftField = swiftField.Replace(swiftMessageField.Key, name);
                            // only add lines that actually contain anything
                            if (swiftField.Length > 0)
                            {
                                // add results to unique list as to not double add when regex rules are the same
                                unqiueResults.Add(swiftField);
                                modifiedResults = modifiedResults + swiftField + "\n";
                            }
                            
                        }
                        else
                        {
                            MessageBox.Show(swiftMessageField.Value+ " " + swiftField);
                        }
                        
                    }
                    catch
                    {
                        MessageBox.Show(swiftMessageField.Key);
                    }
                }
                
            }

            editor.SelectAll();
            editor.ReplaceSel(modifiedResults);

        }

        private static string IndexReplace(int index, int length, string reformatted)
        {
            throw new NotImplementedException();
        }

        internal static void PluginCleanUp()
        {
            Win32.WritePrivateProfileString("SomeSection", "SomeKey", someSetting ? "1" : "0", iniFilePath);
        }


        internal static void myMenuFunction()
        {
            MessageBox.Show("Hello N++!");
        }

        internal static void myDockableDialog()
        {
            if (frmMyDlg == null)
            {
                frmMyDlg = new frmMyDlg();

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = frmMyDlg.Handle;
                _nppTbData.pszName = "My dockable dialog";
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, frmMyDlg.Handle);
            }
        }
    }
}