﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using System.Security.AccessControl;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.ComponentModel;


namespace Kicker
{

    public enum TestType
    {
        VBTest,
        TMTest,
        Invalid,
        Both
    };

    public class clsRunRealTest
    {

        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);



        [DllImport("User32.dll")]
        public static extern int GetWindowText(
             IntPtr hWnd,
          ref string lpString,
          int nMaxCount
        );

        [DllImport("User32.dll")]
        public static extern int GetClassName(
          IntPtr hWnd,
          ref string lpClassName,
          int nMaxCount
        );

        public delegate bool WindowEnumDelegate(IntPtr hwnd,
                                         int lParam);

        // declare the API function to enumerate child windows
        [DllImport("user32.dll")]
        public static extern int EnumChildWindows(IntPtr hwnd,
                                                  WindowEnumDelegate del,
                                                  int lParam);

        // declare the GetWindowText API function
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hwnd,
                                               StringBuilder bld, int size);


        [DllImport("user32.dll")]
        public static extern int SendMessage(
              int hWnd,      // handle to destination window
              uint Msg,       // message
              short wParam,  // first message parameter
              long lParam   // second message parameter
              );



        //[DllImport("User32.Dll", EntryPoint = "PostMessageA")]
        [DllImport("User32.dll")]
        static extern bool PostMessage(
            IntPtr hWnd,
            uint msg,
            int wParam,
            int lParam
            );

        [DllImport("User32.dll")] 
        static extern bool CloseWindow(  IntPtr hWnd);

[DllImport("User32.dll")]
        static extern bool ShowWindow( IntPtr hWnd, int nCmdShow);


