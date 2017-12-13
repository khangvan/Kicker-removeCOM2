using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
//using BarTender;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.ComponentModel;
using System.Diagnostics;
using System.Data.OleDb;
using System.Threading ;



namespace Kicker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        [DllImport("User32.dll")]
        static extern bool DestroyWindow(IntPtr hWnd);

        public string strSqlConnection1;
        public string strSqlConnection2;
        public string strSqlConnection3;

        public SqlConnection sqlConnection1;
        public SqlConnection sqlConnection2;
        public SqlConnection sqlConnection3;


        public Dictionary<string, clsSubTestLimit> dictLimitsByModel;
        public Dictionary<string, clsSubTestLimit> dictLimitsByProductionOrder;
        public List<clsStationInfo> listStations;

        public string strModel;
        public string strSerial;

        public bool _vbTestFinished;
        public bool _tmTestFinished;
        public bool _vbDied ;
        public bool _TMDied;

        public string strLastModel;
        public string strLastTestType;

        public string strKickerInstanceName;

        public string strNetProConnectionString1;
        public string strNetProConnectionString2;
        public string strNetProFileLocation;

        public string strTMFilesSourceDirectory;
        public string strTMFilesDestDirectory;

        public int iMaxTMHalfSecondLoops = 99999;
        public int iMaxVBHalfSecondLoops = 99999;
        public DataTable dtLimits;
        public DataTable dtKicker;
        public DataRow drKickerRowforModel;

        public int iDoRework = 0;
        public Boolean bUseTMAccessFiles = false;
        public Boolean bPromptSerialSwitchBox = true;
        public char cSwitchBoxBinarySeparator =';' ;
        public string strTestManagerRunPath = "C:\\idpmdc\\winate32\\dltest32.exe  ";
        public clsRunRealTest objRealTestRunner;

        public clsCheckModelSerialStates objModelStates;


        //Declaring a BarTender application variable 

        //BarTender.Application btApp;

        public TestType myTestType;

        //Declaring the BarTender format variable 

        //BarTender.Format btFormat;

        public string strReservedPSC;

        public string strProductionOrder;

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string strTemp = "";

                sqlConnection1 = new SqlConnection();
                strSqlConnection1 = ConfigurationManager.AppSettings.Get("ACSEECONNECTION").ToString();
                sqlConnection2 = new SqlConnection();
                strSqlConnection2 = ConfigurationManager.AppSettings.Get("ACSEECLIENTSTATECONNECTION").ToString();
                sqlConnection3 = new SqlConnection();
                strSqlConnection3 = ConfigurationManager.AppSettings.Get("ACSEESTATECONNECTION").ToString();

                iDoRework = 0;
                listStations = new List<clsStationInfo>();

                strNetProConnectionString1 = ConfigurationManager.AppSettings.Get("NETPRODBLocation").ToString() ;
                strNetProConnectionString2 = ConfigurationManager.AppSettings.Get("NETPROSecondDBLocation").ToString();
                strNetProFileLocation = ConfigurationManager.AppSettings.Get("NETPROFilesLocation").ToString();

                string strEnglishphrase = "" ;
                string strForeignphrase = "" ;
                if (!ConfigurationManager.AppSettings.Get("runLocation").ToString().Trim().Equals("Eugene"))
                {
     //               labelSKModel.Visible = true;
     //               labelSKModel.Text = "(" + getForeignPhrase("MODEL", ref strEnglishphrase, ref strForeignphrase) + ")";

                    labelSKSerial.Visible = true;
                    labelSKSerial.Text = "(" + getForeignPhrase("ACSSERIALNUMBER", ref strEnglishphrase, ref strForeignphrase) + ")";

                    labelSKProductionOrder.Visible = true;
                    labelSKProductionOrder.Text = "(" + getForeignPhrase("PRODUCTIONORDER", ref strEnglishphrase, ref strForeignphrase) + ")";

                }

                strTMFilesSourceDirectory = ConfigurationManager.AppSettings.Get("TMSourceDirectory").ToString().Trim();

                using (new ImpersonateUser("fixture", "DL", "stark300")) strTMFilesDestDirectory = ConfigurationManager.AppSettings.Get("TMDestDirectory").ToString().Trim();
                iMaxTMHalfSecondLoops = Int32.Parse(ConfigurationManager.AppSettings.Get("TMTestMaxHalfSeconds").ToString()) ;
                iMaxVBHalfSecondLoops = Int32.Parse(ConfigurationManager.AppSettings.Get("VBTestMaxHalfSeconds").ToString());
                strTemp = ConfigurationManager.AppSettings.Get("UseAccessTables").ToString();
                if (strTemp.Trim().Equals("false"))
                {
                    bUseTMAccessFiles = false;
                }
                else
                {
                    bUseTMAccessFiles = true;
                }

                strTemp = ConfigurationManager.AppSettings.Get("TestManagerRunPath").ToString();
                if (strTemp.Trim().Length > 0)
                {
                    strTestManagerRunPath = strTemp;
                }

                strTemp = ConfigurationManager.AppSettings.Get("PromptSerialSwitchBox").ToString().Trim();
                if (strTemp.Trim().Equals("true"))
                {
                    bPromptSerialSwitchBox = true;
                }
                else
                {
                    bPromptSerialSwitchBox = false;
                }

                cSwitchBoxBinarySeparator = ConfigurationManager.AppSettings.Get("SwitchBoxBinarySeparatorChar").ToString().Trim()[0];

