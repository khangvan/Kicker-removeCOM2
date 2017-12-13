using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Kicker
{
    public class clsTestRunStatusEventArgs : EventArgs
    {

        public string strTestType;
        public string strTestResult;
        public IntPtr hwndTestWindow;
        public Process testProcess;
        public string strInformation;
        public int iCode;

        public bool bTestFinished;
        public bool bTestDied;

        public string strSAPSN;
        public DateTime dtTestFinish;
        public string strProductionOrder;

    }
}
