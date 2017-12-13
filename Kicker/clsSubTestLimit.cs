using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kicker
{
    public class clsSubTestLimit
    {
        public string strStation_Name;
        public string strSubtest_Name;
        public string strSAP_Model_Name;
        public string strLimit_Type;
        public float fltUL;
        public float fltLL;
        public string strStrLimit;
        public string strFlgLimit;
        public string strUnits;
        public string strDescription;
        public string strAuthor;
        public int iACSMode;
        public string strSPCParm;
        public float fltHard_UL;
        public float flthard_LL;
        public DateTime dtLimit_Date;
        public int iProductGroup_Mask;
        public int iLimit_ID;
        public int iNote_ID;
        public int iOpportunitiesForFail;

        public string getKeyString()
        {
            return this.strSAP_Model_Name + this.strSubtest_Name;
        }
    }
}