/*                strPrintPackCustomerLabelName = ConfigurationManager.AppSettings.Get("PrintPackCustomerLabel").ToString();
                strPrintPackOverpackContentLabelName = ConfigurationManager.AppSettings.Get("PrintPackOverpackContentLabel").ToString();
                strPrintPackOverpackModelLabelName = ConfigurationManager.AppSettings.Get("PrintPackOverpackModelLabel").ToString();

          
*/

                strLastModel = "";
                strLastTestType = "";

                using (sqlConnection1 = new SqlConnection(strSqlConnection1))
                {
                    sqlConnection1.Open();
                    if (sqlConnection1.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            SqlCommand cmdGetLimits = sqlConnection1.CreateCommand();
                            cmdGetLimits.CommandType = CommandType.StoredProcedure;
                            cmdGetLimits.CommandText = "ame_report_subtestlimits"; ;


                            cmdGetLimits.Parameters.Add("@astation", SqlDbType.Char, 20);
                            cmdGetLimits.Parameters["@astation"].Value = "Kicker";
                            cmdGetLimits.Parameters["@astation"].Direction = ParameterDirection.Input;

                            SqlDataAdapter adapter = new SqlDataAdapter(cmdGetLimits);
                            DataSet ds = new DataSet();
                            adapter.Fill(ds, "Limits");

                            dtLimits = ds.Tables["LIMITS"];



                            SqlCommand cmdGetKicker = sqlConnection1.CreateCommand();
                            cmdGetKicker.CommandType = CommandType.StoredProcedure;
                            cmdGetKicker.CommandText = "ame_TFFC_GetKickerTable";

                            SqlDataAdapter adapterKicker = new SqlDataAdapter(cmdGetKicker);
                            DataSet dsKick = new DataSet();
                            adapterKicker.Fill(dsKick, "KICKER");

                            dtKicker = dsKick.Tables["KICKER"];




                            //objRealTestRunner = new clsRunRealTest(dictLimitsByModel);

                            objRealTestRunner = new clsRunRealTest(dtLimits, dtKicker,this);
                            objRealTestRunner.TestFinished += this.TestFinishedHandler;
                            objRealTestRunner.GUIUpdate += this.checkandUpdateGui;
                            objModelStates = new clsCheckModelSerialStates(strSqlConnection1);


                            //Instantiating the BarTender object 

                   //bartender         btApp = new BarTender.ApplicationClass();

                            //Setting the BarTender Application Visible 

                            //bartender         btApp.Visible = false;

                            string strCWD = Directory.GetCurrentDirectory();

                            //Setting the BarTender format to open  
                          //  string strFullPrintPath = Path.Combine(new string[] { strCWD, "PSCSERIAL.btw" });
                            //bartender              btFormat = btApp.Formats.Open(strFullPrintPath, false, ""); 

                            string strProgramMDB = Path.Combine(new string[] { strCWD, "Program.mdb" });

                            string strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="; //\\\\svksrv722.psc.pscnet.com\\prd\\dldb\\NetPro\\Tests.mdb;Persist Security Info=True;Jet OLEDB:Database Password=callisto";
                            strAccessConn += strProgramMDB.Trim();
                            strAccessConn += ";Persist Security Info=True;";

                            string strStationQuery = "SELECT test_name, test_instance from [setup] where test_name = 'KICKER'";
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
                            myDataAdapter.Fill(dsStations, "KICKERSTATIONINFO");

                            DataTable dtStations = dsStations.Tables[0];
                            if (dtStations.Rows.Count > 0)
                            {
                                string strKickerInstance = dtStations.Rows[0]["test_instance"].ToString().Trim();
                                this.strKickerInstanceName = "KICKER" + strKickerInstance;
                            }
                            else
                            {
                                if (!ConfigurationManager.AppSettings.Get("runLocation").ToString().Trim().Equals("Eugene"))
                                {
                                    string strEnglishPhrase = "";
                                    string strForeignPhrase = "";
                                    MessageBox.Show("Kicker not defined in Program.mdb (" + getForeignPhrase("KICKERNOTDEFINE", ref strEnglishphrase, ref strForeignphrase) + ")");
                                }
                                else
                                {
                                    MessageBox.Show("Kicker not defined in Program.mdb");
                                }
                            }


           //                 string teststr1 = dtLimits.Rows[0]["Station_Name"].ToString();
           //                 string teststr2 = dtLimits.Rows[1]["Station_Name"].ToString();


                            this.Location = new Point(0, 0);

//                 //           string strResult = objRealTestRunner.LookupTMInfo("000700036340","GD4110-BK-C140", strSerial);



                            // close running VB or TM processes
                            Process TMProcess;
                            Process VBProcess;
                            IntPtr hwndTM;
                            IntPtr hwndVB;
                            Process[] processes;

                            string strProcessName = "dltest32";
                            processes = Process.GetProcessesByName(strProcessName);

                            for (int i = 0; i < processes.Count(); i++)
                            {
                                TMProcess = processes[i];
                                hwndTM = TMProcess.MainWindowHandle;
                                DestroyWindow(hwndTM);
                            }

                            strProcessName = "GT";

                            for (int i = 0; i < processes.Count(); i++)
                            {
                                VBProcess = processes[i];
                                hwndVB = VBProcess.MainWindowHandle;
                                DestroyWindow(hwndVB);
                            }


                            /*
                            IEnumerable<DataRow> acsquery =
      from limit in dtLimits.AsEnumerable()
      where limit.Field<string>("SAP_Model_Name").Trim() == "TESTEST" &&
      limit.Field<string>("Subtest_Name").Trim() == "ACSTEST"
      select limit;
                            if (acsquery.Count() > 0)
                            {
                                MessageBox.Show("Got One");
                            }
 */
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

        }

        private void txtModel_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void txtModel_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            string strReturn;
            if (e.KeyChar == 13)
            {   //Enter Pressed}
                txtModel.Text = txtModel.Text.ToUpper().Trim();

                _vbTestFinished = false;
                _tmTestFinished = false;
                if (txtModel.Text.Substring(0, 1).Equals("S"))
                {
                    if (!txtModel.Text.Substring(0, 3).Equals("SVC"))
                    {
                        
                        
                        // Not SVC- then  Model
                        txtSerial.Focus();
                        
                        

                        
                    }
                    else
                    { // here serial start with SVC

                        string strTempModel = "";
                        string strTempSerial = "";


                        strTempSerial = txtModel.Text.Trim();
                        // Model is Serial-kvanSep122016
                        strTempModel = objModelStates.GetModelbySerial(strTempSerial);


                        // Assign Serial and Do Enter-kvanSep122016

                        txtSerial.Text = strTempSerial;
                        txtModel.Text = strTempModel;

                        DoKeyPress(e, true);


                        //txtSerial.Text = txtModel.Text;
                        //txtModel.Text = "";
                    }
                }
                else
                {
                    if (txtSerial.Text.ToString().Trim().Length > 0)  // we have a model serial combination
                    {
                        string strProdOrder = "";
                        string strPrintType = "";

                        strModel = txtModel.Text.ToString().Trim();
                        strSerial = txtSerial.Text.ToString().Trim();

                        if (chkRework.Checked == true)
                        {
                            iDoRework = 1;
                        }
                        else
                        {
                            iDoRework = 0;
                        }

                        strReturn = objModelStates.checkForPrePrint(strModel, strSerial, ref strProdOrder, ref strPrintType);
                        if (!strReturn.Equals("OK"))
                        {
                            if (!ConfigurationManager.AppSettings.Get("runLocation").ToString().Trim().Equals("Eugene"))
                            {
                                string strEnglishphrase = "";
                                string strForeignphrase = "";
                                MessageBox.Show("Order not started yet! (" + getForeignPhrase("ORDERNOSTARTED", ref strEnglishphrase, ref strForeignphrase) + ")");
                            }
                            else
                            {

                                MessageBox.Show("Order not started yet!");
                            }
                            return;
                        }

                        string strExecString = "";
                        string strNextStation = "";
                        myTestType = objRealTestRunner.DetermineTestType(strModel, out strExecString, out strNextStation);
                        txtProdOrder.Text = strProdOrder;
                        bool bRework;
                        string strLociResult = objRealTestRunner.checkNextStationDBLoci(strModel, strSerial, drKickerRowforModel["TFFC_KICKER_Station"].ToString().Trim(), this.strSqlConnection3, out bRework);

                        if (strPrintType.Trim().Equals("D") && (myTestType == TestType.TMTest))
                        {

                            strReservedPSC = "";
       //                     strReservedPSC = objModelStates.ReserveSerialForProductionOrder(strModel, strSerial, strProdOrder, "KICKER", ref strReservedPSC);

                            if (strReservedPSC.Trim().Equals("BAD") && myTestType == TestType.TMTest)
                            {
                            }
                            else
                            {
                                if (strReservedPSC.Trim().Equals("FUL"))
                                {
                                }
                                else
                                {
                                    txtSAPSerial.Text = strReservedPSC;
                                    lblPSCBarcode.Text = "*" + strReservedPSC.Trim() + "*";
                                }
                            }
                        }
                        else
                        {
                            txtSAPSerial.Enabled = false;
                            lblPSCBarcode.Visible = false;
                        }

                        txtModel.Enabled = false;
                        txtSerial.Enabled = false;



                        _vbTestFinished = false;
                        _tmTestFinished = false;
                        strReturn = objRealTestRunner.ProcessModelSerialPair(strProdOrder, strModel, strSerial, strSqlConnection3, this);


                        if (strReturn.Trim().Equals("OK"))
                        {
                            //   this.txtModel.Text = "";
                            //   this.txtSerial.Text = "";
                            //          this.txtSAPSerial.Text = "";
                            //     this.txtProdOrder.Text = "";

                            while (_vbTestFinished == false && _tmTestFinished == false)
                            {
                                Thread.Sleep(500);
                            }
                            if (_vbTestFinished == true)
                            {
                                this.txtModel.Enabled = true;
                                this.txtSerial.Enabled = true;
                                this.txtModel.Text = "";
                                this.txtSerial.Text = "";
                                this.txtProdOrder.Text = "";
                            }
                            else
                            {
                                if (_tmTestFinished == true)
                                {
                                    txtModel.Enabled = true;
                                    txtSerial.Enabled = true;

                                    this.txtModel.Text = "";
                                    this.txtSerial.Text = "";
                                    this.txtProdOrder.Text = "";

                                    this.txtModel.Focus();
                                }
                            }
                            this.txtModel.Focus();
                        }
                    }
                    else
                    {
                        txtSerial.Focus();
                    }
                }
            }

        }

        private void txtSerial_KeyPress(object sender, KeyPressEventArgs e)
       {
           DoKeyPress(e, false);

           
          
         
        }

        /// <summary>
        /// true= Do and NO CAR KEY CHAR
        /// fale= Please send me Key char
        /// </summary>
        /// <param name="e"></param>
        /// <param name="DoAllWay"></param>
        private void DoKeyPress(KeyPressEventArgs e, bool DoAllWay)
        {
            string strReturn;
            e.KeyChar = (DoAllWay == true) ? (char)13 : e.KeyChar;
            
            if (e.KeyChar == 13)
            {

                txtSerial.Text = txtSerial.Text.ToUpper().Trim();
                if (!txtSerial.Text.Substring(0, 1).ToUpper().Equals("S"))
                {
                    txtModel.Text = txtSerial.Text;
                    txtSerial.Text = "";
                }
                else
                {
                    _vbDied = false;
                    _vbTestFinished = false;
                    if (txtModel.Text.ToString().Trim().Length > 0) // we have a model serial combination
                    {
                        string strProdOrder = "";
                        string strPrintType = "";

                        strModel = txtModel.Text.ToString().Trim();
                        strSerial = txtSerial.Text.ToString().Trim();

                        #region GetModel by Serial - Sep 12 kvan

                        ///if 
                        ///serial is not empty
                        ///and model = empty
                        ///do get model from loci data
                        ///

                        if ((string.IsNullOrEmpty(strModel)) && !(string.IsNullOrEmpty(strSerial)))
                        {
                            //get model

                            strModel = objModelStates.GetModelbySerial(strSerial);
                            txtModel.Text = strModel;
                        }



                        #endregion

                        if (chkRework.Checked == true)
                        {
                            iDoRework = 1;
                        }
                        else
                        {
                            iDoRework = 0;
                        }
                        strReturn = objModelStates.checkForPrePrint(strModel, strSerial, ref strProdOrder, ref strPrintType);
                        if (!strReturn.Equals("OK"))
                        {
                            if (!ConfigurationManager.AppSettings.Get("runLocation").ToString().Trim().Equals("Eugene"))
                            {
                                string strEnglishphrase = "";
                                string strForeignphrase = "";
                                MessageBox.Show("Order not started yet! (" + getForeignPhrase("ORDERNOSTARTED", ref strEnglishphrase, ref strForeignphrase) + ")");
                            }
                            else
                            {

                                MessageBox.Show("Order not started yet!");
                            }

                            return;
                        }

                        string strExecString = "";
                        string strNextStation = "";
                        myTestType = objRealTestRunner.DetermineTestType(strModel, out strExecString, out strNextStation);
                        txtProdOrder.Text = strProdOrder;
                        bool bRework;
                        //                      string strLociResult = objRealTestRunner.checkNextStationDBLoci(strModel, strSerial, drKickerRowforModel["TFFC_KICKER_Station"].ToString().Trim(), this.strSqlConnection3, out bRework);

                        if (strPrintType.Trim().Equals("D") && (myTestType == TestType.TMTest))
                        {

                            strReservedPSC = "";
                            //                  strReservedPSC = objModelStates.ReserveSerialForProductionOrder(strModel, strSerial, strProdOrder, "KICKER", ref strReservedPSC);

                            if (strReservedPSC.Trim().Equals("BAD") && myTestType == TestType.TMTest)
                            {
                            }
                            else
                            {
                                if (strReservedPSC.Trim().Equals("FUL"))
                                {
                                }
                                else
                                {
                                    txtSAPSerial.Text = strReservedPSC;
                                    lblPSCBarcode.Text = "*" + strReservedPSC.Trim() + "*";
                                }
                            }
                        }
                        else
                        {
                            txtSAPSerial.Enabled = false;
                            lblPSCBarcode.Visible = false;
                        }

                        txtModel.Enabled = false;
                        txtSerial.Enabled = false;



                        _vbTestFinished = false;
                        _tmTestFinished = false;

                        #region CHUYỂN NGUỒN COM 2
                        strReturn = objRealTestRunner.ProcessModelSerialPair(strProdOrder, strModel, strSerial, strSqlConnection3, this);
                        #endregion


                        if (strReturn.Trim().Equals("OK"))
                        {
                            //   this.txtModel.Text = "";
                            //   this.txtSerial.Text = "";
                            //          this.txtSAPSerial.Text = "";
                            //     this.txtProdOrder.Text = "";

                            //                           while (_vbTestFinished == false && _tmTestFinished == false)
                            //                           {
                            //                              Thread.Sleep(500);
                            //                              this.Refresh();
                            checkandUpdateGui(this);
                            //                           }
                            if (_vbTestFinished == true)
                            {
                                this.txtModel.Enabled = true;
                                this.txtSerial.Enabled = true;
                                this.txtModel.Text = "";
                                this.txtSerial.Text = "";
                                this.txtProdOrder.Text = "";
                            }
                            else
                            {
                                if (_tmTestFinished == true)
                                {
                                    txtModel.Enabled = true;
                                    txtSerial.Enabled = true;

                                    this.txtModel.Text = "";
                                    this.txtSerial.Text = "";
                                    this.txtProdOrder.Text = "";

                                    this.txtModel.Focus();
                                }
                            }
                            this.txtModel.Focus();
                        }
                    }

                    else   // set focus to model number field
                    {
                        // txtModel.Focus();
                    }
                }
            }
        }

        private void txtSerial_TextChanged(object sender, EventArgs e)
        {

        }




        public delegate void SetGUIDelegate(Form1 myForm);

        public void checkandUpdateGui(Form1 myForm)
        {
            if (myForm.InvokeRequired)
            {
                this.Invoke(new SetGUIDelegate(UpdateGUI));
            }
            else
            {
                this.Refresh();
            }
        }

        public void UpdateGUI(Form1 myForm)
        {
            myForm.Refresh();
        }


        public void UpdateTestGUI(object sender, clsTestRunStatusEventArgs e)
        {
 //           if (this.InvokeRequired)
 //           {
 //               this.BeginInvoke((MethodInvoker)delegate { TestFinishedHandler(sender, e); });
 //               return;
 //           }

            if ((_vbTestFinished == true) || ( _vbDied == true ))
            {
                this.txtModel.Enabled = true;
                this.txtSerial.Enabled = true;
                this.txtModel.Text = "";
                this.txtSerial.Text = "";
                this.txtProdOrder.Text = "";
            }
            else
            {
                if ((_tmTestFinished == true) || (_TMDied == true ))
                {
                    txtModel.Enabled = true;
                    txtSerial.Enabled = true;

                    this.txtModel.Text = "";
                    this.txtSerial.Text = "";
                    this.txtProdOrder.Text = "";

                    this.txtModel.Focus();
                }
            }
            this.txtModel.Focus();
        }

        public void TestFinishedHandler(object sender, clsTestRunStatusEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { TestFinishedHandler(sender,e) ;} ) ;
                return;
            }
            clsTestRunStatusEventArgs myReturnValues = e;

      //      MessageBox.Show("Test Finished");

            if (myTestType.Equals(TestType.TMTest))
            {
                /*
                if (myReturnValues.strTestResult.Trim().Equals("P"))
                {
                 * */
                    /*
                    if (!myReturnValues.strSAPSN.Trim().Equals(strReservedPSC.Trim()))
                    {
 //                       objModelStates.UnReserveSerial(strModel, strSerial, strProductionOrder, "KICKER", ref strReservedPSC);
                        //  strReservedPSC = objModelStates.ReserveSerialForProductionOrder(strModel, strSerial, strProductionOrder, "KICKER", ref strReservedPSC);
//                        objModelStates.CommitSerial(strModel, strSerial, strProductionOrder, "KICKER", myReturnValues.strSAPSN.Trim(), ref strReservedPSC);
                    }
                    else
                    {
                        objModelStates.CommitSerial(strModel, strSerial, strProductionOrder, "KICKER", myReturnValues.strSAPSN.Trim(), ref strReservedPSC);

                    }
                     * */
                this.txtModel.Enabled = true;
                this.txtSerial.Enabled = true;
/*
                    IEnumerable<DataRow> acsquery =
    from kicker in dtKicker.AsEnumerable()
    where kicker.Field<string>("TFFC_KICKER_Model").Trim() == strModel.Trim()
    select kicker;
                     DataTable dtResult = acsquery.CopyToDataTable<DataRow>();
                     if (dtResult.Rows.Count > 0)
                     {
                         drKickerRowforModel = dtKicker.Rows[0];
                     }
                     else
                     {
                         MessageBox.Show("No Entry found in TFFC_KICKER_TABLE");
                     }

                    string strLociReturn = objRealTestRunner.UpdateLoci(strModel,strSerial,dtResult.Rows[0]["TFFC_KICKER_QBUDDIES"].ToString()) ;
 */
  /*
                }
                else
                {
                    objModelStates.UnReserveSerial(strModel, strSerial, strProductionOrder, "KICKER", ref strReservedPSC);

                }
                */
                _tmTestFinished = true;
                if (e.bTestDied == true)
                {
                    _TMDied = true;
                }
            }
            else
            {
                if (myTestType.Equals(TestType.VBTest))
                {
                    _vbTestFinished = true;

//                    this.txtModel.Focus();
                    if (e.bTestDied == true)
                    {
                        _vbDied   = true;
                    }

                }
            }
            
            UpdateTestGUI(sender,e);
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
         ///   btApp.Quit();
            if (objRealTestRunner != null)
            {
                if (objRealTestRunner.hwndVB != (IntPtr)0)
                {
                    bool bRet = DestroyWindow(objRealTestRunner.hwndVB);
                }
                if (objRealTestRunner.hwndTM != (IntPtr)0)
                {
                    bool bRet = DestroyWindow(objRealTestRunner.hwndTM);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtModel.Enabled = true;
            txtSerial.Enabled = true;
        }

        private void txtSAPSerial_TextChanged(object sender, EventArgs e)
        {

        }


        public string getForeignPhrase(string strPhraseKey, ref string strEnglishPhrase, ref string strForeignPhrase)
        {
            string strReturnPhrase = "";

            strEnglishPhrase = "";
            strForeignPhrase = "";

            using (sqlConnection1 = new SqlConnection(strSqlConnection1))
            {
                sqlConnection1.Open();
                if (sqlConnection1.State.Equals(ConnectionState.Open))
                {
                    try
                    {
                        SqlCommand cmdGetPhrase = sqlConnection1.CreateCommand();
                        cmdGetPhrase.CommandType = CommandType.StoredProcedure;
                        cmdGetPhrase.CommandText = "ame_get_foreignphrase"; ;


                        cmdGetPhrase.Parameters.Add("@phrasekey", SqlDbType.Char, 20);
                        cmdGetPhrase.Parameters["@phrasekey"].Value = strPhraseKey;
                        cmdGetPhrase.Parameters["@phrasekey"].Direction = ParameterDirection.Input;


                        SqlDataReader myPhrase = cmdGetPhrase.ExecuteReader();

                        myPhrase.Read();
                        strEnglishPhrase = myPhrase["PHRASE_ENGLISH"].ToString().Trim();
                        strForeignPhrase = myPhrase["PHRASE_FOREIGN"].ToString().Trim();
                        strReturnPhrase = strForeignPhrase;

                    }
                    catch (Exception ex)
                    {
                        strEnglishPhrase = "";
                        strForeignPhrase = "";
                        strReturnPhrase = "";
                    }
                }
            }


            return strReturnPhrase;
        }
        private static void CheckFolderExistthenCreate(string Des)
        {

            // Using File
            if (!(Directory.Exists(Des)))
            {
                Directory.CreateDirectory(Des);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            
            
            string strSource = @"\\vnmsrv608\DevelopSoftware\P20-KickerWO_PWControl\PS3645A on COM 6.exe";
            string strDes = @"C:\DATA\ColumbusConfig\Power Supply Stand Alone for XP" + @"\PS3645A on COM 6.exe";
                //"C:\kicker-pwCOM6\Kicker\bin\Debug\Power Supply Stand Alone for XP\PS3645A on COM 6.exe";
            CheckFileExistthenCopy(strSource, strDes);
            #region MỞ NGUỒN COM 6 CHO TM / OPEN PW VIA COM6

            Process.Start(strDes);

            #endregion
        }

        private static void CheckFileExistthenCopy(string Source, string Des)
        {
            // Using File
            if (!(File.Exists(Des)))
            {
                File.Copy(Source, Des);
            }


        }

    }
}