[DllImport("User32.dll")]
       static extern bool DestroyWindow(IntPtr hWnd);



        const uint WM_KEYDOWN = 0x100;
        const uint WM_KEYUP = 0x0101;

        const short WM_a = 0x41;
        const short WM_b = 0x42;
        const short WM_c = 0x43;

        const int SW_RESTORE = 0x09;


        public string strExecPath;
        public string strParam1;
        public string strParam2;

        public string strScript;
        public string strSteps;

        public IntPtr  hwndRunning ;

        public string strNetProConnectionString1;
        public string strNetProConnectionString2;
        public string strNetProFileLocation;

        public Process VBProcess = null;
        public Process TMProcess = null;

        public IntPtr hwndVB ;
        public IntPtr hwndTM ;

        protected DataTable dtLimits;

        protected DataTable dtKickerTable;

        public string strModel;
        public string strSerial;
        public string strStation;
        public string strRunStation;
        public string strSqlConnectionString;

        public Form1 frmMainForm;

        public Boolean _bcancelled = false;
        public Boolean bUseAccessTablesForTM = false;
        public string strLastExec;

        public delegate void evTestFinished(object sender, clsTestRunStatusEventArgs e);

        public event evTestFinished TestFinished;

        protected Dictionary<string, clsSubTestLimit> theLimits;

        public clsRunRealTest()
        {
        }

        public clsRunRealTest(Dictionary<string, clsSubTestLimit> myLimits)
        {
            theLimits = myLimits;
        }

        public clsRunRealTest(DataTable mydtLimits)
        {
            dtLimits = mydtLimits;
        }

        public clsRunRealTest(DataTable mydtLimits, DataTable myKicker)
        {
            dtLimits = mydtLimits;
            dtKickerTable = myKicker;
        }


        public clsRunRealTest(DataTable mydtLimits, DataTable myKicker, Form1 mainform)
        {
            dtLimits = mydtLimits;
            dtKickerTable = myKicker;
            frmMainForm = mainform;
            strLastExec = "";
        }


        protected virtual void OnTestFinished(clsTestRunStatusEventArgs e)
        {
            if (TestFinished != null)
                TestFinished(this, e);
        }

        public delegate void evUpdateGui(Form1 myForm);
        public event  evUpdateGui GUIUpdate ;

        public virtual void OnUpdateTime(Form1 myForm)
        {
            if (GUIUpdate != null)
                GUIUpdate(myForm);
        }

        public string runVB6TestSync(string strExec)
        {
            try
            {

                System.Diagnostics.ProcessStartInfo procStartInfo =
        new System.Diagnostics.ProcessStartInfo("cmd", "/c " + strExec);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
//                Console.WriteLine(result);
 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return "OK";
        }

        private bool _myVBTaskIsRunning = false;
/*
        private void VBTaskWorker(string strExecString, MyAsyncContext asyncCOntext, out bool cancelled)
        {
            cancelled = false;

            try
            {
            
                System.Diagnostics.ProcessStartInfo procStartInfo =
        new System.Diagnostics.ProcessStartInfo("cmd", "/c " + strExecString);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
//                Console.WriteLine(result);
 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        */



        private void VBTaskWorker(string strExec,  out bool cancelled)
        {
            cancelled = false;

            try
            {
                if ( hwndVB == (IntPtr)0)
                {

                System.Diagnostics.ProcessStartInfo procStartInfo =
        new System.Diagnostics.ProcessStartInfo("cmd", "/c " + strExec);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;

                proc.Start();
                // Get the output into a string
              ///  string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                //                Console.WriteLine(result);


                Thread.Sleep(7000);
                int iPosEXE = strExec.IndexOf(".exe");
                string strUpToExec = strExec.Substring(0, iPosEXE + 4);
                string[] strParts = strUpToExec.Split('\\');
                int iLaststring = strParts.Length;

                string strRealExec = strParts[iLaststring - 1];
                int iExecPos = strRealExec.IndexOf(".exe");
                string strProcessName = strRealExec.Substring(0, iExecPos);
                strLastExec = strProcessName;

                Process[] processes = Process.GetProcessesByName(strProcessName);

                if ( processes.Length > 0 )
                {
                    VBProcess = processes[0];
                    hwndVB = VBProcess.MainWindowHandle;
                }

                } //if ( hwndVB == (IntPtr)0)


       //                         SeedForTestFinsih(frmMainForm.strKickerInstanceName, dtKickerTable.Rows[0]["TFFC_Kicker_Station"].ToString().Trim(), strModel, strSerial, this.frmMainForm.strSqlConnection1);


                                bool bTestFinishFound = false;
                                bool bProcessDied = false;
                                string strTestResult = "";
                                string strTestFINISH_RESULT = "";
                                string strSAPSN_RESULT = "";
                                DateTime dtFinishTest_Return = DateTime.Parse("6/6/2000");
                                string strReturnWait = WaitForVBTestToFinish(strModel, strSerial, 
                                    out bTestFinishFound, out bProcessDied, 
                                    out strTestResult, out strTestFINISH_RESULT,
                                    out strSAPSN_RESULT, out dtFinishTest_Return);

                                // fire event back to UI thread
                                clsTestRunStatusEventArgs myEventArgs= new clsTestRunStatusEventArgs();
                                myEventArgs.strTestType = "VB";
                                myEventArgs.strTestResult = strTestResult;
                                myEventArgs.bTestFinished = bTestFinishFound;
                                myEventArgs.bTestDied = bProcessDied;
                                myEventArgs.hwndTestWindow = hwndVB;
                                myEventArgs.testProcess = VBProcess;
                                OnTestFinished(myEventArgs);

                                myEventArgs.strSAPSN = strSAPSN_RESULT;
                                myEventArgs.dtTestFinish = dtFinishTest_Return;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private delegate void MyVBTaskWorkerDelegate(string strExec,
            out bool cancelled);

        private void MyVBTaskWorkerCompleted(IAsyncResult ar)
        {
            MyVBTaskWorkerDelegate worker =
                (MyVBTaskWorkerDelegate)((AsyncResult)ar).AsyncDelegate;
            AsyncOperation async = (AsyncOperation)ar.AsyncState;
            bool cancelled;

            worker.EndInvoke(out cancelled, ar);

            lock (_sync)
            {
                VBProcess = null;
                _myVBTaskIsRunning = false;
                _myTaskContext = null;
            }

            // raise the completed event
            AsyncCompletedEventArgs completedArgs = new AsyncCompletedEventArgs(null, cancelled, null);

            async.PostOperationCompleted(delegate(object e) { OnMyVBTaskCompleted((AsyncCompletedEventArgs)e); },
                completedArgs);
        }

        // VB AsyncCompletedEventHandler
        public event AsyncCompletedEventHandler MyVBTaskCompleted;

        protected virtual void OnMyVBTaskCompleted(AsyncCompletedEventArgs e)
        {
            if (MyVBTaskCompleted != null)
                MyVBTaskCompleted(this, e);
        }

        public string runVB6TestASync(string strExec)
        {
            try
            {


                frmMainForm.strLastModel = strModel;
                MyVBTaskWorkerDelegate TMWorker = new MyVBTaskWorkerDelegate(VBTaskWorker);
                AsyncCallback completedCallback = new AsyncCallback(MyVBTaskWorkerCompleted);

                lock (_sync)
                {
                    if (_myVBTaskIsRunning)
                    {
                   //     MessageBox.Show("VB already running in runVBTestASync");

                    }
                    

                    AsyncOperation async = AsyncOperationManager.CreateOperation(null);
                    MyAsyncContext context = new MyAsyncContext();
                    bool cancelled;


                    System.Diagnostics.ProcessStartInfo procStartInfo =
new System.Diagnostics.ProcessStartInfo("cmd", "/c " + strExec);

                    // The following commands are needed to redirect the standard output.
                    // This means that it will be redirected to the Process.StandardOutput StreamReader.
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.UseShellExecute = false;
                    // Do not create the black window.
                    procStartInfo.CreateNoWindow = true;
                    // Now we create a process, assign its ProcessStartInfo and start it
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStartInfo;
                    VBProcess = proc;
                    TMWorker.BeginInvoke(strExec, out cancelled, completedCallback, async);
              ///      proc.Start();
                    // Get the output into a string
                 ///   string result = proc.StandardOutput.ReadToEnd();
                    _myVBTaskIsRunning = true;
                    _myTaskContext = context;
                }
/*  WK
                Thread.Sleep(7000);
                int iPosEXE = strExec.IndexOf(".exe") ;
                string strUpToExec = strExec.Substring(0,iPosEXE + 4) ;
                string[] strParts = strUpToExec.Split('\\');
                int iLaststring = strParts.Length;

                string strRealExec = strParts[iLaststring - 1];
                int iExecPos = strRealExec.IndexOf(".exe");
                string strProcessName = strRealExec.Substring(0, iExecPos);

                Process[] processes = Process.GetProcessesByName(strProcessName);

                VBProcess = processes[0];
                hwndVB = VBProcess.MainWindowHandle;
*/
    //            SetForegroundWindow(hwndVB);
      //          Thread.Sleep(2000);
         //       MessageBox.Show("About to send keys");
        //        SetForegroundWindow(hwndVB);
//                SendKeys.Send(strModel);
///                SendKeys.Send("WDKWDK");
//                Thread.Sleep(2000);
 //               SendKeys.Send("{ENTER}");
 //               SendKeys.Send(strSerial);
////wk                frmMainForm.strLastModel = strModel;


////wk                SeedForTestFinsih("KICKER", "AUDIT", strModel, strSerial, this.frmMainForm.strSqlConnection1);

 /*               SendKeys.Send("{ENTER}");
                SendKeys.Send("*");
                SendKeys.Send("11");
                SendKeys.Send("=");

*/

       ///         proc.Start();
                // Get the output into a string
       ///         string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                //                Console.WriteLine(result);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return "OK";
        }


        public string runTMTestSync(string strExec)     //, string strScript, string strSteps)
        {
            try
            {
//                string strFullExecString = "/c" + strExec + " \" " + strScript + " \" " + strSteps;
                System.Diagnostics.ProcessStartInfo procStartInfo =
        new System.Diagnostics.ProcessStartInfo("cmd", "/c " + strExec);
         
                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Get the output into a string
                string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
//                Console.WriteLine(result);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return "OK";
        }

        private bool _myTMTaskIsRunning = false;


/*
        private void TMTaskWorker(string strExecString, MyAsyncContext asyncContext, out bool cancelled)
        {
            cancelled = false;

            try
            {

                try
                {
                    //                string strFullExecString = "/c" + strExec + " \" " + strScript + " \" " + strSteps;
                    System.Diagnostics.ProcessStartInfo procStartInfo =
            new System.Diagnostics.ProcessStartInfo("cmd", "/c " + strExecString);

                    // The following commands are needed to redirect the standard output.
                    // This means that it will be redirected to the Process.StandardOutput StreamReader.
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.UseShellExecute = false;
                    // Do not create the black window.
                    procStartInfo.CreateNoWindow = true;
                    // Now we create a process, assign its ProcessStartInfo and start it
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();
                    // Get the output into a string
                    string result = proc.StandardOutput.ReadToEnd();
                    // Display the command output.
                    //                Console.WriteLine(result);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch ( Exception ex )
            {
                MessageBox.Show("TMTaskWorker returned exception=" + ex.Message);
            }
        }

*/


        private void TMTaskWorker(string strExec,  out bool cancelled)
        {
            cancelled = false;

            try
            {

                if ( hwndTM == (IntPtr)0)
                {
                try
                {

                    //                string strFullExecString = "/c" + strExec + " \" " + strScript + " \" " + strSteps;
                    System.Diagnostics.ProcessStartInfo procStartInfo =
            new System.Diagnostics.ProcessStartInfo("cmd", "/c " + strExec);

                    // The following commands are needed to redirect the standard output.
                    // This means that it will be redirected to the Process.StandardOutput StreamReader.
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.UseShellExecute = false;
                    // Do not create the black window.
                    procStartInfo.CreateNoWindow = true;
                    // Now we create a process, assign its ProcessStartInfo and start it
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStartInfo;
 
                    proc.Start();
                    // Get the output into a string
 //                   string result = proc.StandardOutput.ReadToEnd();
                    // Display the command output.
                    //                Console.WriteLine(result);





                    Thread.Sleep(5000);
                    int iPosEXE = strExec.IndexOf(".exe");
                    string strUpToExec = strExec.Substring(0, iPosEXE + 4);
                    string[] strParts = strUpToExec.Split('\\');
                    int iLaststring = strParts.Length;

                    string strRealExec = strParts[iLaststring - 1];
                    int iExecPos = strRealExec.IndexOf(".exe");
                    string strProcessName = strRealExec.Substring(0, iExecPos);
                    strLastExec = strProcessName;

                    Process[] processes = Process.GetProcessesByName(strProcessName);

                    if (processes.Length > 0)
                    {
                        TMProcess = processes[0];
                        hwndTM = TMProcess.MainWindowHandle;
                        SetForegroundWindow(hwndTM);
                        Thread.Sleep(1000);
                    }
                    
                    

                    //       MessageBox.Show("About to send keys");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                } // if ( hwndTM == (IntPtr)0)
                    SetForegroundWindow(hwndTM);
                    //               SendKeys.Send("{ENTER}");




   //                 SeedForTMTestFinsih(frmMainForm.strKickerInstanceName.Trim(), dtKickerTable.Rows[0]["TFFC_Kicker_Station"].ToString().Trim(), strModel, strSerial, this.frmMainForm.strSqlConnection1);
                    bool bTestFinishFound = false;
                    bool bProcessDied = false;
                    string strTestResult="";

                    string strTestFINISH_RESULT = "";
                    string strSAPSN_RESULT = "";
                    DateTime dtFinishTest_Return = DateTime.Parse("6/6/2001");


           //         frmMainForm.Refresh();
           //         frmMainForm.WindowState = FormWindowState.Maximized;
                    string strReturnWait = WaitForTMTestToFinish(strModel, strSerial, 
                        out bTestFinishFound, out bProcessDied,
                        out strTestResult, out strTestFINISH_RESULT,
                                    out strSAPSN_RESULT, out dtFinishTest_Return);

                    clsTestRunStatusEventArgs myEventArgs = new clsTestRunStatusEventArgs();
                    myEventArgs.strTestType = "TM";
                    myEventArgs.strTestResult = strTestResult;
                    myEventArgs.bTestFinished = bTestFinishFound;
                    myEventArgs.bTestDied = bProcessDied;
                    myEventArgs.hwndTestWindow = hwndTM;
                    myEventArgs.testProcess = TMProcess;

                    myEventArgs.strSAPSN = strSAPSN_RESULT;
                    myEventArgs.dtTestFinish = dtFinishTest_Return;
                    
                    OnTestFinished(myEventArgs);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("TMTaskWorker returned exception=" + ex.Message);
            }
        }
        private delegate void MyTMTaskWorkerDelegate(string strExec,
            out bool cancelled);


        private void MyTMTaskWorkerCompleted(IAsyncResult ar)
        {
            MyTMTaskWorkerDelegate worker =
                (MyTMTaskWorkerDelegate)((AsyncResult)ar).AsyncDelegate;
            AsyncOperation async = (AsyncOperation)ar.AsyncState;
            bool cancelled;

            worker.EndInvoke(out cancelled, ar);


            lock (_sync)
            {
                TMProcess = null;
                _myTMTaskIsRunning = false;
                _myTaskContext = null;
            }

            // raise the completed event
            AsyncCompletedEventArgs completedArgs = new AsyncCompletedEventArgs(null,
              cancelled, null);
            async.PostOperationCompleted(
              delegate(object e) { OnMyTaskCompleted((AsyncCompletedEventArgs)e); },
              completedArgs);
        }


        //  AsyncCompletedEventHandler

        public event AsyncCompletedEventHandler MyTaskCompleted;

        //   public event AsyncCompletedEventHandler MyTaskCompleted;

        protected virtual void OnMyTaskCompleted(AsyncCompletedEventArgs e)
        {
            if (MyTaskCompleted != null)
                MyTaskCompleted(this, e);
        }


        public string runTMTestASync(string strExec)     //, string strScript, string strSteps)
        {
            try
            {
                frmMainForm.strLastModel = strModel;
/*
                //                string strFullExecString = "/c" + strExec + " \" " + strScript + " \" " + strSteps;
                System.Diagnostics.ProcessStartInfo procStartInfo =
        new System.Diagnostics.ProcessStartInfo("cmd", "/c " + strExec);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
 */               
                MyTMTaskWorkerDelegate TMWorker = new MyTMTaskWorkerDelegate(TMTaskWorker);
                AsyncCallback completedCallback = new AsyncCallback(MyTMTaskWorkerCompleted);

                lock (_sync)
                {
                    if (_myTMTaskIsRunning)
                    {
                        MessageBox.Show("Test Manager already running in runTMTestASync");

                    }

                    AsyncOperation async = AsyncOperationManager.CreateOperation(null);
                    MyAsyncContext context = new MyAsyncContext();
                    bool cancelled;
  //                  TMProcess = proc;
                    TMWorker.BeginInvoke(strExec,  out cancelled, completedCallback, async);
               //     TMWorker.BeginInvoke(strExec, context, out cancelled, completedCallback, async);
              ///      proc.Start();
                    // Get the output into a string
                 ///   string result = proc.StandardOutput.ReadToEnd();
                    _myTMTaskIsRunning = true;
                    _myTaskContext = context;
                }
/*
                Thread.Sleep(5000);
                int iPosEXE = strExec.IndexOf(".exe");
                string strUpToExec = strExec.Substring(0, iPosEXE + 4);
                string[] strParts = strUpToExec.Split('\\');
                int iLaststring = strParts.Length;

                string strRealExec = strParts[iLaststring - 1];
                int iExecPos = strRealExec.IndexOf(".exe");
                string strProcessName = strRealExec.Substring(0, iExecPos);

                Process[] processes = Process.GetProcessesByName(strProcessName);

                TMProcess = processes[0];
                hwndTM = TMProcess.MainWindowHandle;
                SetForegroundWindow(hwndTM);
                Thread.Sleep(1000);
                //       MessageBox.Show("About to send keys");
                SetForegroundWindow(hwndTM);
 //               SendKeys.Send("{ENTER}");




                SeedForTMTestFinsih("KICKER", "AUDIT", strModel, strSerial, this.frmMainForm.strSqlConnection1);
                */

       ///         proc.Start();
                // Get the output into a string
       ///         string result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                //                Console.WriteLine(result);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return "OK";
        }




 



        string strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\\..\\BugTypes.MDB";



        public void WaitForFinishSignal()
        {
        }

        public string LookupACSModelSerialInLoci(string strSqlConnection, 
            string strModel, 
            string strSerial,
            out string strProdOrder)
        {
            SqlConnection sqlConnectionState;
            strProdOrder = "";

            try
            {
                using (sqlConnectionState = new SqlConnection(strSqlConnection))
                {
                    sqlConnectionState.Open();
                    if (sqlConnectionState.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdGetProdOrder = sqlConnectionState.CreateCommand();
                            cmdGetProdOrder.CommandType = CommandType.StoredProcedure;
                            cmdGetProdOrder.CommandText = "ame_TFFC_GetProdOrderForSerial"; 


                            cmdGetProdOrder.Parameters.Add("@model", SqlDbType.Char, 20);
                            cmdGetProdOrder.Parameters["@model"].Value = strModel;
                            cmdGetProdOrder.Parameters["@model"].Direction = ParameterDirection.Input;


                            cmdGetProdOrder.Parameters.Add("@serial", SqlDbType.Char, 20);
                            cmdGetProdOrder.Parameters["@serial"].Value = strSerial;
                            cmdGetProdOrder.Parameters["@serial"].Direction = ParameterDirection.Input;

                            SqlDataReader rd = cmdGetProdOrder.ExecuteReader();
                            if (rd.Read())
                            {
                                strProdOrder = rd["ProdOrder"].ToString();
                            }

 
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return "OK";
        }


        public string ProcessModelSerialPair(string strProductionOrder, string strModel, string strSerial, string strSqlConnection, Form1 frmMainForm)
        {
/*
            var query  = from c in dtLimits.AsEnumerable()
                         where c.["abc"] == "abc"
                        select c ;

*/
            this.strModel = strModel;
            this.strSerial = strSerial;
            strSqlConnectionString = strSqlConnection;
            this.frmMainForm = frmMainForm;
            string strExecPath="";
            string strPassNextStation = "";
            try
            {
                //promptForSerialBox(strModel); // bao chuyen cho box

                TestType myTestType = DetermineTestType(strModel, out strExecPath, out strPassNextStation);


                if ( myTestType == TestType.VBTest )
                {

                   // string strProgramPath = table.Rows[0]["strLimit"].ToString();
                    string strProgramPath = strExecPath; //"D:\\data\\audit3\\audit.exe";

                    string dumb = strProgramPath;


                    if (!frmMainForm.strLastTestType.Equals("VB"))
                    {
                        if (hwndTM != (IntPtr)0)
                        {
                            DestroyWindow(hwndTM);
                            hwndTM = (IntPtr)0;
                        }
                    }
                    else
                    {
                      //  if (!frmMainForm.strLastModel.Equals(strModel))
                       // {
                            if (hwndVB != (IntPtr)0)
                            {
                                DestroyWindow(hwndVB);
                                hwndVB = (IntPtr)0;
                            }
                     //   }
                    }
                    
                    string strRunReturn = runVB6TestASync(strProgramPath);

                    frmMainForm.strLastTestType = "VB6";
                   // string strRunReturn = runVB6TestSync(strProgramPath);
     /*               MessageBox.Show("Got Run Return=" + strRunReturn);
                    for (int j = 0; j < 10; j++)
                    {
                        MessageBox.Show("Is VBRunning?");
                    }
     
*/
                    if (strRunReturn.Equals("OK"))
                    {
    //                    frmMainForm.WindowState = FormWindowState.Minimized;

                        bool bTestFinishFound = false;
                        bool bProcessDied = false;


      ////wk                  string strReturnWait = WaitForVBTestToFinish(strModel, strSerial, out bTestFinishFound, out bProcessDied);

                        if (bProcessDied == true)
                        {
                            MessageBox.Show("Process died somehow");
                        }

                        if (bTestFinishFound == true)
                        {
                            frmMainForm.WindowState = FormWindowState.Normal;
                            if (hwndVB != (IntPtr)0)
                            {
                                CloseWindow(hwndVB);
                            }

                        }


                    }

                }
                else
                {


                    if (myTestType == TestType.TMTest)
                    {
                        //#region MỞ NGUỒN COM 6 CHO TM / OPEN PW VIA COM6

                        //Process.Start(@"C:\kicker-pwCOM6\Kicker\bin\Debug\Power Supply Stand Alone for XP\PS3645A on COM 6.exe");

                        //#endregion

                        // we have a test manager script to run

                        string strProdOrder = "";
                        string strLociLookupReturn = LookupACSModelSerialInLoci(strSqlConnection,
                                                        strModel,
                                                        strSerial,
                                                        out strProdOrder);
                        // string strResult = LookupTMInfo(strProdOrder,strModel, strSerial);
                        if (!frmMainForm.strLastTestType.Equals("TM"))
                        {
                            if (hwndVB != (IntPtr)0)
                            {
                                DestroyWindow(hwndVB);
                                hwndVB = (IntPtr)0;
                            }
                        }
                        else
                        {
                           // if (!frmMainForm.strLastModel.Equals(strModel))
                          //  {
                                if (hwndTM != (IntPtr)0)
                                {
                                    DestroyWindow(hwndTM);
                                    hwndTM = (IntPtr)0;
                                }
                           // }
                        }
                        string strResult = LookupTMInfo(strProdOrder, strModel, strSerial);
 
                        frmMainForm.strLastTestType = "TM";


   //                     frmMainForm.WindowState = FormWindowState.Minimized;

                        bool bProcessDied = false;
                        bool bTestFinishFound = false;


                        if (bProcessDied == true)
                        {
                            MessageBox.Show("Process died somehow");
                        }

                        if (bTestFinishFound == true)
                        {
                            frmMainForm.WindowState = FormWindowState.Normal;
                            if (hwndTM != (IntPtr)0)
                            {
                                CloseWindow(hwndTM);
                            }

                        }
                        //                   string strRunReturn = runTMTestSync("exec", "script", "steps");
                    }
                }

//                foreach (DataRow ax in query)
//                {
//                }

 //               DataTable table = query.CopyToDataTable();



 //               if (table.Rows.Count < 1)
 //               {
 //               }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            /*
            IEnumerable<DataRow> query =
    from order in orders.AsEnumerable()
    where order.Field<DateTime>("OrderDate") > new DateTime(2001, 8, 1)
    select order;
            */



            return "OK";
        }


        public string checkNextStationDBLoci(string strModel, string strSerial, string strStation, string strSqlConnection, out bool brework)
        {
            brework = false;
            SqlConnection sqlConnectionState;
            bool bCorrectStation = false;


            string strORTStatus;
            string strORTBin;
            string strORTStart;
            string strPSCSN;
            string strAssmSN;
            string strSAPModel;
            long lSieveByte;
            long lUnitStnByte;
            long lLineByte;
            long lPlantByte;
            string strNext_Station = "";
            using (sqlConnectionState = new SqlConnection(strSqlConnection))
            {
                sqlConnectionState.Open();
                if (sqlConnectionState.State.Equals(ConnectionState.Open))
                {
                    try
                    {

                        IEnumerable<DataRow> acsquery =
    from kicker in dtKickerTable.AsEnumerable()
    where kicker.Field<string>("TFFC_KICKER_Model").Trim() == strModel.Trim() &&
                   kicker.Field<int>("TFFC_KICKER_Rework") == frmMainForm.iDoRework
    select kicker;

                        DataTable dtResult = acsquery.CopyToDataTable<DataRow>();
                        string strCurrentStation = dtResult.Rows[0]["TFFC_KICKER_Station"].ToString();
                        if (dtResult.Rows.Count > 0)
                        {
//                            string strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\\\\svksrv722.psc.pscnet.com\\prd\\dldb\\NetPro\\Tests.mdb;Persist Security Info=True;Jet OLEDB:Database Password=callisto";

                            string strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" ; //\\\\svksrv722.psc.pscnet.com\\prd\\dldb\\NetPro\\Tests.mdb;Persist Security Info=True;Jet OLEDB:Database Password=callisto";
                            strAccessConn += dtResult.Rows[0]["TFFC_KICKER_DBPath"].ToString().Trim();
                            strAccessConn += ";Persist Security Info=True;";

                            string strStationQuery = "SELECT Station_Name, Machine_Name, FactoryGroup_Mask, ProductGroup_Mask, Order_Value, Perform_Test from [STATIONS]";
                            OleDbConnection myAccessConn = null;
                            try
                            {
                                myAccessConn = new OleDbConnection(strAccessConn);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }

                            DataSet dsStations = new DataSet();

                            OleDbCommand myAccessCommand = new OleDbCommand(strStationQuery, myAccessConn);
                            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);

                            myAccessConn.Open();
                            myDataAdapter.Fill(dsStations, "STATIONS");
                            
                            DataTable dtStations = dsStations.Tables[0];
                            for (int i = 0; i < dtStations.Rows.Count; i++)
                            {
                                clsStationInfo objStationInfo = new clsStationInfo();
                                objStationInfo.strStationName = dtStations.Rows[i]["Station_Name"].ToString();
                                objStationInfo.strMachineName = dtStations.Rows[i]["Machine_Name"].ToString();
                                objStationInfo.iFactoryGroup_Mask = Int32.Parse(dtStations.Rows[i]["FactoryGroup_Mask"].ToString()) ;
                                objStationInfo.iProductGroup_Mask = Int32.Parse(dtStations.Rows[i]["ProductGroup_Mask"].ToString());
                                objStationInfo.iOrder_Value = Int32.Parse(dtStations.Rows[i]["Order_Value"].ToString()) ;
                                objStationInfo.strPerform_Test = dtStations.Rows[i]["Perform_Test"].ToString();
                                frmMainForm.listStations.Add(objStationInfo);
                            }

                            myAccessConn.Close();



                            string strLociReturn;

                            strLociReturn = GetLoci(strSerial, out strORTStatus, out strORTBin,
                                out strORTStart, out strPSCSN, out strAssmSN,
                                out strSAPModel, out lSieveByte, out lUnitStnByte,
                                out lLineByte, out lPlantByte, out strNext_Station);


                            if (Char.IsNumber((char)(strNext_Station[strNext_Station.Length - 1])) == true)
                            {
                                strNext_Station = strNext_Station.Substring(0, strNext_Station.Length - 1);
                            }

                          

                            if (Char.IsNumber((char)(strStation[strStation.Length - 1])) == true)
                            {
                                strStation = strStation.Substring(0, strStation.Length - 1);
                            }

                            if ( strNext_Station.Trim().Equals(strNext_Station.Trim()))
                            {
                                return "OK" ;
                            }

                            clsStationInfo objCurrentStation = new clsStationInfo();
                            for (int k = 0; k < frmMainForm.listStations.Count; k++)
                            {
                                if (frmMainForm.listStations[k].strStationName.Trim().Equals(strStation))
                                {
                                    objCurrentStation.iProductGroup_Mask = frmMainForm.listStations[k].iProductGroup_Mask;
                                    objCurrentStation.iOrder_Value = frmMainForm.listStations[k].iOrder_Value;
                                }
                            }

                            for (int j = 0; j < frmMainForm.listStations.Count; j++)
                            {
                                string strTempStn;

                                strTempStn = frmMainForm.listStations[j].strStationName.Trim();
                                if (Char.IsNumber((char)(strTempStn[strTempStn.Length - 1])) == true)
                                {
                                    strTempStn = strTempStn.Substring(0, strTempStn.Length - 1);
                                }
                                if (strTempStn.Trim().Equals(strNext_Station))
                                {
                                    if ((objCurrentStation.iProductGroup_Mask & frmMainForm.listStations[j].iProductGroup_Mask) >0 )
                                    {
                                        if (objCurrentStation.iOrder_Value <= frmMainForm.listStations[j].iOrder_Value)
                                        {
                                            bCorrectStation = true;
                                            if (objCurrentStation.iOrder_Value < frmMainForm.listStations[j].iOrder_Value)
                                            {
                                                brework = true;

                                            }
                                            return "OK";
                                        }
                                    }

                                }



                            }


                        }
                        else
                        {
                            MessageBox.Show("No rows in Kicker for NextStationLoci");
                        }

                    //    if ( dtKickerTable.Rows[0][
                        /*
                        SqlCommand cmdCheckPrevStation = sqlConnectionState.CreateCommand();
                        cmdCheckPrevStation.CommandType = CommandType.StoredProcedure;
                        cmdCheckPrevStation.CommandText = "ame_TFFC_CheckPrevStation";


                        cmdCheckPrevStation.Parameters.Add("@model", SqlDbType.Char, 20);
                        cmdCheckPrevStation.Parameters["@model"].Value = strModel;
                        cmdCheckPrevStation.Parameters["@model"].Direction = ParameterDirection.Input;

                        cmdCheckPrevStation.Parameters.Add("@serial", SqlDbType.Char, 20);
                        cmdCheckPrevStation.Parameters["@serial"].Value = strSerial;
                        cmdCheckPrevStation.Parameters["@serial"].Direction = ParameterDirection.Input;

                        cmdCheckPrevStation.Parameters.Add("@station", SqlDbType.Char, 20);
                        cmdCheckPrevStation.Parameters["@station"].Value = strStation;
                        cmdCheckPrevStation.Parameters["@station"].Direction = ParameterDirection.Input;

                        cmdCheckPrevStation.Parameters.Add("@OK", SqlDbType.Char, 20);
                        cmdCheckPrevStation.Parameters["@OK"].Direction = ParameterDirection.Output;

                        cmdCheckPrevStation.Parameters.Add("@rework", SqlDbType.Char, 20);
                        cmdCheckPrevStation.Parameters["@rework"].Direction = ParameterDirection.Output;
                        */


                        



                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }


            if (bCorrectStation == true)
            {
                return "OK";
            }
            else
            {
                return strNext_Station;
            }
        }

        public string GetLoci(string strACSSerial, out string strORTStatus,
           out string  strORTBin, out string strORTStart, out string strPSCSN,
            out string strAssmSN, out string strSAPModel, out long lSieveByte,
            out long lUnitStnByte, out long lLineByte, 
            out long lPlantByte, out string strNext_Station)
        {
            string strResult = "";

            strORTStatus = "N";
            strORTBin = "";
            strORTStart = "";
            strPSCSN = "";
            strAssmSN = "";
            strSAPModel = "";
            lSieveByte = 0;
            lUnitStnByte = 0;
            lLineByte = 0;
            lPlantByte = 0;
            strStation = "";
            strNext_Station = "";

            SqlConnection sqlConnectionACSEEState;

            try
            {
                using (sqlConnectionACSEEState = new SqlConnection(frmMainForm.strSqlConnection3))
                {
                    sqlConnectionACSEEState.Open();
                    if (sqlConnectionACSEEState.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdGetLoci = sqlConnectionACSEEState.CreateCommand();
                            cmdGetLoci.CommandType = CommandType.StoredProcedure;
                            cmdGetLoci.CommandText = "ame_get_loci";


                            cmdGetLoci.Parameters.Add("@acssn", SqlDbType.Char, 20);
                            cmdGetLoci.Parameters["@acssn"].Value = strACSSerial;
                            cmdGetLoci.Parameters["@acssn"].Direction = ParameterDirection.Input;

                            using (SqlDataReader rd = cmdGetLoci.ExecuteReader())
                            {
                                if (rd.HasRows)
                                {
                                    rd.Read();

                                    strResult = rd[0].ToString();
                                    if (strResult.Trim().Equals("OK"))
                                    {
                                        rd.NextResult() ;
                                        rd.Read();
                                        string tryit = rd["SAP_Model"].ToString().Trim();
                                        strORTStatus = rd["ORT_Status"].ToString().Trim() ;
                                        strORTBin = rd["ORT_Bin"].ToString().Trim() ;
                                        strORTStart = rd["ORT_Start"].ToString().Trim() ;
                                        strPSCSN = rd["PSC_Serial"].ToString().Trim() ;
    
                                        strAssmSN = rd["Assembly_ACSSN"].ToString().Trim() ;
                                        strSAPModel = rd["SAP_Model"].ToString().Trim() ;
                                        lSieveByte = Int32.Parse(rd["SieveByte"].ToString()) ;
                                        lUnitStnByte = Int32.Parse(rd["UnitStnByte"].ToString()) ;
                                        lLineByte = Int32.Parse(rd["LineByte"].ToString() ) ;
                                        lPlantByte = Int32.Parse(rd["PlantByte"].ToString()) ;
                                        strNext_Station = rd["Next_Station_Name"].ToString().Trim();
                                    }
                                    else
                                    {
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return strResult;
        }


        public string [] getNextStationsPerQBuddies(string strModel, string strSerial)
        {



            string [] retStringArray = new string [] { "NextStation"} ;
            return retStringArray;
        }

        public string LookupTMInfo(string strProductionOrder, string strModel, string strSerial)
        {

//            string strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\svksrv722\\prd\\dldb\\NetPro\\Tests.mdb;Persist Security Info=True;Jet OLEDB:Database Password=callisto";
            if ( frmMainForm.bUseTMAccessFiles == false )
  //          if (bUseAccessTablesForTM == false)
            {
                IEnumerable<DataRow> acsquery =
                   from kicker in dtKickerTable.AsEnumerable()
                   where kicker.Field<string>("TFFC_KICKER_Model").Trim() == strModel.Trim() &&
                   kicker.Field<int>("TFFC_KICKER_Rework") == frmMainForm.iDoRework
                   select kicker;
                string strTMCommandParameters = "";
                string strTMCodiceDirectory = "";
                DataTable dtResult = acsquery.CopyToDataTable<DataRow>();
                if (dtResult.Rows.Count > 0)
                {
                    strTMCommandParameters = dtResult.Rows[0]["TFFC_KICKER_TMParameters"].ToString();
                    strTMCodiceDirectory = dtResult.Rows[0]["TFFC_KICKER_TMCodice"].ToString();
                    string strDownloadSpace = frmMainForm.strTMFilesDestDirectory.Trim(); //"c:\\idpmdc\\TA\\TM\\";
                    string strSourceSpace = frmMainForm.strTMFilesSourceDirectory.Trim(); //"\\\\svksrv300\\grpfiles\\prd\\SW-Test\\Winate\\";


                    strDownloadSpace += strTMCodiceDirectory;
                    strSourceSpace += strTMCodiceDirectory;


                    using (new ImpersonateUser("production", "DL", "Prod1234"))
                    {// download xuống local
                        Copy(strSourceSpace, strDownloadSpace);
                    }
                    string strSESFileName = strDownloadSpace.Trim() + "\\" + strTMCodiceDirectory.Trim() + ".SES";

                    System.IO.FileStream fs = System.IO.File.Create(strSESFileName);
                    fs.Close();

                    using (StreamWriter file = new System.IO.StreamWriter(strSESFileName))
                    {
                        string strORDINE = "ORDINE=" + strProductionOrder;
                        file.WriteLine(strORDINE);

                        string strCODICE = "CODICE=" + strModel;
                        file.WriteLine(strCODICE);

                        string strDESCRIZZIONE = "DESCRIZIONE=" ;
                        file.WriteLine(strDESCRIZZIONE);

                        string strSEZIONECODE = "SEZIONECODE=" + "1";
                        file.WriteLine(strSEZIONECODE);

                        string strSEZIONENAME = "SEZIONENAME=" ;
                        file.WriteLine(strSEZIONENAME);

                        string strUSER = "USER=DLSK";
                        file.WriteLine(strUSER);

                        string strPOSTAZZIONE = "POSTAZIONE=SBAROZZI-XP";
                        file.WriteLine(strPOSTAZZIONE);
                    }
                 //   string strExecutionString = "C:\\idpmdc\\winate32\\dltest32.exe  ";
                    string strExecutionString = frmMainForm.strTestManagerRunPath;
                    strExecutionString += strTMCommandParameters;
                    string strExecResult = runTMTestASync(strExecutionString);
                }

            }
            else
            {







                string strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\\\\svksrv722.psc.pscnet.com\\prd\\dldb\\NetPro\\Tests.mdb;Persist Security Info=True;Jet OLEDB:Database Password=callisto";
                string strAccessConn2 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=";
                string strNetProDBLocation = frmMainForm.strNetProConnectionString1.Trim();
                strAccessConn2 += strNetProDBLocation;
                strAccessConn2 +=" ;Persist Security Info=True;Jet OLEDB:Database Password=callisto" ;

                string strAccessSelect = "SELECT DATA0 FROM [TEST-TM]";
                DataSet myDataSet = new DataSet();



                //            string strSelect1 = "SELECT [ASSOCIAZIONI MATERIALI].*, [ASSOCIAZIONI MATERIALI].CODICE
                //FROM [ASSOCIAZIONI MATERIALI]
                //WHERE ((([ASSOCIAZIONI MATERIALI].CODICE)='GD4110-BK-C140'));"


                // SELECT [ASSOCIAZIONI MATERIALI].SEZIONE0, [ASSOCIAZIONI MATERIALI].CODICE, [ASSOCIAZIONI MATERIALI].PARAM0
                // FROM [ASSOCIAZIONI MATERIALI]
                // WHERE ((([ASSOCIAZIONI MATERIALI].CODICE)='GD4110-BK-C140'));



                //SELECT [ASSOCIAZIONI COMUNI].*, [ASSOCIAZIONI COMUNI].CODICE
                //FROM [ASSOCIAZIONI COMUNI]
                //WHERE ((([ASSOCIAZIONI COMUNI].CODICE)='SW90862 Final HHFFC Config. GD4110-BK-C140'));

                //SELECT [TEST-TM].DATA0, [TEST-TM].DATA1, [TEST-TM].CODICE
                //FROM [TEST-TM]
                //WHERE ((([TEST-TM].CODICE)='SW90862'));
                OleDbConnection myAccessConn = null;
                try
                {
                    myAccessConn = new OleDbConnection(strAccessConn);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                string strSeparator = "§";
                char cSeparator = '§';
                // material association lookup
                try
                {
                    string strSelect1 = "SELECT [ASSOCIAZIONI MATERIALI].*, [ASSOCIAZIONI MATERIALI].CODICE FROM [ASSOCIAZIONI MATERIALI] WHERE ((([ASSOCIAZIONI MATERIALI].CODICE)='"; // GD4110-BK-C140'))";
                    strSelect1 += strModel + "'))";


                    OleDbCommand myAccessCommand = new OleDbCommand(strSelect1, myAccessConn);
                    OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);

                    myAccessConn.Open();
                    myDataAdapter.Fill(myDataSet, "ASSOCMATERIAL");

                    DataTable dtASSOCMaterial = myDataSet.Tables[0];

                    string strSezione1;
                    string strParam0;
                    string strDescription;
                    string strSezione0;

                    strSezione0 = dtASSOCMaterial.Rows[0]["Sezione0"].ToString();
                    strSezione1 = dtASSOCMaterial.Rows[0]["Sezione1"].ToString();
                    strParam0 = dtASSOCMaterial.Rows[0]["Param0"].ToString();
                    strDescription = dtASSOCMaterial.Rows[0]["DESCRIZIONE"].ToString();

                    string[] strSession0 = strSezione0.Split(new char[] { '§' });
                    string[] strSessionParams = strSezione1.Split(new char[] { '§' });
                    string[] strParameters = strParam0.Split(new char[] { '§' });


                    string strSelectCommon = "SELECT [ASSOCIAZIONI COMUNI].TEST0, [ASSOCIAZIONI COMUNI].CODICE FROM [ASSOCIAZIONI COMUNI] WHERE ((([ASSOCIAZIONI COMUNI].CODICE)='"; // SW90862 Final HHFFC Config. GD4110-BK-C140'))" ;
                    strSelectCommon += strParameters[1] + "'))";
                    OleDbCommand myCommonAccessCommand = new OleDbCommand(strSelectCommon, myAccessConn);
                    OleDbDataAdapter myCommonDataAdapter = new OleDbDataAdapter(myCommonAccessCommand);

                    //                myAccessConn.Open();
                    DataSet dsCommon = new DataSet();
                    myCommonDataAdapter.Fill(dsCommon, "ASSOCCOMMON");

                    DataTable dtCommon = dsCommon.Tables[0];

                    string strTest0;

                    strTest0 = dtCommon.Rows[0]["Test0"].ToString();

                    string[] strTestZero = strTest0.Split(new char[] { '§' });




                    string strTestQuery = "SELECT [TEST-TM].DATA0, [TEST-TM].DATA1, [TEST-TM].CODICE FROM [TEST-TM] WHERE ((([TEST-TM].CODICE)='"; // SW90862'))" ;
                    strTestQuery += strTestZero[1] + "'))";
                    OleDbCommand myTestAccessCommand = new OleDbCommand(strTestQuery, myAccessConn);
                    OleDbDataAdapter myTestDataAdapter = new OleDbDataAdapter(myTestAccessCommand);

                    //                myAccessConn.Open();
                    DataSet dsTest = new DataSet();
                    myTestDataAdapter.Fill(dsTest, "TESTTM");

                    DataTable dtTest = dsTest.Tables[0];

                    string strTestData0 = dtTest.Rows[0]["Data0"].ToString();
                    string strTestData1 = dtTest.Rows[0]["Data1"].ToString();
                    string strTestCodice = dtTest.Rows[0]["Codice"].ToString();



                    string strDownloadSpace = "c:\\idpmdc\\TA\\TM\\";
                    string strSourceSpace  ;


                    strDownloadSpace += strTestCodice;

                    strSourceSpace = strTestCodice;



                    using (new ImpersonateUser("production", "PSC", "Prod1234"))
                    {
                        Copy(strSourceSpace, strDownloadSpace);
                    }

                    // make .SES file
                    string strSESFileName = strDownloadSpace + "\\" + strTestCodice + ".SES";
                    /*
                                   if ( File.Exists(strSESFileName))
                                   {
                                       File.Delete(strSESFileName) ;
                                   }

                                   File.Create(strSESFileName) ;
                   */
                    System.IO.FileStream fs = System.IO.File.Create(strSESFileName);
                    fs.Close();

                    using (StreamWriter file = new System.IO.StreamWriter(strSESFileName))
                    {
                        string strORDINE = "ORDINE=" + strProductionOrder;
                        file.WriteLine(strORDINE);

                        string strCODICE = "CODICE=" + strTestZero[2].ToString();
                        file.WriteLine(strCODICE);

                        string strDESCRIZZIONE = "DESCRIZIONE=" + strDescription;
                        file.WriteLine(strDESCRIZZIONE);

                        string strSEZIONECODE = "SEZIONECODE=" + "1";
                        file.WriteLine(strSEZIONECODE);

                        string strSEZIONENAME = "SEZIONENAME=" + strSession0[0].ToString();
                        file.WriteLine(strSEZIONENAME);

                        string strUSER = "USER=DLSK";
                        file.WriteLine(strUSER);

                        string strPOSTAZZIONE = "POSTAZIONE=SBAROZZI-XP";
                        file.WriteLine(strPOSTAZZIONE);
                    }

                    string strExecutionString = "C:\\idpmdc\\winate32\\dltest32.exe  c:\\idpmdc\\TA\\TM\\";

                    strExecutionString += strTestZero[1].ToString() + "\\";

                    strExecutionString += strTestData1 + " ";

                    strExecutionString += "ACS_" + strTestZero[2].ToString();

                    ///   string strExecResult = runTMTestSync(strExecutionString) ;

                    string strExecResult = runTMTestASync(strExecutionString);
                }
                ///     Thread.Sleep(20000);
                //          for (int j = 0; j < 10; j++)
                //          {
                //              MessageBox.Show("Is TMRunning?");
                //          }
                //                ORDINE=000700036340
                //                CODICE=GD4110-BK-C140
                //                DESCRIZIONE=GRYPHON GD4110 SH5105 KIT BLACK
                //                SEZIONECODE=1
                //                SEZIONENAME=Final Test
                //                USER=DLSK
                //                POSTAZIONE=SBAROZZI-XP

                    /*
                    if (Directory.Exists(strDownloadSpace))
                    {
                        string strDownloadFile = strDownloadSpace + "\\" + strTestData1;

                        if (File.Exists(strDownloadFile))
                        {
                            DateTime fLastFileDateTime = File.GetLastWriteTime(strDownloadFile);
                            DateTime fNow = DateTime.Now;

                            string strLastFileDate = fLastFileDateTime.ToShortDateString().Trim();
                            string strNow = fNow.ToShortDateString().Trim();

                            if (strNow.Equals(strLastFileDate))
                            {
                            }
                        }

                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(strDownloadSpace);
     
                    }
                    */


                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }


            return "OK";
        }


        // Routines to Copy entire data structure over to local computer
            public static void Copy(string sourceDirectory, string targetDirectory)     
            {

                try
                {
                    DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
                    DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
                    CopyAll(diSource, diTarget);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }      
        
            public static void CopyAll(DirectoryInfo source, DirectoryInfo target)     
            {

                try
                {
                    // Check if the target directory exists, if not, create it.         
                    if (Directory.Exists(target.FullName) == false)
                    {
                        Directory.CreateDirectory(target.FullName);
                    }

                    // Copy each file into it's new directory.         
                    foreach (FileInfo fi in source.GetFiles())
                    {
                        Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                        
                        

                        string strCopyToPath = Path.Combine(target.ToString(), fi.Name);
                        string strCopyFromPath = Path.Combine(fi.Directory.ToString(), fi.Name);

                        if (File.Exists(strCopyToPath))
                        {
                            DateTime fLastFileDateTime;
                            string strLastFileDate;
                            DateTime fNow;
                            string strNow;

                            fLastFileDateTime = File.GetLastWriteTime(strCopyToPath);
                            fNow = DateTime.Now;

                            strLastFileDate = fLastFileDateTime.ToShortDateString().Trim();
                            strNow = fNow.ToShortDateString().Trim();
                            if (!strNow.Equals(strLastFileDate))
                            {
                                File.Delete(strCopyToPath);
                                File.Copy(strCopyFromPath, strCopyToPath);
                                File.SetLastWriteTime(strCopyToPath, DateTime.Now);
                            }


                        }
                        else
                        {
                            File.Copy(strCopyFromPath, strCopyToPath);
                            File.SetLastWriteTime(strCopyToPath, DateTime.Now);
                        }
                        //fi.CopyTo(strCopyPath, true);
                    }

                    // Copy each subdirectory using recursion.         
                    foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
                    {
                        DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                        CopyAll(diSourceSubDir, nextTargetSubDir);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            } 



        public string writeACSTestLog(string strSqlConnection,
            string strTestID,
            string strModel,
            string strSerial,
            string strStation,
            string strPassFail,
            string strFirstRun,
            int iACSMode)
        {
            SqlConnection sqlConnectionACSEE;

            try
            {
                using (sqlConnectionACSEE = new SqlConnection(strSqlConnection))
                {
                    sqlConnectionACSEE.Open();
                    if (sqlConnectionACSEE.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdWriteTestlog = sqlConnectionACSEE.CreateCommand();
                            cmdWriteTestlog.CommandType = CommandType.StoredProcedure;
                            cmdWriteTestlog.CommandText = "ame_test_finish";


                            cmdWriteTestlog.Parameters.Add("@aserial", SqlDbType.Char, 20);
                            cmdWriteTestlog.Parameters["@aserial"].Value = strSerial;
                            cmdWriteTestlog.Parameters["@aserial"].Direction = ParameterDirection.Input;

                            cmdWriteTestlog.Parameters.Add("@sap", SqlDbType.Char, 20);
                            cmdWriteTestlog.Parameters["@sap"].Value = strModel;
                            cmdWriteTestlog.Parameters["@sap"].Direction = ParameterDirection.Input;

                            cmdWriteTestlog.Parameters.Add("@sname", SqlDbType.Char, 20);
                            cmdWriteTestlog.Parameters["@sname"].Value = strStation;
                            cmdWriteTestlog.Parameters["@sname"].Direction = ParameterDirection.Input;

                            cmdWriteTestlog.Parameters.Add("@testid", SqlDbType.Char, 50);
                            cmdWriteTestlog.Parameters["@testid"].Value = strTestID;
                            cmdWriteTestlog.Parameters["@testid"].Direction = ParameterDirection.Input;

                            cmdWriteTestlog.Parameters.Add("@pass", SqlDbType.Char, 3);
                            cmdWriteTestlog.Parameters["@pass"].Value = strPassFail;
                            cmdWriteTestlog.Parameters["@pass"].Direction = ParameterDirection.Input;

                            cmdWriteTestlog.Parameters.Add("@first", SqlDbType.Char, 2);
                            cmdWriteTestlog.Parameters["@first"].Value = strFirstRun;
                            cmdWriteTestlog.Parameters["@first"].Direction = ParameterDirection.Input;

                            cmdWriteTestlog.Parameters.Add("@acsmode", SqlDbType.Int);
                            cmdWriteTestlog.Parameters["@acsmode"].Value = iACSMode;
                            cmdWriteTestlog.Parameters["@acsmode"].Direction = ParameterDirection.Input;

                            SqlDataReader rd = cmdWriteTestlog.ExecuteReader();
                            if (rd.Read())
                            {
                                int iResult = Int32.Parse(rd[0].ToString()) ;
                                if (iResult == 1)
                                {
                                    
                                }
                                else
                                {
                                    MessageBox.Show("Error on TestFinish=" + iResult.ToString());
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            return "OK";
        }


        public string writeACSSubtestLog(string strSqlConnection,
            string strTestID,
            string strSerial,
            string strStation,
            string strSubtestName,
            string strPassFail,
            string strStrValue,
            int iIntValue,
            float fltFloatValue,
            string strUnits,
            string strComment)
        {


            SqlConnection sqlConnectionACSEE;

            try
            {
                using (sqlConnectionACSEE = new SqlConnection(strSqlConnection))
                {
                    sqlConnectionACSEE.Open();
                    if (sqlConnectionACSEE.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdWriteSubTestlog = sqlConnectionACSEE.CreateCommand();
                            cmdWriteSubTestlog.CommandType = CommandType.StoredProcedure;
                            cmdWriteSubTestlog.CommandText = "ame_test_finish";


                            cmdWriteSubTestlog.Parameters.Add("@aserial", SqlDbType.Char, 20);
                            cmdWriteSubTestlog.Parameters["@aserial"].Value = strSerial;
                            cmdWriteSubTestlog.Parameters["@aserial"].Direction = ParameterDirection.Input;

                            cmdWriteSubTestlog.Parameters.Add("@sname", SqlDbType.Char, 20);
                            cmdWriteSubTestlog.Parameters["@sname"].Value = strStation;
                            cmdWriteSubTestlog.Parameters["@sname"].Direction = ParameterDirection.Input;

                            cmdWriteSubTestlog.Parameters.Add("@subname", SqlDbType.Char, 30);
                            cmdWriteSubTestlog.Parameters["@subname"].Value = strSubtestName;
                            cmdWriteSubTestlog.Parameters["@subname"].Direction = ParameterDirection.Input;


                            cmdWriteSubTestlog.Parameters.Add("@testid", SqlDbType.Char, 50);
                            cmdWriteSubTestlog.Parameters["@testid"].Value = strTestID;
                            cmdWriteSubTestlog.Parameters["@testid"].Direction = ParameterDirection.Input;

                            cmdWriteSubTestlog.Parameters.Add("@Pass", SqlDbType.Char, 3);
                            cmdWriteSubTestlog.Parameters["@Pass"].Value = strPassFail;
                            cmdWriteSubTestlog.Parameters["@Pass"].Direction = ParameterDirection.Input;

                            cmdWriteSubTestlog.Parameters.Add("@strval", SqlDbType.Char, 20);
                            cmdWriteSubTestlog.Parameters["@strval"].Value = strStrValue;
                            cmdWriteSubTestlog.Parameters["@strval"].Direction = ParameterDirection.Input;

                            cmdWriteSubTestlog.Parameters.Add("@intval", SqlDbType.Int);
                            cmdWriteSubTestlog.Parameters["@intval"].Value = iIntValue;
                            cmdWriteSubTestlog.Parameters["@intval"].Direction = ParameterDirection.Input;

                            cmdWriteSubTestlog.Parameters.Add("@floatvalue", SqlDbType.Real);
                            cmdWriteSubTestlog.Parameters["@floatvalue"].Value = fltFloatValue;
                            cmdWriteSubTestlog.Parameters["@floatvalue"].Direction = ParameterDirection.Input;

                            cmdWriteSubTestlog.Parameters.Add("@units", SqlDbType.Char, 30);
                            cmdWriteSubTestlog.Parameters["@units"].Value = strUnits;
                            cmdWriteSubTestlog.Parameters["@units"].Direction = ParameterDirection.Input;

                            cmdWriteSubTestlog.Parameters.Add("@comment", SqlDbType.Char, 80);
                            cmdWriteSubTestlog.Parameters["@comment"].Value = strStrValue;
                            cmdWriteSubTestlog.Parameters["@comment"].Direction = ParameterDirection.Input;

                            SqlDataReader rd = cmdWriteSubTestlog.ExecuteReader();
                            if (rd.Read())
                            {
                                int iResult = Int32.Parse(rd[0].ToString());
                                if (iResult == 1)
                                {

                                }
                                else
                                {
                                    MessageBox.Show("Error on TestFinish=" + iResult.ToString());
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            return "OK";
        }


        internal class MyAsyncContext
        {
            private readonly object _sync = new object();
            private bool _isCancelling = false;

            public bool IsCancelling
            {
                get
                {
                    lock (_sync) { return _isCancelling; }
                }
            }

            public void Cancel()
            {
                lock (_sync) { _isCancelling = true; }
            }
        }

        private MyAsyncContext _myTaskContext = null;
        private readonly object _sync = new object();

        public void CancelAsync()
        {
            lock (_sync)
            {
                if (_myTaskContext != null)
                    _myTaskContext.Cancel();
            }
        }



        public string SeedForTestFinsih(string strStation, string strRunStation, string strModel, string strACSSerial, string strSqlConnectionString)
        {

            SqlConnection sqlConnectionACSEE;

            try
            {
                using (sqlConnectionACSEE = new SqlConnection(strSqlConnectionString))
                {
                    sqlConnectionACSEE.Open();
                    if (sqlConnectionACSEE.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdSeedForTestFinish = sqlConnectionACSEE.CreateCommand();
                            cmdSeedForTestFinish.CommandType = CommandType.StoredProcedure;
                            cmdSeedForTestFinish.CommandText = "ame_TFFC_SeedForTestFinish";

                            cmdSeedForTestFinish.Parameters.Add("@station", SqlDbType.Char, 20);
                            cmdSeedForTestFinish.Parameters["@station"].Value = strStation;
                            cmdSeedForTestFinish.Parameters["@station"].Direction = ParameterDirection.Input;

                            cmdSeedForTestFinish.Parameters.Add("@runstation", SqlDbType.Char, 20);
                            cmdSeedForTestFinish.Parameters["@runstation"].Value = strRunStation;
                            cmdSeedForTestFinish.Parameters["@runstation"].Direction = ParameterDirection.Input;

                            cmdSeedForTestFinish.Parameters.Add("@model", SqlDbType.Char, 20);
                            cmdSeedForTestFinish.Parameters["@model"].Value = strModel;
                            cmdSeedForTestFinish.Parameters["@model"].Direction = ParameterDirection.Input;


                            cmdSeedForTestFinish.Parameters.Add("@ACSSerial", SqlDbType.Char, 20);
                            cmdSeedForTestFinish.Parameters["@ACSSerial"].Value = strACSSerial;
                            cmdSeedForTestFinish.Parameters["@ACSSerial"].Direction = ParameterDirection.Input;


                            cmdSeedForTestFinish.Parameters.Add("@Result", SqlDbType.Char, 80);
                            cmdSeedForTestFinish.Parameters["@Result"].Direction = ParameterDirection.Output;



                            SqlDataReader rd = cmdSeedForTestFinish.ExecuteReader();
                            if (rd.Read())
                            {
                                int iResult = Int32.Parse(rd[0].ToString());
                                if (iResult == 1)
                                {

                                }
                                else
                                {
                                    MessageBox.Show("Error on TestFinish=" + iResult.ToString());
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




            return "OK";
        }


        public string SeedForTMTestFinsih(string strStation, string strRunStation, string strModel, string strACSSerial, string strSqlConnectionString)
        {

            SqlConnection sqlConnectionACSEE;

            try
            {
                using (sqlConnectionACSEE = new SqlConnection(strSqlConnectionString))
                {
                    sqlConnectionACSEE.Open();
                    if (sqlConnectionACSEE.State.Equals(ConnectionState.Open))
                    {
                        try
                        {

                            string strMachineName = "";

                            strMachineName = System.Environment.MachineName;

                            strMachineName += "i1";
                            SqlCommand cmdSeedForTestFinish = sqlConnectionACSEE.CreateCommand();
                            cmdSeedForTestFinish.CommandType = CommandType.StoredProcedure;
                            cmdSeedForTestFinish.CommandText = "ame_TFFC_SeedForTestFinish";

                            cmdSeedForTestFinish.Parameters.Add("@station", SqlDbType.Char, 20);
                            cmdSeedForTestFinish.Parameters["@station"].Value = strMachineName;
                            cmdSeedForTestFinish.Parameters["@station"].Direction = ParameterDirection.Input;

                            cmdSeedForTestFinish.Parameters.Add("@runstation", SqlDbType.Char, 20);
                            cmdSeedForTestFinish.Parameters["@runstation"].Value = strMachineName;
                            cmdSeedForTestFinish.Parameters["@runstation"].Direction = ParameterDirection.Input;

                            cmdSeedForTestFinish.Parameters.Add("@model", SqlDbType.Char, 20);
                            cmdSeedForTestFinish.Parameters["@model"].Value = strModel;
                            cmdSeedForTestFinish.Parameters["@model"].Direction = ParameterDirection.Input;


                            cmdSeedForTestFinish.Parameters.Add("@ACSSerial", SqlDbType.Char, 20);
                            cmdSeedForTestFinish.Parameters["@ACSSerial"].Value = strACSSerial;
                            cmdSeedForTestFinish.Parameters["@ACSSerial"].Direction = ParameterDirection.Input;


                            cmdSeedForTestFinish.Parameters.Add("@Result", SqlDbType.Char, 80);
                            cmdSeedForTestFinish.Parameters["@Result"].Direction = ParameterDirection.Output;



                            SqlDataReader rd = cmdSeedForTestFinish.ExecuteReader();
                            if (rd.Read())
                            {
                                int iResult = Int32.Parse(rd[0].ToString());
                                if (iResult == 1)
                                {

                                }
                                else
                                {
                                    MessageBox.Show("Error on TestFinish=" + iResult.ToString());
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




            return "OK";
        }

        public void promptForSerialBox(string strModel)
        {

            IEnumerable<DataRow> acsquery =
                from kicker in dtKickerTable.AsEnumerable()
                where kicker.Field<string>("TFFC_KICKER_Model").Trim() == strModel.Trim() &&
                   kicker.Field<int>("TFFC_KICKER_Rework") == frmMainForm.iDoRework
                select kicker;

            DataTable dtResult = acsquery.CopyToDataTable<DataRow>();
            if (dtResult.Rows.Count > 0)
            {
                string strPromptString;
                string strBinaryPortString;

                frmMainForm.dtKicker = new DataTable();
                frmMainForm.dtKicker = dtResult.Clone();
                frmMainForm.drKickerRowforModel = dtResult.Rows[0];
                strPromptString = dtResult.Rows[0]["TFFC_KICKER_SerialPortConfig"].ToString().Trim();
                strBinaryPortString = dtResult.Rows[0]["TFFC_KICKER_BinarySerialPortConfig"].ToString().Trim();

                if (strPromptString.Trim().Length > 0)
                {
                    if (frmMainForm.bPromptSerialSwitchBox == true)
                    {
                        MessageBox.Show(strPromptString);
                    }
                    else   // send command to serial controller
                    {
                        Boolean blnUSBPWRON = false;
                        int iUSBPWRON = 0;
                        Boolean blnAuxEnable = false;
                        int iAuxEnable = 0;
                        int iPort1;
                        short Port1 = -1;
                        int Port2= 0;
                        int ControlPort = 2; //Comport = COM2:
                        int iTimeOut = 5;
                        float TimeOut = 5;
                        string[] promptTokens = strBinaryPortString.Split(frmMainForm.cSwitchBoxBinarySeparator);
                        for (int k = 0; k < promptTokens.Length; k++)
                        {
                            string strCurrentToken = promptTokens[k];
                            if (strCurrentToken.Length > 5)
                            {
                                switch (strCurrentToken.Substring(0, 5))
                                {
                                    case "PORT1":
                                        if (Int32.TryParse(strCurrentToken.Substring(6), out iPort1) == false)
                                        {
                                            iPort1 = 2;
                                        }
                                        Port1 = (short)iPort1;
                                        break;
                                    case "PORT2":
                                        if (Int32.TryParse(strCurrentToken.Substring(6), out Port2) == false)
                                        {
                                            Port2 = 0;
                                        }
                                        break;
                                    case "CPORT":
                                        if (Int32.TryParse(strCurrentToken.Substring(6), out ControlPort) == false)
                                        {
                                            ControlPort = 2;
                                        }

                                        break;
                                    case "TMOUT":

                                        if (Int32.TryParse(strCurrentToken.Substring(6), out iTimeOut) == false)
                                        {
                                            iTimeOut = 5;
                                        }
                                        TimeOut = (float)iTimeOut;
                                        break;
                                    case "USBON":
                                        if (Int32.TryParse(strCurrentToken.Substring(6), out iUSBPWRON) == false)
                                        {
                                            iUSBPWRON = 0;
                                        }
                                        if (iUSBPWRON == 1)
                                        {
                                            blnUSBPWRON = true;
                                        }
                                        else
                                        {
                                            blnUSBPWRON = false;
                                        }
                                        break;
                                    case "AUXEN":
                                        if (Int32.TryParse(strCurrentToken.Substring(6), out iAuxEnable) == false)
                                        {
                                            iAuxEnable = 0;
                                        }

                                        if (iAuxEnable == 1)
                                        {
                                            blnAuxEnable = true;
                                        }
                                        else
                                        {
                                            blnAuxEnable = false;
                                        }

                                        break;

                                    case "BLULT":
                                        if (Int32.TryParse(strCurrentToken.Substring(6), out iAuxEnable) == false)
                                        {
                                            iAuxEnable = 0;
                                        }

                                        if (iAuxEnable == 1)
                                        {
                                            blnAuxEnable = true;
                                        }
                                        else
                                        {
                                            blnAuxEnable = false;
                                        }

                                        break;

                                }
                            }
                        }
                        SKFFCBox myBoxControl = new SKFFCBox();
                        myBoxControl.PortSelect(ControlPort, (long)Port1, blnUSBPWRON, TimeOut, blnAuxEnable, (long)Port2);
                    }
                }
            }
        }

        public TestType DetermineTestType(string strModel, out string strExecPath, out string strPassNextStation)
        {

            strExecPath = "";
            strPassNextStation = "";
            
/*
            IEnumerable<DataRow> acsquery =
                  from limit in dtLimits.AsEnumerable()
                  where limit.Field<string>("SAP_Model_Name").Trim() == strModel.Trim() &&
                  limit.Field<string>("Subtest_Name").Trim() == "ACSTEST"
                  select limit;
*/
            try
            {

                IEnumerable<DataRow> acsquery =
                    from kicker in dtKickerTable.AsEnumerable()
                    where kicker.Field<string>("TFFC_KICKER_Model").Trim() == strModel.Trim() &&
                       kicker.Field<int>("TFFC_KICKER_Rework") == frmMainForm.iDoRework
                    select kicker;

                DataTable dtResult = acsquery.CopyToDataTable<DataRow>();
                if (dtResult.Rows.Count > 0)
                {

                    frmMainForm.dtKicker = new DataTable();
                    frmMainForm.dtKicker = dtResult.Clone();
                    frmMainForm.drKickerRowforModel = dtResult.Rows[0];
                    if (dtResult.Rows[0]["TFFC_KICKER_TestType"].ToString().Trim().Equals("VB"))
                    {
                        strExecPath = dtResult.Rows[0]["TFFC_KICKER_RunPath"].ToString();
                        strPassNextStation = dtResult.Rows[0]["TFFC_KICKER_QBUDDIES"].ToString();
                        return TestType.VBTest;
                    }
                    else
                    {
                        return TestType.TMTest;
                    }
                }
                else
                {
                    MessageBox.Show("NO Entry in TFFC_Kicker_Table for model");
                    return TestType.Invalid;
                }

                /*
                            IEnumerable<DataRow> TMquery =
                                  from limit in dtLimits.AsEnumerable()
                                  where limit.Field<string>("SAP_Model_Name").Trim() == strModel.Trim() &&
                                  limit.Field<string>("Subtest_Name").Trim() == "TMTEST"
                                  select limit;
                */
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error - check if model is in TFFC_Kicker Table!!!");
                return TestType.Invalid;
            }
        }


        public string WaitForVBTestToFinish(string strModel, string strSerial, 
            out bool bTestFinished, out bool bProcessDied, 
            out string strTestResult,out string strTestFinish_Result_Return,
            out string strSAPSN_Return, out DateTime dtFinishTest_Return)
        {
           bool bTestFinishFound = false;
            bProcessDied = false;
            int iTestLoop = 0;
            strTestResult = "";


            string strSAPSN = "";
            string strTESTFINISH_RESULT = "";
            DateTime dtTestFinishTime = DateTime.Parse("12/12/2000");
            while ((bTestFinishFound == false) && (bProcessDied == false))
            {
                // poll to see if test finished yet
          //      frmMainForm.Refresh();
          //      frmMainForm.Invalidate();
               
                Thread.Sleep(500);
                Process[] processes = Process.GetProcessesByName(strLastExec);
                if (processes.Count() < 1)
                {
                    bProcessDied = true;
                }
                else
                {
                    if (VBProcess == null)
                    {
                        bProcessDied = true;
                    }
                    else  // check if evidence of test finish
                    {

                        /*
                        SqlConnection sqlConnectionACSEE;
                        try
                        {
                            using (sqlConnectionACSEE = new SqlConnection(frmMainForm.strSqlConnection1))
                            {
                                sqlConnectionACSEE.Open();
                                if (sqlConnectionACSEE.State.Equals(ConnectionState.Open))
                                {
                                    try
                                    {
                                        SqlCommand cmdCheckForFinishRecord = sqlConnectionACSEE.CreateCommand();
                                        cmdCheckForFinishRecord.CommandType = CommandType.StoredProcedure;
                                        cmdCheckForFinishRecord.CommandText = "ame_TFFC_CheckForSerialFinish";


                                        cmdCheckForFinishRecord.Parameters.Add("@model", SqlDbType.Char, 20);
                                        cmdCheckForFinishRecord.Parameters["@model"].Value = strModel;
                                        cmdCheckForFinishRecord.Parameters["@model"].Direction = ParameterDirection.Input;


                                        cmdCheckForFinishRecord.Parameters.Add("@serial", SqlDbType.Char, 20);
                                        cmdCheckForFinishRecord.Parameters["@serial"].Value = strSerial;
                                        cmdCheckForFinishRecord.Parameters["@serial"].Direction = ParameterDirection.Input;



                                        cmdCheckForFinishRecord.Parameters.Add("@kickerstation", SqlDbType.Char, 20);
                                        cmdCheckForFinishRecord.Parameters["@kickerstation"].Value = frmMainForm.strKickerInstanceName.Trim();
                                        cmdCheckForFinishRecord.Parameters["@kickerstation"].Direction = ParameterDirection.Input;



                                        cmdCheckForFinishRecord.Parameters.Add("@Result", SqlDbType.Char, 80);
                                        cmdCheckForFinishRecord.Parameters["@Result"].Direction = ParameterDirection.Output;

                                        using (SqlDataReader rd = cmdCheckForFinishRecord.ExecuteReader())
                                        {
                                            if (rd.HasRows)
                                            {
                                                rd.Read();

                                                string strResult = rd["Result"].ToString();
                                                strSAPSN = rd["SAPSN"].ToString().Trim();
                                                strTESTFINISH_RESULT = rd["TESTFINISH_RESULT"].ToString().Trim();
                                                dtTestFinishTime = DateTime.Parse(rd["TESTFINISHTIME"].ToString().Trim());


                                                if (strResult.Trim().Equals("P") || strResult.Trim().Equals("F"))
                                                {
                                                    bTestFinishFound = true;
                                                    if (strResult.Trim().Equals("P"))
                                                    {
                                                        strTestResult = strResult.Trim();
                                                    }
                                                    else
                                                    {
                                                        if (strResult.Trim().Equals("F"))
                                                        {
                                                            strTestResult = strResult.Trim();
                                                        }
                                                        else
                                                        {
                                                            strTestResult = strResult.Trim();
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }



                        */
                    }
                }


            //    OnUpdateTime(frmMainForm);
                iTestLoop++;
                if (iTestLoop >frmMainForm.iMaxVBHalfSecondLoops)
                {

                    bTestFinishFound = true;
                }
            }

            if (bTestFinishFound == true)
            {
                bTestFinished = true;
            }
            else
            {
                bTestFinished = false;
            }
            strTestFinish_Result_Return = strTESTFINISH_RESULT;
            strSAPSN_Return = strSAPSN;
            dtFinishTest_Return = dtTestFinishTime;
            return "OK";
        }




        public string WaitForTMTestToFinish(string strModel, string strSerial, 
            out bool bTestFinished, out bool bProcessDied, 
            out string strTestResult, out string strTestFinish_Result_Return,
            out string strSAPSN_Return, out DateTime dtFinishTest_Return)
        {
            bool bTestFinishFound = false;
            bProcessDied = false;
            int iTestLoop = 0;
            strTestResult = "";

            string strSAPSN = "";
            string strTESTFINISH_RESULT = "";
            DateTime dtTestFinishTime = DateTime.Parse("12/12/2001") ;

            while ((bTestFinishFound == false) && (bProcessDied == false))
            {
                // poll to see if test finished yet

                Thread.Sleep(500);
                Process[] processes = Process.GetProcessesByName(strLastExec);
                if (processes.Count() < 1)
                {
                    bProcessDied = true;

                }
                else
                {
                    if (TMProcess == null)
                    {
                        bProcessDied = true;
                    }
                    else  // check if evidence of test finish
                    {
                        /*
                        SqlConnection sqlConnectionACSEE;
                        try
                        {
                            using (sqlConnectionACSEE = new SqlConnection(frmMainForm.strSqlConnection1))
                            {
                                sqlConnectionACSEE.Open();
                                if (sqlConnectionACSEE.State.Equals(ConnectionState.Open))
                                {
                                    try
                                    {
                                        string strMachineName = "";

                                        strMachineName = System.Environment.MachineName;

                                        strMachineName += "i1";

                                        SqlCommand cmdCheckForTMFinishRecord = sqlConnectionACSEE.CreateCommand();
                                        cmdCheckForTMFinishRecord.CommandType = CommandType.StoredProcedure;
                                        cmdCheckForTMFinishRecord.CommandText = "ame_TFFC_CheckForTMFinish";


                                        cmdCheckForTMFinishRecord.Parameters.Add("@model", SqlDbType.Char, 20);
                                        cmdCheckForTMFinishRecord.Parameters["@model"].Value = strModel;
                                        cmdCheckForTMFinishRecord.Parameters["@model"].Direction = ParameterDirection.Input;


                                        cmdCheckForTMFinishRecord.Parameters.Add("@acsserial", SqlDbType.Char, 20);
                                        cmdCheckForTMFinishRecord.Parameters["@acsserial"].Value = strSerial;
                                        cmdCheckForTMFinishRecord.Parameters["@acsserial"].Direction = ParameterDirection.Input;


                                        cmdCheckForTMFinishRecord.Parameters.Add("@station", SqlDbType.Char, 20);
                                        cmdCheckForTMFinishRecord.Parameters["@station"].Value = strMachineName;
                                        cmdCheckForTMFinishRecord.Parameters["@station"].Direction = ParameterDirection.Input;



                                        cmdCheckForTMFinishRecord.Parameters.Add("@kickerstation", SqlDbType.Char, 20);
                                        cmdCheckForTMFinishRecord.Parameters["@kickerstation"].Value = frmMainForm.strKickerInstanceName.Trim();
                                        cmdCheckForTMFinishRecord.Parameters["@kickerstation"].Direction = ParameterDirection.Input;



                                        cmdCheckForTMFinishRecord.Parameters.Add("@Result", SqlDbType.Char, 80);
                                        cmdCheckForTMFinishRecord.Parameters["@Result"].Direction = ParameterDirection.Output;

                                        using (SqlDataReader rd = cmdCheckForTMFinishRecord.ExecuteReader())
                                        {
                                            if (rd.HasRows)
                                            {
                                                rd.Read();

                                                string strResult = rd["Result"].ToString();
                                                strSAPSN = rd["SAPSN"].ToString().Trim();
                                                strTESTFINISH_RESULT = rd["TESTFINISH_RESULT"].ToString().Trim();
                                                dtTestFinishTime = DateTime.Parse(rd["TESTFINISHTIME"].ToString().Trim());

                                                if (strResult.Trim().Equals("P") || strResult.Trim().Equals("F"))
                                                {
                                                    bTestFinishFound = true;
                                                    if (strResult.Trim().Equals("P"))
                                                    {
                                                        strTestResult = strResult.Trim();
                                                    }
                                                    else
                                                    {
                                                        if (strResult.Trim().Equals("F"))
                                                        {
                                                            strTestResult = strResult.Trim();
                                                        }
                                                        else
                                                        {
                                                            strTestResult = strResult.Trim();
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                }
                            }

                        }
                            
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                          */
                    }
                }
                iTestLoop++;
                if (iTestLoop > frmMainForm.iMaxTMHalfSecondLoops)
                {
                    bTestFinishFound = true;
                }
            }

            if (bTestFinishFound == true)
            {
                bTestFinished = true;
            }
            else
            {
                bTestFinished = false;
            }

            strTestFinish_Result_Return = strTESTFINISH_RESULT;
            strSAPSN_Return = strSAPSN;
            dtFinishTest_Return = dtTestFinishTime;
            return "OK";
        }


        public string UpdateLoci(string strModel, string strACSSerial, string strNextStation)
        {
            string strSqlConnectACSEEState = frmMainForm.strSqlConnection3 ;
            SqlConnection sqlConnectACSEEState;


            try
            {
                using (sqlConnectACSEEState = new SqlConnection(strSqlConnectACSEEState))
                {
                    sqlConnectACSEEState.Open();
                    if (sqlConnectACSEEState.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdUpdateLoci = sqlConnectACSEEState.CreateCommand();
                            cmdUpdateLoci.CommandType = CommandType.StoredProcedure;
                            cmdUpdateLoci.CommandText = "ame_TFFC_update_loci";

                            cmdUpdateLoci.Parameters.Add("@serial", SqlDbType.Char, 20);
                            cmdUpdateLoci.Parameters["@serial"].Value = strACSSerial;
                            cmdUpdateLoci.Parameters["@serial"].Direction = ParameterDirection.Input;


                            cmdUpdateLoci.Parameters.Add("@sapmodel", SqlDbType.Char, 20);
                            cmdUpdateLoci.Parameters["@sapmodel"].Value = strModel;
                            cmdUpdateLoci.Parameters["@sapmodel"].Direction = ParameterDirection.Input;

                            cmdUpdateLoci.Parameters.Add("@nextstation", SqlDbType.Char, 20);
                            cmdUpdateLoci.Parameters["@nextstation"].Value = strNextStation;
                            cmdUpdateLoci.Parameters["@nextstation"].Direction = ParameterDirection.Input;


                            
                            using (SqlDataReader rd = cmdUpdateLoci.ExecuteReader())
                            {
                                if (rd.HasRows)
                                {
                                    rd.Read();
                                    string strLociResult = rd[0].ToString();
                                    if (strLociResult.Trim().Equals("OK"))
                                    {
                                        MessageBox.Show("Error updating loci=" + strLociResult);

                                    }
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    
            
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            return "OK";
        }
    }
}
